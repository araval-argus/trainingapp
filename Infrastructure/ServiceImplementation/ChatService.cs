using ChatApp.Business.Helpers;
using ChatApp.Business.ServiceInterfaces;
using ChatApp.Context;
using ChatApp.Context.EntityClasses;
using ChatApp.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration.UserSecrets;
using Microsoft.OpenApi.Any;
using Microsoft.VisualBasic;
using Microsoft.VisualBasic.FileIO;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection.Metadata;
using System.Security.Permissions;

namespace ChatApp.Infrastructure.ServiceImplementation
{
	public class ChatService : IChatService
	{

		private readonly ChatAppContext context;
		private readonly IWebHostEnvironment webHostEnvironment;
		public ChatService(ChatAppContext context, IWebHostEnvironment webHostEnvironment)
		{
			this.context = context;
			this.webHostEnvironment = webHostEnvironment;
		}
		public IEnumerable<ColleagueModel> SearchColleague(string name, string username)
		{
			return context.Profiles
			.Where(profile => (profile.FirstName.ToLower().StartsWith(name) || profile.LastName.ToLower().StartsWith(name)) && profile.UserName != username)
			.Select(colleagues => new ColleagueModel()
			{
				FirstName = colleagues.FirstName,
				LastName = colleagues.LastName,
				Email = colleagues.Email,
				UserName = colleagues.UserName,
				Designation = colleagues.Designation,
				ImagePath = colleagues.ImagePath,
			})
			.ToList();
		}
		public string GetUsername(string Authorization)
		{
			var handler = new JwtSecurityTokenHandler();
			string auth = Authorization.Split(' ')[1];
			var decodedToken = handler.ReadJwtToken(auth);

			return decodedToken.Claims.First(claim => claim.Type == "sub").Value;
		}

		public int GetIdFromUserName(string userName)
		{
			Profile user = context.Profiles.FirstOrDefault(profile => profile.UserName == userName);
			return user.Id;
		}

