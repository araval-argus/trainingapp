using ChatApp.Business.Helpers;
using ChatApp.Business.ServiceInterfaces;
using ChatApp.Context;
using ChatApp.Context.EntityClasses;
using ChatApp.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace ChatApp.Infrastructure.ServiceImplementation
{
    public class GroupMemberService : IGroupMemberService
    {
        private ChatAppContext context;

        public GroupMemberService(ChatAppContext context)
        {
            this.context = context;
        }
        public void AddGroupMember(string userId, int groupId, bool isAdmin)
        {
            GroupMember member = new()
            {
                ProfileID = userId,
                GroupID = groupId,
                IsAdmin = isAdmin
            };
            
            this.context.GroupMembers.Add(member);
            this.context.SaveChanges();
        }

        public IList<GroupMemberModel> ListOfNotJoinedMembers(int groupID)
        {
            if (groupID <= 0)
            {
                return null;
            }

            var joinedUserIds = this.context.GroupMembers
                .Where(m => m.GroupID == groupID)
                .Distinct()
                .Select(m => m.ProfileID);
            List<GroupMemberModel> notJoinedUsers = new List<GroupMemberModel>();

            var users = this.context.Profiles.Include("Designation").Where(p => p.IsActive && !joinedUserIds.Contains(p.Id));
            foreach (var user in users)
            {
                notJoinedUsers.Add(new GroupMemberModel
                {
                    groupId = groupID,
                    UserName = user.UserName,
                    FirstName = user.FirstName,
                    LastName = user.LastName,   
                    ImageUrl = user.ImageUrl,
                    IsAdmin = false
                });
            }
            

            return notJoinedUsers.Distinct().ToList();
        }


        public bool IsAdmin(string userId, int groupId)
        {
            return this.context.GroupMembers.Any(m => m.ProfileID == userId && m.GroupID == groupId && m.IsAdmin);
        }

        public GroupMember RemoveMember(string memberId, int groupId)
        {
            GroupMember member = this.context.GroupMembers.FirstOrDefault(m => m.ProfileID == memberId && m.GroupID == groupId);
            if(member == null)
            {
                return null;
            }
            this.context.GroupMembers.Remove(member);
            this.context.SaveChanges();
            return member;
        }

        public bool IsThereAnyAdminLeftInTheGroup(int groupId)
        {
            return this.context.GroupMembers.Any(m => m.GroupID == groupId && m.IsAdmin);
        }

        public IList<GroupMember> ListOfJoinedMembers(int groupID)
        {

            IList<GroupMember> listOfJoinedMembers = new List<GroupMember>();
            listOfJoinedMembers = this.context.GroupMembers.Where(m => m.GroupID == groupID).OrderBy(m => m.JoinedAt).ToList();
            return listOfJoinedMembers;
        }


        public void MakeAdmin(GroupMember member)
        {
            member.IsAdmin = true;
            this.context.GroupMembers.Update(member);
            this.context.SaveChanges();
        }

        public IList<GroupMemberModel> ListOfJoinedGroupMemberModels(int groupId)
        {
            var joinedUsers = this.context.GroupMembers.Include("Member")
               .Where(m => m.GroupID == groupId)
               .Distinct()
               .Select(m => new GroupMemberModel
               {
                   groupId = m.GroupID,
                   UserName = m.Member.UserName,
                   FirstName= m.Member.FirstName,
                   LastName= m.Member.LastName,
                   ImageUrl= m.Member.ImageUrl,
                   IsAdmin= m.IsAdmin
               });
            
            return joinedUsers.ToList();
        }

        public bool IsMember(string userId, int groupId)
        {
            return this.context.GroupMembers.Any(member => member.ProfileID == userId && member.GroupID == groupId);
        }

        public GroupMember FetchMember(string profileId, int groupId)
        {
            return this.context.GroupMembers.FirstOrDefault(m => m.ProfileID == profileId && m.GroupID == groupId);
        }
    }
}
