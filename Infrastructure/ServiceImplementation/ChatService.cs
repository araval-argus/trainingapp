using ChatApp.Business.Helpers;
using ChatApp.Business.ServiceInterfaces;
using ChatApp.Context;
using ChatApp.Context.EntityClasses;
using ChatApp.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ChatApp.Infrastructure.ServiceImplementation
{
    public class ChatService : IChatService
    {
        private readonly ChatAppContext context;

        public ChatService(ChatAppContext context)
        {
            this.context = context;
        }

        public IEnumerable<UserModel> FetchFriendsProfiles(string searchTerm)
        {
            return context.Profiles.Include("Designation")
                .Where(profile => profile.IsActive && profile.FirstName.ToLower().StartsWith(searchTerm))
                .Select(profile => new UserModel()
                {
                    FirstName = profile.FirstName,
                    LastName = profile.LastName,
                    Email = profile.Email,
                    UserName = profile.UserName,
                    Designation = profile.Designation,
                    ImageUrl = profile.ImageUrl,
                }); ;
        }

        public MessageEntity AddMessage(MessageModel messageModel)
        {
            MessageEntity messageEntity = new()
            {
                Message = messageModel.Message,
                SenderID = FetchProfileIdFromUserName(messageModel.SenderUserName),
                RecieverID = FetchProfileIdFromUserName(messageModel.RecieverUserName),
                MessageType = messageModel.MessageType,
                RepliedToMsg = Convert.ToInt32(messageModel.RepliedToMsg),
                CreatedAt = messageModel.CreatedAt
            };

            this.context.Messages.Add(messageEntity);
            this.context.SaveChanges();

            return messageEntity;
        }

        public IEnumerable<MessageEntity> FetchMessages(int senderID, int recieverID)
        {
            return this.context.Messages.Where(
                message => (message.SenderID == senderID && message.RecieverID == recieverID)
                || (message.SenderID == recieverID && message.RecieverID == senderID)
                );
        }

        public string FetchMessageFromId(int id)
        {
            string result = "";
            if (this.context.Messages.Any(m => m.Id == id))
            {
                result = this.context.Messages.FirstOrDefault(m => m.Id == id).Message;
            }
            return result;
        }


        public void MarkMsgsAsSeen(IEnumerable<MessageEntity> messages)
        {
            this.context.Messages.UpdateRange(messages);
            this.context.SaveChanges();
        }


        int FetchProfileIdFromUserName(string userName)
        {
            return this.context.Profiles.FirstOrDefault(
               profile => profile.IsActive && profile.UserName.Equals(userName)).Id;
        }

        public void MarkMsgAsSeen(int messageId)
        {
            var message = this.context.Messages.FirstOrDefault(message => message.Id == messageId);
            if(message != null)
            {
                message.IsSeen = true;
                this.context.Messages.Update(message);
                this.context.SaveChanges();
            }
        }
    }
}
