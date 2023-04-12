using ChatApp.Context.EntityClasses;
using ChatApp.Models;
using System;
using System.Collections.Generic;

namespace ChatApp.Business.ServiceInterfaces
{
	public interface IChatService
	{
		IEnumerable<ColleagueModel> SearchColleague(string name,string username);
		string GetUsername(string authorization);
		MessageSendModel DoMessage(MessageModel message);
		int GetIdFromUserName(string username);
		public IEnumerable<MessageSendModel> GetMsg(string username, string selUserName);
	}
}
