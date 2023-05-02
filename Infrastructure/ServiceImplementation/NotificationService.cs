using ChatApp.Business.ServiceInterfaces;
using ChatApp.Context;
using ChatApp.Context.EntityClasses;
using ChatApp.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.SignalR;
using Microsoft.VisualBasic.FileIO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text.RegularExpressions;

namespace ChatApp.Infrastructure.ServiceImplementation
{
	public class NotificationService : INotificationServices
	{
		private readonly ChatAppContext context;
		private readonly IHubContext<chatHub> hubContext;
		private readonly IProfileService profileService;
		public NotificationService(ChatAppContext context, IHubContext<chatHub> hubContext, IProfileService profileService)
		{
			this.context = context;
			this.hubContext = hubContext;
			this.profileService = profileService;
		}

		public IEnumerable<NotificationsDTO> GetAllNotifications(string userName)
		{
			if(context.Profiles.Any(u=>u.UserName==userName))
			{
				var response = new List<NotificationsDTO>();
				var unSortedResponse = new List<NotificationsDTO>();
				int userId = profileService.GetIdFromUserName(userName);
				var Notifications = context.Notifications.Where(u => u.ToId == userId).ToList();
				foreach(var notification in Notifications)
				{
					var profile = context.Profiles.FirstOrDefault(u=>u.Id==notification.FromId);
					var notif = new NotificationsDTO
					{
						Id= notification.Id,
						MsgFrom = profile.UserName,
						MsgTo = profileService.GetUserNameFromId(notification.ToId),
						GroupId = notification.GroupId,
						Content = notification.Content,
						CreatedAt = notification.CreatedAt,
						FromImage = profile.ImagePath
					};
					unSortedResponse.Add(notif);
				}
				response = unSortedResponse.OrderByDescending(m=>m.CreatedAt).ToList();
				return response;
			}
			return Enumerable.Empty<NotificationsDTO>();
		}

		public void Clear(string userName)
		{
			int Id = profileService.GetIdFromUserName(userName);
			var notifications = context.Notifications.Where(u => u.ToId == Id);
			if (notifications != null)
			{
				context.RemoveRange(notifications);
				context.SaveChanges();
			}
		}

		public void addToGroup(List<int> userIds ,int loginUserId , int groupId)
		{
			foreach (var userId in userIds) {
				addNotificationToDb(userId, loginUserId, groupId ,"Add");
				var connect = context.Connections.FirstOrDefault(u => u.ProfileId == userId);
				if (connect != null)
				{
					var notif = convertToNotificationDTO(userId, loginUserId, groupId);
					this.hubContext.Clients.Client(connect.SignalId).SendAsync("notification", notif);
				}
			}
		}

		public void adminNoti(int userId,int loginUserId , int groupId)
		{
			addNotificationToDb(userId, loginUserId, groupId, "Admin");
			var connect = context.Connections.FirstOrDefault(u => u.ProfileId == userId);
			if (connect != null)
			{				
				var notif = convertToNotificationDTO(userId, loginUserId, groupId);
				this.hubContext.Clients.Client(connect.SignalId).SendAsync("notification", notif);
			}
		}

		public void removeNoti(int userId , int loginUserId , int groupId)
		{
			addNotificationToDb(userId, loginUserId, groupId, "Removed");
			var connect = context.Connections.FirstOrDefault(u => u.ProfileId == userId);
			if (connect != null)
			{
				var notif = convertToNotificationDTO(userId, loginUserId, groupId);
				this.hubContext.Clients.Client(connect.SignalId).SendAsync("notification", notif);
			}
		}

		public void groupMsgNoti(int loginUserId , int groupId , string msgtype)
		{
			var usersId = context.GroupMembers.Where(u=>u.GrpId== groupId&&u.ProfileId!=loginUserId).Select(u=>u.ProfileId).ToList();
			foreach (var userId in usersId)
			{
				addNotificationToDb(userId, loginUserId, groupId, msgtype);
				var connect = context.Connections.FirstOrDefault(u => u.ProfileId == userId);
				if (connect != null)
				{
					var notif = convertToNotificationDTO(userId, loginUserId, groupId);
					this.hubContext.Clients.Client(connect.SignalId).SendAsync("notification", notif);
				}
			}
		}

		#region Methods
		public void addNotificationToDb(int userId , int loginUserId , int groupId , string type)
		{
			var profile = context.Profiles.FirstOrDefault(u => u.Id == loginUserId);
			var group = context.Groups.FirstOrDefault(u => u.Id == groupId);
			var existingNoti = context.Notifications.FirstOrDefault(u => u.ToId == userId && u.FromId == loginUserId && u.GroupId == groupId);
			if (existingNoti == null)
			{
				var Notification = new Notifications
				{
					FromId = loginUserId,
					ToId = userId,
					GroupId = groupId,
					CreatedAt = DateTime.Now,
				};
				if(type == "Add") {	Notification.Content = profile.UserName + " Added You To " + group.GroupName;	}
				else if(type == "Admin") { Notification.Content = profile.UserName + " Made You Admin of " + group.GroupName;  }
				else if (type == "Removed") { Notification.Content = profile.UserName + " Removed You From " + group.GroupName; }
				else if (type == "Removed") { Notification.Content = profile.UserName + " Removed You From " + group.GroupName; }
				else if (type == "audio") { Notification.Content = profile.UserName + " Sent " + type + " In " + group.GroupName; }
				else if (type == "video") { Notification.Content = profile.UserName + " Sent " + type + " In " + group.GroupName; }
				else if (type == "image") { Notification.Content = profile.UserName + " Sent " + type + " In " + group.GroupName; }
				else if (type == "groupText") { Notification.Content = profile.UserName + " Sent Text In " + group.GroupName; }
				context.Notifications.Add(Notification);
				context.SaveChanges();
			}
			else
			{
				if (type == "Add") { existingNoti.Content = profile.UserName + " Added You To " + group.GroupName; }
				else if(type == "Admin") { existingNoti.Content = profile.UserName + " Made You Admin of " + group.GroupName;  }
				else if (type == "Removed") { existingNoti.Content = profile.UserName + " Removed You From " + group.GroupName; }
				else if (type == "audio") { existingNoti.Content = profile.UserName + " Sent " + type + " In " + group.GroupName; }
				else if (type == "video") { existingNoti.Content = profile.UserName + " Sent " + type + " In " + group.GroupName; }
				else if (type == "image") { existingNoti.Content = profile.UserName + " Sent " + type + " In " + group.GroupName; }
				else if (type == "groupText") { existingNoti.Content = profile.UserName + " Sent Text In " + group.GroupName; }
				existingNoti.CreatedAt = DateTime.Now;
				context.Update(existingNoti);
				context.SaveChanges();
			}
		}

		public NotificationsDTO convertToNotificationDTO(int userId , int loginUserId , int groupId)
		{
			var notification = context.Notifications.FirstOrDefault(u => u.ToId == userId && u.FromId == loginUserId && u.GroupId == groupId);
			var profile = context.Profiles.FirstOrDefault(u => u.Id == loginUserId);
			var notif = new NotificationsDTO
			{
				Id = notification.Id,
				MsgFrom = profile.UserName,
				MsgTo = profileService.GetUserNameFromId(userId),
				GroupId = notification.GroupId,
				Content = notification.Content,
				CreatedAt = notification.CreatedAt,
				FromImage = profile.ImagePath
			};
			return notif;
		}
		#endregion
	}
}
