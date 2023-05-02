using ChatApp.Context.EntityClasses;
using ChatApp.Models;
using System.Collections.Generic;

namespace ChatApp.Business.ServiceInterfaces
{
	public interface INotificationServices
	{
		public IEnumerable<NotificationsDTO> GetAllNotifications(string userName);

		public void addToGroup(List<int> selUserIds,int userId,int groupId);
		public void adminNoti(int userId, int loginUserId, int groupId);
		public void removeNoti(int selUserId, int userId,int groupId);
		public void groupMsgNoti(int loginUserId, int groupId, string? msgtype);
		public void Clear(string userName);
	}
}
