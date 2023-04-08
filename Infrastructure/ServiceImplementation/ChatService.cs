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
            Chat chat = new()
            {
                From = userName,
                To = chatModel.To,
                content = chatModel.content,
                sentAt = DateTime.Now
            };
            context.Chats.Add(chat);
            context.SaveChanges();
            return true;
        }

        public List<ChatDTO> GetAllChats(string from, string to)
        {
            List<Chat> sentChats = context.Chats.Where(e => e.From == from && e.To == to).ToList();
            List<Chat> receivedChats = context.Chats.Where(e => e.From == to && e.To == from).ToList();
            List<ChatDTO> chatDTOs = new();
            if (sentChats.Count > 0 || receivedChats.Count > 0) {
                chatDTOs = Mapper.chatMapper(sentChats, receivedChats);
            }
            return chatDTOs;
        }

        public List<recentChatDTO> recent(string from)
        {
            //Get all the sent chat from user
            List<Chat> sentChats = context.Chats.Where(e => e.From == from || e.To == from ).ToList();
            return GetRecent(sentChats, from);
        }

        private static List<recentChatDTO> GetRecent(List<Chat> chats, string sender)
        {
            List<Chat> recentChats = chats.GroupBy(chat => chat.From == sender ? chat.To : chat.From)
                .Select(grp => grp.OrderByDescending(msg => msg.sentAt).FirstOrDefault())
                .ToList();
            List<recentChatDTO> recentChatDTOs = Mapper.recentChatMapper(recentChats, sender);
            return recentChatDTOs;
        }
    }
}
