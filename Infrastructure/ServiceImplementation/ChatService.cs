using ChatApp.Business.Helpers;
using ChatApp.Business.ServiceInterfaces;
using ChatApp.Context;
using ChatApp.Context.EntityClasses;
using ChatApp.Models;
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

        public IEnumerable<FriendProfileModel> FetchFriendsProfiles(string searchTerm)
        {
            return context.Profiles
                .Where(profile => profile.FirstName.ToLower().StartsWith(searchTerm))
                .Select(profile => new FriendProfileModel()
                {
                    FirstName = profile.FirstName,
                    LastName = profile.LastName,
                    Email = profile.Email,
                    UserName = profile.UserName,
                    Designation = Business.Helpers.Designation.getDesignationType(profile.DesignationID),
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
                MessageType = messageModel.MessageType
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
               profile => profile.UserName.Equals(userName)).Id;
        }
    }
}
