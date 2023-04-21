using ChatApp.Business.ServiceInterfaces;
using ChatApp.Context;
using ChatApp.Context.EntityClasses;
using ChatApp.Models;
using Microsoft.AspNetCore.SignalR;
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

		public chatHub(ChatAppContext context, IChatService chatservice)
		{
			this.context = context;
			this.chatService = chatservice;
		}

		public override Task OnConnectedAsync()
		{
			return base.OnConnectedAsync();
		}

		public override Task OnDisconnectedAsync(Exception exception)
		{
			if (exception != null)
			{
				Console.WriteLine(exception.Message);
			}
			return base.OnDisconnectedAsync(exception);
		}

		public async Task ConnectDone(string userName)
		{
			string curSignalId = Context.ConnectionId;
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
				int userId = context.Profiles.FirstOrDefault(u => u.UserName == userName).Id;
				var connect = context.Connections.FirstOrDefault(u => u.ProfileId == userId);
				context.Connections.Remove(connect);
				context.SaveChanges();
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
			this.chatService.ResponsesToUsersMessage(messageFromId, messageToId, response);
		}

	}
}
