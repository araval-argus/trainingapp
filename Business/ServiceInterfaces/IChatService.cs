using ChatApp.Context.EntityClasses;
using ChatApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ChatApp.Business.ServiceInterfaces
{
	public interface IChatService
	{
		IEnumerable<ColleagueModel> SearchColleague(string name,string username);
		string GetUsername(string authorization);
		int GetIdFromUserName(string username);
		 IEnumerable<MessageSendModel> GetMsg(string username, string selUserName);
		IEnumerable<RecentChatModel> GetRecentUsers(string username);
		public void MarkAsRead(string username, string selusername);
		public void SendFileMessage(MessageModel msg);
	}
}
