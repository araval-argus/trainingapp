using ChatApp.Context.EntityClasses;
using ChatApp.Models;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace ChatApp.Business.ServiceInterfaces
{
    public interface IGroupMemberService
    {
        void AddGroupMember(string userId, int groupId, bool isAdmin);
        bool IsAdmin(string userId, int groupId);
        void MakeAdmin(GroupMember member);
        bool IsMember(string userId, int groupId);
        GroupMember RemoveMember(string memberId, int groupId);
        bool IsThereAnyAdminLeftInTheGroup(int groupId);
        IList<GroupMember> ListOfJoinedMembers(int groupID);
        IList<GroupMemberModel> ListOfJoinedGroupMemberModels(int groupId);
        IList<GroupMemberModel> ListOfNotJoinedMembers(int groupID);
        GroupMember FetchMember(string profileId, int groupId);
    }
}
