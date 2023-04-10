using ChatApp.Business.ServiceInterfaces;
using ChatApp.Context;
using ChatApp.Context.EntityClasses;
using ChatApp.Models;
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
                    Designation = GetDesignation(profile.Designation),
                    imageUrl = profile.ImageUrl,
                });
        }

        public MessageEntity AddMessage(MessageModel messageModel)
        {
            MessageEntity messageEntity = new()
            {
                Message = messageModel.Message,
                SenderID = messageModel.SenderID,
                RecieverID = messageModel.RecieverID
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

        public static string GetDesignation(int designation)
        {
            switch (designation)
            {
                case 2:
                    return "Programmer Analyst";
                case 3:
                    return "Solution Analyst";
                case 4:
                    return "Lead Solution Analyst";
                case 5:
                    return "Intern";
                case 6:
                    return "Probationer";
                case 7:
                    return "Quality Analyst";
                default:
                    return "Imposter";
            }
        }
    }
}
