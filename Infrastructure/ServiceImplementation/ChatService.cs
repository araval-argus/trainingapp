using AutoMapper;
using ChatApp.Business.Helpers;
using ChatApp.Business.ServiceInterfaces;
using ChatApp.Context;
using ChatApp.Context.EntityClasses;
using ChatApp.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
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
			Context.EntityClasses.Profile user = context.Profiles.FirstOrDefault(profile => profile.UserName == username);
			return user.Id;
		}

		public MessageSendModel DoMessage(MessageModel message)
		{
			Message newMessage = null;
			MessageSendModel response = null;
			newMessage = new Message
			{
				Content = message.Content,
				CreatedAt = DateTime.Now,
				MessageFrom = GetIdFromUserName(message.MessageFrom),
				MessageTo = GetIdFromUserName(message.MessageTo),
			};
			context.Messages.Add(newMessage);
			context.SaveChanges();
			response = new MessageSendModel
			{
				Id = newMessage.Id,
				Content = newMessage.Content,
				CreatedAt = newMessage.CreatedAt,
				MessageFrom = message.MessageFrom,
				MessageTo = message.MessageTo
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
					MessageTo = (msg.MessageTo == userId) ? username : selUserName
				};
				returnList.Add(newObj);
			}
			return returnList;
		}
	}
}
