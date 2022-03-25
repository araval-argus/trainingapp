using ChatApp.Business.Helpers;
using ChatApp.Business.ServiceInterfaces;
using ChatApp.Context;
using ChatApp.Context.EntityClasses;
using ChatApp.Models;
using ChatApp.Models.Chat;
using Microsoft.EntityFrameworkCore;
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

        public IEnumerable<ChatModel> ChatLists(int userFrom, int userTo, int limit = 50)
        {
            var chats = context.Chats.Where(u => (u.MessageFrom == userFrom && u.MessageTo == userTo) || (u.MessageFrom == userTo && u.MessageTo == userFrom)).ToList();

            var returnChat = chats;

            string userFromUserName = userService.GetUserNameFromUserId(userFrom);
            string userToUserName = userService.GetUserNameFromUserId(userTo);

            IEnumerable<ChatModel> returnObj = ConvertChatToChatModel(returnChat, userFromUserName, userToUserName, userFrom, userTo);
            return returnObj;
        }

        public IEnumerable<RecentChatUsers> RecentChatUsers(int userId)
        {
            var userObj = userService.GetUserFromUserId(userId);

            var recentChatUsers = context.Chats.Where(u => u.MessageFrom == userId || u.MessageTo == userId)
                                        .Select(p => p.MessageFrom == userId ? p.MessageTo : p.MessageFrom)
                                            .Distinct();

            List<RecentChatUsers> recentChatUser = new();

            foreach (var user in recentChatUsers)
            {
                UserModel friendProfile = userService.GetUserFromUserId(user);

                int unreadCount = context.Chats
                    .Count(
                        u => (
                        ((u.MessageFrom == friendProfile.Id && u.MessageTo == userId))
                        && u.IsSeen == 0)
                    );

                var lastMsgObj = context.Chats.OrderBy(o => o.CreatedAt).LastOrDefault(
                    u => ((u.MessageFrom == userId && u.MessageTo == friendProfile.Id)
                        || (u.MessageFrom == friendProfile.Id && u.MessageTo == userId))
                    );

                string lastMsg = "";

                if (lastMsgObj != null)
                {
                    lastMsg = lastMsgObj.Content;
                }

                var userListObj = new RecentChatUsers();
                userListObj.User = friendProfile;
                userListObj.UnreadCount = unreadCount;
                userListObj.LastMessage = lastMsg;


                recentChatUser.Add(userListObj);
            }

            return recentChatUser;
        }

        public ChatModel SendTextMessage(string userFrom, string userTo, string content, int replyTo)
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
            chat.UpdatedAt = DateTime.Now;
            chat.ReplyTo = replyTo;


            var sendMessage = context.Chats.Add(chat);
            context.SaveChanges();

            string replyToMsg = "";

            if ( replyTo != 0)
            {
                replyToMsg = GetChatMsgById(replyTo);
            }

            var retrunObj = new ChatModel
            {
                Id = sendMessage.Entity.Id,
                MessageFrom = userFrom,
                MessageTo = userTo,
                Type = "text",
                Content = content,
                CreatedAt = sendMessage.Entity.CreatedAt,
                ReplyTo = replyToMsg
            };

            return retrunObj;

        }

        public void MarkConversationAsRead(int userId, int friendId)
        {
            var p = context.Chats
                        .Where(
                            u => (
                                ((u.MessageFrom == userId && u.MessageTo == friendId) || (u.MessageFrom == friendId && u.MessageTo == userId))
                                && u.IsSeen == 0
                            )
                        ).ToList();
            p.ForEach(c => c.IsSeen = 1);

            context.SaveChanges();
        }


        private IEnumerable<ChatModel> ConvertChatToChatModel(List<Chat> chat, string userFrom, string userTo, int userFromId, int userToId) 
        {
            var returnObj = new List<ChatModel>();

            for ( int i=0; i< chat.Count; i++)
            {
                string replyMsg = "";

                if (chat[i].ReplyTo != 0)
                {
                    replyMsg = GetChatMsgById(chat[i].ReplyTo);
                }

                var chatobj = new ChatModel
                {
                    Id = chat[i].Id,
                    MessageFrom = (chat[i].MessageFrom == userFromId) ? userFrom : userTo,
                    MessageTo = (chat[i].MessageTo == userFromId) ? userFrom : userTo,
                    Type = "text",
                    Content = chat[i].Content,
                    ReplyTo = replyMsg,
                    CreatedAt = chat[i].CreatedAt,
                    UpdatedAt = chat[i].UpdatedAt,
                    DeletedAt = chat[i].DeletedAt
                };

                returnObj.Add(chatobj);
            }

            return returnObj;
        }
        
        private string GetChatMsgById(int id)
        {
            return context.Chats.FirstOrDefault(c => c.Id == id).Content;
        }

    }
}
