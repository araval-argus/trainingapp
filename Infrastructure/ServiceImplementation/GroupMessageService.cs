using ChatApp.Business.ServiceInterfaces;
using ChatApp.Context;
using ChatApp.Context.EntityClasses;
using ChatApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ChatApp.Infrastructure.ServiceImplementation
{
    public class GroupMessageService : IGroupMessageService
    {
        private ChatAppContext context;

        public GroupMessageService(ChatAppContext context)
        {
            this.context = context;
        }

        public IList<GroupMessageModel> FetchGroupMessages(int groupId)
        {
            IList<GroupMessage> messages = this.context.GroupMessages.Where( message => message.GroupID == groupId ).ToList();

            IList<GroupMessageModel> groupMessagesAsModels = new List<GroupMessageModel>(); 
            
            foreach(var groupMessage in messages)
            {
                GroupMessageModel groupMessageModel = new()
                {
                    Id = groupMessage.Id,
                    GroupId = groupId,
                    Message = groupMessage.Message,
                    MessageType = groupMessage.MessageType,
                    CreatedAt = groupMessage.CreatedAt,
                };
                var sender = this.context.Profiles.FirstOrDefault(p => p.IsActive && p.Id == groupMessage.SenderID);

                if(sender != null)
                {
                    groupMessageModel.SenderUserName = sender.UserName;
                }

                if(groupMessage.RepliedToMsg >= 0)
                {
                    groupMessageModel.RepliedToMsg = this.context.GroupMessages.FirstOrDefault(message => message.Id == groupMessage.RepliedToMsg).Message;
                }

                groupMessagesAsModels.Add(groupMessageModel);
            }

            return groupMessagesAsModels;
        }

        public GroupMessage AddGroupMessage(GroupMessageModel groupMessageModel)
        {
            GroupMessage groupMessage = new()
            {
                Message = groupMessageModel.Message,
                MessageType = groupMessageModel.MessageType,
                CreatedAt = groupMessageModel.CreatedAt,
                GroupID = groupMessageModel.GroupId,
                RepliedToMsg = Convert.ToInt32(groupMessageModel.RepliedToMsg),
                SenderID = this.context.Profiles.FirstOrDefault(p => p.UserName == groupMessageModel.SenderUserName).Id
            };
            this.context.GroupMessages.Add(groupMessage);
            this.context.SaveChanges();
            return groupMessage;
        }

        public GroupMessage FetchGroupMessageFromId(int groupMessageId)
        {
            return this.context.GroupMessages.FirstOrDefault(message => message.Id == groupMessageId);
        }

        public void DeleteAllGroupMessages(int groupId)
        {
            var messages = this.context.GroupMessages.Where(message => message.GroupID == groupId);
            if(messages.Count() > 0)
            {
                this.context.GroupMessages.RemoveRange(messages);
            }
            this.context.SaveChanges();
        }

        public void RemoveAllGroupMessagesSentByMember(int memberId, int groupId)
        {
            var messages = this.context.GroupMessages.Where(messages => messages.GroupID == groupId && messages.SenderID == memberId);
            if(messages.Count() > 0 )
            {
                this.context.GroupMessages.RemoveRange(messages);
            }
            this.context.SaveChanges();
        }

        public GroupMessage FetchLastMessage(int groupId)
        {
            return this.context.GroupMessages.OrderBy(message => message.Id).LastOrDefault(message => message.GroupID == groupId);
        }

        public IList<GroupMessage> FetchAllGroupMessages(int userId)
        {
            IList<GroupMessage> groupMessages = new List<GroupMessage>();

            IEnumerable<GroupMember> groupMembers = this.context.GroupMembers.Where(member => member.ProfileID == userId);

            foreach(GroupMember member in groupMembers)
            {
                int groupIdOfGroupMember = member.GroupID;
                var messages = this.context.GroupMessages.Where(message => message.GroupID == groupIdOfGroupMember);
                groupMessages = groupMessages.Concat(messages).ToList();

            }

            return groupMessages;
        }
    }
}
