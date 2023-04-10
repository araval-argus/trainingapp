using ChatApp.Business.Helpers;
using ChatApp.Business.ServiceInterfaces;
using ChatApp.Context;
using ChatApp.Context.EntityClasses;
using ChatApp.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace ChatApp.Infrastructure.ServiceImplementation
{
    public class ChatService : IChatService
    {
        private readonly ChatAppContext context;
        private readonly IProfileService _profileService;
        public ChatService(ChatAppContext context, IProfileService profileService)
        {
            this.context = context;
            _profileService = profileService;
        }

        public bool AddChat(ChatModel chatModel, string userName)
        {
            Profile profile = context.Profiles.FirstOrDefault(p => p.UserName == userName);
            Profile receiver = context.Profiles.FirstOrDefault(p => p.UserName == chatModel.To);
            if(chatModel == null)
            {
                return false;
            }
            Chat chat = new()
            {
                From = profile.Id,
                To = receiver.Id,
                content = chatModel.content,
                sentAt = DateTime.Now,
                replyToChat = chatModel.replyToChat
            };
            context.Chats.Add(chat);
            context.SaveChanges();
            return true;
        }

        public List<ChatDTO> GetAllChats(string userName, string to)
        {
            Profile profile = context.Profiles.FirstOrDefault(p => p.UserName == userName);
            Profile receiver = context.Profiles.FirstOrDefault(p => p.UserName == to);
            List<Chat> sentChats = context.Chats.Where(e => e.From == profile.Id && e.To == receiver.Id).ToList();
            List<Chat> receivedChats = context.Chats.Where(e => e.From == receiver.Id && e.To == profile.Id).ToList();
            List<ChatDTO> chatDTOs = new();
            if (sentChats.Count > 0 || receivedChats.Count > 0) {
                chatDTOs = Mapper.chatMapper(sentChats, receivedChats);
            }
            return chatDTOs;
        }

        public List<recentChatDTO> recent(string user)
        {
            var id = context.Profiles.FirstOrDefault(p => p.UserName == user).Id;

            //IEnumerable<Chat> abcc = context.Set<Chat>();

            IEnumerable<Chat> sentChats = context.Chats.Where(e => e.From == id || e.To == id ).ToList();

            IEnumerable<Chat> recentChats = sentChats.GroupBy(chat => chat.From == id ? chat.To : chat.From)
            .Select(grp => grp.OrderByDescending(msg => msg.sentAt).FirstOrDefault())
            .ToList();
            List<recentChatDTO> recentChatDTOs = new();
            foreach (Chat chat in recentChats)
            {
                Profile receiver = _profileService.getUser(chat.To == id ? chat.From : chat.To);
                recentChatDTOs.Add(new recentChatDTO
                {
                    to = Mapper.profileMapper(receiver),
                    chatContent = new chatFormat
                    {
                        id = chat.Id,
                        content = chat.content,
                        sentAt = chat.sentAt,
                        replyToChat = chat.replyToChat
                    }
                });
            }
            return recentChatDTOs;
        }
    }
}
