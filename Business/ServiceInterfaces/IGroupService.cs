using ChatApp.Context.EntityClasses;
using ChatApp.Models;
using System.Collections.Generic;

namespace ChatApp.Business.ServiceInterfaces
{
	public interface IGroupService
	{
		public RecentGroupModel CreateGroup(string userName, CreateGroupModel grp);
		public IEnumerable<RecentGroupModel> GetRecentGroups(string userName);
		public IEnumerable<GroupMemberList> getAllMembers(int groupId);
		public GroupLoadModel getGroup(int groupId, string username);
		public IEnumerable<AddMemberToGroupModel> getAllProfiles(int groupId, string userName);
		public IEnumerable<GroupMemberList> addMembersToGroup(int grpId, string[] selUsers, string userName);
		public GroupLoadModel updateGroup(string userName, CreateGroupModel grp, int grpId);
		public void leaveGroup(string userName, int groupId);
		public void makeAdmin(int groupId, string selUserName, string userName);
		public void removeUser(int groupId, string selUserName, string username);
		public void SendFileMessage(GMessageInModel msg);
		public IEnumerable<GMessageSendModel> GetAllMessage(int groupId);
	}
}
