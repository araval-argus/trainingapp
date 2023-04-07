using ChatApp.Business.Helpers;
using ChatApp.Business.ServiceInterfaces;
using ChatApp.Context;
using ChatApp.Context.EntityClasses;
using ChatApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ChatApp.Infrastructure.ServiceImplementation
{
    public class ChatService : IChatService
    {
        private readonly ChatAppContext context;
        public ChatService(ChatAppContext context) {
            this.context = context;
        }

        public bool AddChat(ChatModel chatModel, string userName)
        {
            if(chatModel == null)
            {
                return false;
            }
            Chat chat = new Chat();
            chat.From = userName;
            chat.To= chatModel.To;
            chat.content = chatModel.content;
            chat.sentAt = DateTime.Now;
            context.Chats.Add(chat);
            context.SaveChanges();
            return true;
        }

        public List<ChatDTO> GetAllChats(string from, string to)
        {
            List<Chat> sentChats = context.Chats.Where(e => e.From == from && e.To == to).ToList();
            List<Chat> received = context.Chats.Where(e => e.From == to && e.To == from).ToList();
            List<ChatDTO> chatDTOs = new List<ChatDTO>();
            if (sentChats.Count > 0 || sentChats.Count > 0) {
                chatDTOs = Mapper.chatMapper(sentChats, received);
            }
            return chatDTOs;
        }
    }
}