		public MessageSendModel DoMessage(MessageModel message)
		{
			Message newMessage = null;
			MessageSendModel response = null;
			string replyMessage;
			newMessage = new Message
			{
				Content = message.Content,
				CreatedAt = DateTime.Now,
				MessageFrom = GetIdFromUserName(message.MessageFrom),
				MessageTo = GetIdFromUserName(message.MessageTo),
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
			return response;
		}

		public IEnumerable<MessageSendModel> GetMsg(string userName, string selUserName)
		{
			
			int userId = GetIdFromUserName(userName);
			int selUserId = GetIdFromUserName(selUserName);
			var list = context.Messages
			.Where(msg => (msg.MessageFrom == userId && msg.MessageTo == selUserId) || (msg.MessageFrom == selUserId && msg.MessageTo == userId));
			var returnList = new List<MessageSendModel>();

			foreach (var msg in list)
			{
				var newObj = new MessageSendModel
				{
					Id = msg.Id,
					Content = msg.Content,
					CreatedAt = msg.CreatedAt,
					MessageFrom = (msg.MessageFrom == userId) ? userName : selUserName,
					MessageTo = (msg.MessageTo == userId) ? userName : selUserName,
					Type = msg.Type,
				};
				if(msg.MessageFrom == selUserId && msg.MessageTo == userId)
				{
					newObj.Seen = 1;
					msg.Seen = 1;
				}
				else
				{
					newObj.Seen = msg.Seen;
				}
				if (msg.RepliedTo != -1)
				{
					var curmsg = context.Messages.FirstOrDefault(e => e.Id == msg.RepliedTo);
					newObj.RepliedTo = curmsg.Content;
				}
				else
				{
					newObj.RepliedTo = null;
				}
				returnList.Add(newObj);
			}
			context.SaveChanges();
			return returnList;
		}

		public IEnumerable<RecentChatModel> GetRecentUsers(string userName)
		{
			var curUserId = GetIdFromUserName(userName);
			// Get Id of Users The Logged In person Talked To
			var userTalkedWith = context.Messages
				.Where(m => (m.MessageFrom == curUserId || m.MessageTo == curUserId))
				.Select(m => m.MessageFrom == curUserId ? m.MessageTo : m.MessageFrom)
				.Distinct();

			var recentChatList = new List<RecentChatModel>();
			var sortedMessages = context.Messages.OrderByDescending(m => m.CreatedAt);

			foreach (var userId in userTalkedWith)
			{
				Profile userTalked = context.Profiles.FirstOrDefault(profile => profile.Id == userId);
				var msg = sortedMessages.FirstOrDefault(m => m.MessageFrom == userId || m.MessageTo == userId);
				var seenCount = sortedMessages.Where(msg => (msg.MessageFrom == userId && msg.MessageTo == curUserId && msg.Seen == 0)).Count();
				var newObj = new RecentChatModel
				{
					Content = msg.Content,
					CreatedAt = (DateTime)msg.CreatedAt,
					FirstName = userTalked.FirstName,
					LastName = userTalked.LastName,
					ImagePath = userTalked.ImagePath,
					UserName = userTalked.UserName,
					Seen = seenCount,
					Type = msg.Type,
				};
				recentChatList.Add(newObj);
			}
			var orderedRecentChatList = recentChatList.OrderByDescending(m => m.CreatedAt);
			return orderedRecentChatList;
		}
		public void MarkAsRead(string userName, string selUserName)
		{
			List<Message> msgs = null;
			var CurUserId = GetIdFromUserName(userName);
			if (selUserName == "All")
			{
				msgs = context.Messages.Where(m => m.MessageTo == CurUserId).ToList();
			}
			else
			{
				var SelUserId = GetIdFromUserName(selUserName);
				msgs = context.Messages.Where(m => m.MessageFrom == SelUserId && m.MessageTo == CurUserId).ToList();
			}
			foreach (var msg in msgs)
			{
				msg.Seen = 1;
			}
			context.SaveChanges();
		}

		public MessageSendModel SendFileMessage(MessageModel msg)
		{
			var message = new Message();
			var filename = Guid.NewGuid().ToString(); // new generated image file name
			var extension = Path.GetExtension(msg.File.FileName);// Get Extension Of the File
			var filetype = msg.File.ContentType.Split('/')[0];
			string uploads;
			if (filetype == "audio")
			{
				uploads = Path.Combine(webHostEnvironment.WebRootPath, @"chat/audio");
				message.Content = "/chat/audio/" + filename + extension;
				message.Type = "audio";
			}
			else if (filetype == "video")
			{
				uploads = Path.Combine(webHostEnvironment.WebRootPath, @"chat/videos");
				message.Content = "/chat/videos/" + filename + extension;
				message.Type = "video";
			}
			else
			{
				uploads = Path.Combine(webHostEnvironment.WebRootPath, @"chat/images");
				message.Content = "/chat/images/" + filename + extension;
				message.Type = "image";
			}

			using (var fileStreams = new FileStream(Path.Combine(uploads, filename + extension), FileMode.Create))
			{
				msg.File.CopyTo(fileStreams);
			}			

			message.MessageTo = GetIdFromUserName(msg.MessageTo);
			message.MessageFrom = GetIdFromUserName(msg.MessageFrom);
			message.RepliedTo = -1;
			message.Seen = 0;
			message.CreatedAt = DateTime.Now;

			context.Messages.Add(message);
			context.SaveChanges();

			var response = new MessageSendModel()
			{
				Id = message.Id,
				Content = message.Content,
				CreatedAt = message.CreatedAt,
				MessageFrom = msg.MessageFrom,
				MessageTo = msg.MessageTo,
				RepliedTo = null,
				Seen = 0,
				Type= message.Type,
			};
			return response;
			
		}
	}
}
