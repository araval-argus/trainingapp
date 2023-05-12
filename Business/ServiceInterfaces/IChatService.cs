using ChatApp.Models.Chat;
using ChatApp.Models.User;
using System.Collections.Generic;

namespace ChatApp.Business.ServiceInterfaces
{
    public interface IChatService
	{
		IEnumerable<SelectedUserModel> SearchColleague(string name,string username);
		string GetUsername(string authorization);
		int GetIdFromUserName(string username);
		 IEnumerable<MessageDTO> GetMsg(string username, string selUserName);
		IEnumerable<RecentChatModel> GetRecentUsers(string username);
		public void MarkAsRead(string username, string selusername);
		public void SendFileMessage(MessageModel msg);
	}
}
