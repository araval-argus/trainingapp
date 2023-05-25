using ChatApp.Business.ServiceInterfaces;
using ChatApp.Context;
using ChatApp.Context.EntityClasses;
using ChatApp.Models;
using Microsoft.AspNetCore.Hosting;
using System.IO;
using System;
using System.Linq;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using ChatApp.Business.Helpers;

namespace ChatApp.Infrastructure.ServiceImplementation
{
    public class GroupService: IGroupService
    {
        private ChatAppContext context;
        private IWebHostEnvironment webHostEnvironment;
        public GroupService(ChatAppContext context, IWebHostEnvironment webHostEnvironment)
        {
            this.context = context;
            this.webHostEnvironment = webHostEnvironment;
        }
        public GroupModel CreateGroup(GroupModel groupModel, string creatorId)
        {
            Group newGroup = new()
            {
                Name = groupModel.Name,
                Description = groupModel.Description,
                CreatedBy = creatorId,
                LastUpdatedBy = creatorId,
                CreatedAt = DateTime.UtcNow,
                LastUpdatedAt = DateTime.UtcNow,
                IsActive = true
            };
            if (groupModel.GroupIcon != null)
            {
                string path = Path.Combine(webHostEnvironment.WebRootPath, "GroupIcons");
                newGroup.GroupIcon = FileManagement.CreateUniqueFile(path, groupModel.GroupIcon);
            }
            
            var group1 = this.context.Groups.Add(newGroup);
            this.context.SaveChanges();
            newGroup = group1.Entity;
            groupModel = EntityToModel.ConvertToGroupModel(newGroup);
            groupModel.LoggedInUserIsAdmin = true;
            return groupModel;
        }

        public IEnumerable<GroupModel> FetchGroups(string userId)
        {
            IList<GroupModel> Groups = new List<GroupModel>();
            if(String.IsNullOrEmpty(userId))
            {
                return Groups;
            }
            IEnumerable<int> GroupIDs = this.context.GroupMembers.Where( member => member.ProfileID == userId).Select( member => member.GroupID);
            foreach(int groupID in GroupIDs)
            {
                var group = this.context.Groups.Include("Creator").FirstOrDefault(group => group.Id == groupID);
                if (group != null)
                {
                    var groupModel = EntityToModel.ConvertToGroupModel(group);
                    groupModel.LoggedInUserIsAdmin = this.context.GroupMembers.FirstOrDefault(m => m.ProfileID == userId && m.GroupID == group.Id).IsAdmin;
                    var lastMessage = this.context.GroupMessages.OrderBy(message => message.Id).LastOrDefault(message => message.GroupID == groupModel.Id);
                    if(lastMessage != null)
                    {
                        groupModel.LastMessage = lastMessage.Message;
                        groupModel.LastMessageTimeStamp = lastMessage.CreatedAt;
                    }
                    else
                    {
                        groupModel.LastMessageTimeStamp = group.LastUpdatedAt;
                    }
                    Groups.Add(groupModel);
                }
            }
            Groups = Groups.OrderByDescending(group => group.LastMessageTimeStamp).ToList();
            return Groups;
        }

        public Group FetchGroupFromId(int groupId)
        {
            if(groupId == null)
            {
                return null;
            }
            Group group = this.context.Groups.Include("Creator").Include("LastModifier").FirstOrDefault(g => g.Id == groupId);
            return group;
        }

        public void DeleteGroup(int groupId)
        {
            this.context.Groups.Remove(FetchGroupFromId(groupId));
            this.context.SaveChanges();
        }


        public void UpdateGroup(GroupModel groupModel, Profile user)
        {
            var group = this.context.Groups.FirstOrDefault(g => g.Id == groupModel.Id);
            group.Name = groupModel.Name;
            group.Description = groupModel.Description;
            group.LastUpdatedAt = DateTime.Now;
            group.LastUpdatedBy = user.Id;

            if (groupModel.GroupIcon != null)
            {
                string path = Path.Combine(webHostEnvironment.WebRootPath, "GroupIcons");
                group.GroupIcon = FileManagement.CreateUniqueFile(path, groupModel.GroupIcon);
            }

            this.context.SaveChanges();
        }

    }
}
