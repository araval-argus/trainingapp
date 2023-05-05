using ChatApp.Context.EntityClasses;
using ChatApp.Models;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace ChatApp.Business.ServiceInterfaces
{
    public interface IGroupMemberService
    {
        void AddGroupMember(int userId, int groupId, bool isAdmin);
        bool IsAdmin(int userId, int groupId);
        void MakeAdmin(GroupMember member);
        bool IsMember(int userId, int groupId);
        GroupMember RemoveMember(int memberId, int groupId);
        bool IsThereAnyAdminLeftInTheGroup(int groupId);
        IList<GroupMember> ListOfJoinedMembers(int groupID);
        IList<GroupMemberModel> ListOfJoinedGroupMemberModels(int groupId);
        IList<GroupMemberModel> ListOfNotJoinedMembers(int groupID);
        GroupMember FetchMember(int profileId, int groupId);
    }
}
