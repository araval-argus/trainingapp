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
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net;
using System.Reflection.Metadata;

namespace ChatApp.Infrastructure.ServiceImplementation
{
	public class ChatService : IChatService
	{

		private readonly ChatAppContext context;
		public ChatService(ChatAppContext context)
		{
			this.context = context;
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

		public int GetIdFromUserName(string username)
		{
			Profile user = context.Profiles.FirstOrDefault(profile => profile.UserName == username);
			return user.Id;
		}

		public MessageSendModel DoMessage(MessageModel message)
		{
			Message newMessage = null;
			MessageSendModel response = null;
			string ReplyMessage;
			newMessage = new Message
			{
				Content = message.Content,
				CreatedAt = DateTime.Now,
				MessageFrom = GetIdFromUserName(message.MessageFrom),
				MessageTo = GetIdFromUserName(message.MessageTo),
				RepliedTo = message.RepliedTo,
				Seen = 0
			};
			context.Messages.Add(newMessage);
			context.SaveChanges();
			if (message.RepliedTo == -1)
			{
				ReplyMessage = null;
			}
			else
			{
				var msg = context.Messages.FirstOrDefault(msg => msg.Id == message.RepliedTo);
				ReplyMessage = msg.Content;
			}
			response = new MessageSendModel
			{
				Id = newMessage.Id,
				Content = newMessage.Content,
				CreatedAt = newMessage.CreatedAt,
				MessageFrom = message.MessageFrom,
				MessageTo = message.MessageTo,
				RepliedTo = ReplyMessage,
				Seen = 0,
			};
			return response;
		}

		public IEnumerable<MessageSendModel> GetMsg(string username, string selUserName)
		{
			int userId = GetIdFromUserName(username);
			int selUserId = GetIdFromUserName(selUserName);
			var list =  context.Messages
			.Where(msg => (msg.MessageFrom == userId && msg.MessageTo == selUserId )||( msg.MessageFrom == selUserId && msg.MessageTo == userId));
			var returnList = new List<MessageSendModel>();

			foreach (var msg in list )
			{
				var newObj = new MessageSendModel
				{
					Id = msg.Id,
					Content = msg.Content,
					CreatedAt = msg.CreatedAt,
					MessageFrom = (msg.MessageFrom == userId) ? username : selUserName,
					MessageTo = (msg.MessageTo == userId) ? username : selUserName,
					Seen = msg.Seen
				};

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
			return returnList;
		}

		public IEnumerable<RecentChatModel> GetRecentUsers(string username)
		{
			var CurUserId = GetIdFromUserName(username);
			// Get Id of Users The Logged In person Talked To
			var UserTalkedWith = context.Messages
				.Where(m => (m.MessageFrom == CurUserId || m.MessageTo == CurUserId))
				.Select(m => m.MessageFrom == CurUserId ? m.MessageTo : m.MessageFrom)
				.Distinct();

			var RecentChatList = new List<RecentChatModel>();
			var SortedMessages = context.Messages.OrderByDescending(m => m.CreatedAt);

			foreach (var userId in UserTalkedWith)
			{
				Profile userTalked = context.Profiles.FirstOrDefault(profile => profile.Id == userId);
				var msg = SortedMessages.FirstOrDefault(m => m.MessageFrom == userId || m.MessageTo == userId);
				var seenCount = SortedMessages.Where(msg => (msg.MessageFrom == userId && msg.MessageTo == CurUserId && msg.Seen == 0)).Count();
				var newObj = new RecentChatModel
				{
					Content = msg.Content,
					CreatedAt = (DateTime)msg.CreatedAt,
					FirstName = userTalked.FirstName,
					LastName = userTalked.LastName,
					ImagePath = userTalked.ImagePath,
					UserName = userTalked.UserName,
					Seen = seenCount,
				};
				RecentChatList.Add(newObj);
			}
			var OrderedRecentChatList = RecentChatList.OrderByDescending(m => m.CreatedAt);
			return OrderedRecentChatList;
		}
		public void MarkAsRead(string username , string selusername)
		{
			List<Message> msgs = null;
			var CurUserId = GetIdFromUserName(username);
			if (selusername == "All")
			{
				msgs = context.Messages.Where(m => m.MessageTo == CurUserId).ToList();
			}
			else
			{
				var SelUserId = GetIdFromUserName(selusername);
				msgs = context.Messages.Where(m => m.MessageFrom==SelUserId && m.MessageTo==CurUserId).ToList();
			}
			foreach (var msg in msgs)
			{
				msg.Seen = 1;
			}
			context.SaveChanges();
		}
	}
}
