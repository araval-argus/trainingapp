using ChatApp.Business.Helpers;
using ChatApp.Business.ServiceInterfaces;
using ChatApp.Context;
using ChatApp.Context.EntityClasses;
using ChatApp.Models;
using ChatApp.Models.Chat;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChatApp.Infrastructure.ServiceImplementation
{
    public class ChatService: IChatService
    {
        private readonly ChatAppContext context;
        private readonly IUserService userService;

        public ChatService(ChatAppContext context, IUserService userService)
        {
            this.context = context;
            this.userService = userService;
        }

        public IEnumerable<ChatModel> chatLists(int userFrom, int userTo, int limit = 50)
        {
            var chats = context.Chats.Where(u => (u.MessageFrom == userFrom && u.MessageTo == userTo) || (u.MessageFrom == userTo && u.MessageTo == userFrom)).ToList();

            var returnChat = chats;

            string userFromUserName = userService.GetUserNameFromUserId(userFrom);
            string userToUserName = userService.GetUserNameFromUserId(userTo);

            IEnumerable<ChatModel> returnObj = (IEnumerable<ChatModel>)convertChatToChatModel(returnChat, userFromUserName, userToUserName, userFrom, userTo);
            return returnObj;
        }

        public IEnumerable<UserModel> recentChatUsers(int userId)
        {
            var users = context.Chats.Where(u => u.MessageFrom == userId || u.MessageTo == userId)
                                        .Select(p => p.MessageFrom == userId ? p.MessageTo : p.MessageFrom)
                                            .Distinct();

            List<UserModel> recentChatUser = new();

            foreach (var user in users)
            {
                UserModel profile = userService.GetUserFromUserId(user);
                recentChatUser.Add(profile);
            }

            return recentChatUser;
        }




        // new msg
        public ChatModel sendTextMessage(string userFrom, string userTo, string content)
        {
            int userFromId = userService.GetUserIdFromUserName(userFrom);
            int userToId = userService.GetUserIdFromUserName(userTo);

            Chat chat = new();

            // setting chat object
            chat.MessageFrom = userFromId;
            chat.MessageTo = userToId;
            chat.Type = "text";
            chat.Content = content;
            chat.CreatedAt = DateTime.Now;


            var sendMessage = context.Chats.Add(chat);
            context.SaveChanges();

            var retrunObj = new ChatModel
            {
                Id = sendMessage.Entity.Id,
                MessageFrom = userFrom,
                MessageTo = userTo,
                Type = "text",
                Content = content,
                CreatedAt = sendMessage.Entity.CreatedAt

            };

            return retrunObj;

        }

        private IEnumerable<ChatModel> convertChatToChatModel(List<Chat> chat, string userFrom, string userTo, int userFromId, int userToId) 
        {
            var returnObj = new List<ChatModel>();

            for ( int i=0; i< chat.Count; i++)
            {
                var chatobj = new ChatModel
                {
                    Id = chat[i].Id,
                    MessageFrom = (chat[i].MessageFrom == userFromId)? userFrom : userTo,
                    MessageTo = (chat[i].MessageTo == userFromId)? userFrom : userTo,
                    Type = "text",
                    Content = chat[i].Content,
                    CreatedAt = chat[i].CreatedAt,
                    UpdatedAt = chat[i].UpdatedAt,
                    DeletedAt = chat[i].DeletedAt
                };

                returnObj.Add(chatobj);
            }

            return returnObj;
        }
        
    }
}
