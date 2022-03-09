using ChatApp.Business.ServiceInterfaces;
using ChatApp.Context;
using ChatApp.Context.EntityClasses;
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

        public ChatService(ChatAppContext context)
        {
        this.context = context;
        }

        public IEnumerable<Chat> chatLists(int userFrom, int userTo, int limit = 50)
        {
            var chats = context.Chats.Where(u => (u.MessageFrom == userFrom || u.MessageTo == userFrom) && (u.MessageFrom == userTo || u.MessageTo == userTo)).ToList();

            var returnChat = (IEnumerable<Chat>)chats;
            return returnChat;
        }


        // new msg

    }
}
