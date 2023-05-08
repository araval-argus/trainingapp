using ChatApp.Business.ServiceInterfaces;
using ChatApp.Context;
using ChatApp.Context.EntityClasses;
using ChatApp.Models;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualBasic.FileIO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChatApp
{
	public class chatHub : Hub
	{
		private readonly ChatAppContext context;
		private readonly IChatService chatService;
		private readonly INotificationServices notificationServices;

		public chatHub(ChatAppContext context, IChatService chatservice, INotificationServices notificationServices)
		{
			this.context = context;
			this.chatService = chatservice;
			this.notificationServices = notificationServices;
		}

		public override async Task OnDisconnectedAsync(Exception exception)
		{
			var connect = context.Connections.FirstOrDefault(u=>u.SignalId==Context.ConnectionId);
			if (connect != null){
				context.RemoveRange(connect);
				context.SaveChanges();
			}
		}

		#region OneToOneHub
		public async Task ConnectDone(string userName)
		{
			var curSignalId = Context.ConnectionId;
			Profile user = context.Profiles.FirstOrDefault(p => p.UserName == userName);

			if (user != null) {

				if (this.context.Connections.Any(u => u.ProfileId == user.Id))
				{
					IEnumerable<Connections> users = this.context.Connections.Where(u => u.ProfileId == user.Id);
					this.context.Connections.RemoveRange(users);
					context.SaveChanges();
				}

				Connections connUser = new Connections()
				{
					ProfileId = user.Id,
					SignalId = curSignalId,
					TimeStamp = DateTime.Now,
				};
				await context.Connections.AddAsync(connUser);
				context.SaveChanges();

				await Clients.Caller.SendAsync("ResponseSuccess", user);
			}
			else
			{
				await Clients.Client(curSignalId).SendAsync("ResponseFail");
			}

		}

		public void ConnectRemove(string userName)
		{
			if (userName != null)
			{
				var user = context.Profiles.FirstOrDefault(u => u.UserName == userName);
				user.Status = 6;
				context.Profiles.Update(user);
				context.SaveChanges();
				Clients.All.SendAsync("userStatusChanged",userName,"Offline");
				var connect = context.Connections.FirstOrDefault(u => u.ProfileId == user.Id);
				if (connect != null)
				{
					context.Connections.Remove(connect);
					context.SaveChanges();
				}
			}
			else
			{
				return;
			}
		}

		public async Task sendMsg(MessageModel message)
		{
			Message newMessage = null;
			MessageSendModel response = null;
			string replyMessage;
			int messageFromId = chatService.GetIdFromUserName(message.MessageFrom);
			int messageToId = chatService.GetIdFromUserName(message.MessageTo);
			newMessage = new Message
			{
				Content = message.Content,
				CreatedAt = DateTime.Now,
				MessageFrom = messageFromId,
				MessageTo = messageToId,
				RepliedTo = (int)message.RepliedTo,
				Seen = 0,
				Type = null,
			};
			context.Messages.Add(newMessage);
			context.SaveChanges();
			if (message.RepliedTo == -1)
			{
				replyMessage = null;
			}
			else
			{
				var msg = context.Messages.FirstOrDefault(msg => msg.Id == message.RepliedTo);
				replyMessage = msg.Content;
			}
			response = new MessageSendModel
			{
				Id = newMessage.Id,
				Content = newMessage.Content,
				CreatedAt = newMessage.CreatedAt,
				MessageFrom = message.MessageFrom,
				MessageTo = message.MessageTo,
				RepliedTo = replyMessage,
				Seen = 0,
				Type = null,
			};
			//NOTIFICATION
			var existingNoti = context.Notifications.FirstOrDefault(u => u.ToId == messageToId && u.FromId == messageFromId && u.GroupId == null);
			if (existingNoti == null)
			{
				var Notification = new Notifications
				{
					FromId = messageFromId,
					ToId = messageToId,
					GroupId = null,
					Content = message.MessageFrom + " Sent You Text",
					CreatedAt = DateTime.Now,
				};
				context.Notifications.Add(Notification);
				context.SaveChanges();
			}
			else
			{
				existingNoti.Content = message.MessageFrom + " Sent You Text";
				existingNoti.CreatedAt = DateTime.Now;
				context.Update(existingNoti);
				context.SaveChanges();
			}
			//SIGNALR RESPONSE
			Connections Sender = this.context.Connections.FirstOrDefault(u => u.ProfileId == messageFromId);
			Connections Receiver = this.context.Connections.FirstOrDefault(u => u.ProfileId == messageToId);
			if (Receiver != null)
			{
				var profile = context.Profiles.FirstOrDefault(u => u.UserName == message.MessageFrom);
				var notification = context.Notifications.FirstOrDefault(u => u.ToId == messageToId && u.FromId == messageFromId && u.GroupId == null);
				var notif = new NotificationsDTO
				{
					Id = notification.Id,
					MsgFrom = profile.UserName,
					MsgTo = message.MessageTo,
					GroupId = notification.GroupId,
					Content = notification.Content,
					CreatedAt = notification.CreatedAt,
					FromImage = profile.ImagePath
				};
				await Clients.Clients(Sender.SignalId, Receiver.SignalId).SendAsync("recieveMessage", response);
				await Clients.Client(Receiver.SignalId).SendAsync("notification", notif);
			}
			else
			{
				await Clients.Client(Sender.SignalId).SendAsync("recieveMessage", response);
			}
		}

		public async Task seenMessage(string msgFrom , string msgTo)
		{
			int msgFromId = context.Profiles.FirstOrDefault(u=>u.UserName== msgFrom).Id;
			int msgToId = context.Profiles.FirstOrDefault(u=>u.UserName== msgTo).Id;

			var msgs = context.Messages.Where(u=>u.MessageFrom==msgFromId && u.MessageTo==msgToId).ToList();
			foreach ( var msg in msgs )
			{
				msg.Seen = 1;
			}
			context.UpdateRange(msgs);
			await context.SaveChangesAsync();

			if(context.Connections.Any(u => u.ProfileId == msgFromId))
			{
				var msgFromSignalId = context.Connections.FirstOrDefault(u => u.ProfileId == msgFromId).SignalId;
				var msgToConnect = context.Connections.FirstOrDefault(u => u.ProfileId== msgToId);
				if(msgToConnect!=null){
					await Clients.Clients(msgFromSignalId, msgToConnect.SignalId).SendAsync("msgSeen", msgFrom);
				}
			}
		}
		#endregion

		#region GroupHub

		public async Task sendGroupMsg(GMessageInModel message)
		{
			GroupMessages newMessage = null;
			GMessageSendModel response = null;
			string replyMessage;
			int messageFromId = chatService.GetIdFromUserName(message.MessageFrom);
			int groupId = message.GroupId;
			newMessage = new GroupMessages
			{
				Content = message.Content,
				CreatedAt = DateTime.Now,
				MessageFrom = messageFromId,
				GrpId= groupId,
				RepliedTo = (int)message.RepliedTo,
				Type = null,
			};
			context.GroupMessages.Add(newMessage);
			context.SaveChanges();
			if (message.RepliedTo == -1)
			{
				replyMessage = null;
			}
			else
			{
				var msg = context.GroupMessages.FirstOrDefault(msg => msg.Id == message.RepliedTo);
				replyMessage = msg.Content;
			}
			var profile = context.Profiles.FirstOrDefault(p => p.Id == messageFromId);
			response = new GMessageSendModel
			{
				Id = newMessage.Id,
				Content = newMessage.Content,
				CreatedAt = (DateTime)newMessage.CreatedAt,
				MessageFrom = message.MessageFrom,
				MessageFromImage = profile.ImagePath,
				RepliedTo = replyMessage,
				Type = null,
			};
			var groupMemberIds = context.GroupMembers.Where(u => u.GrpId == groupId).Select(u => u.ProfileId).ToList();
			foreach(var memberId in groupMemberIds)
			{
				var connection = context.Connections.FirstOrDefault(u => u.ProfileId == memberId);
				if(connection != null)
				{
					await Clients.Clients(connection.SignalId).SendAsync("RecieveMessageGroup", response);
				}
			}
			this.notificationServices.groupMsgNoti(messageFromId, groupId, "groupText");
		}
		#endregion
	}
}
