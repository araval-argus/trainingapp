using ChatApp.Business.Helpers;
using ChatApp.Business.ServiceInterfaces;
using ChatApp.Context;
using ChatApp.Context.EntityClasses;
using ChatApp.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace ChatApp.Infrastructure.ServiceImplementation
{
    public class ChatService : IChatService
    {
        private readonly ChatAppContext context;
        private readonly IProfileService _profileService;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public ChatService(ChatAppContext context, IProfileService profileService, IWebHostEnvironment webHostEnvironment)
        {
            this.context = context;
            this._profileService = profileService;
            this._webHostEnvironment = webHostEnvironment;
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
            //This will update all the message to mark as read
            receivedChats.ForEach(e =>
            {
                e.isRead = 1;
            });
            context.Chats.UpdateRange(receivedChats);
            context.SaveChanges();
            return chatDTOs;
        }

        public List<recentChatDTO> recent(string user)
        {
            var id = context.Profiles.AsNoTracking().FirstOrDefault(p => p.UserName == user).Id;
            IEnumerable<Chat> sentChats = context.Chats.AsNoTracking().AsNoTracking().Where(e => e.From == id || e.To == id ).ToList();
            IEnumerable<IGrouping<int, Chat>> recentChatsGroups = sentChats.GroupBy(chat => chat.From == id ? chat.To : chat.From);
            IDictionary<int, int> unreadCountDictionary = new Dictionary<int, int>();
            foreach(var group in recentChatsGroups)
            {
                var unreadCount = 0;
                foreach(Chat chat in group.ToList()) {
                    //If message is coming and also that message is not read then increase the count
                    unreadCount += chat.isRead == 0 && chat.To == id ? 1 : 0;
                }
                unreadCountDictionary.Add(group.Key, unreadCount);
            }
            IEnumerable<Chat> recentChats = recentChatsGroups.Select(grp => grp.OrderByDescending(msg => msg.sentAt).FirstOrDefault())
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
                        replyToChat = chat.replyToChat,
                        isRead = chat.isRead
                    },
                    unreadCount = unreadCountDictionary[receiver.Id]
                });
            }
            return recentChatDTOs;
        }


        public bool addFile(string userName, ChatFileModel chatFile)
        {
            Profile sender = context.Profiles.AsNoTracking().FirstOrDefault(e => e.UserName == userName);
            Profile receiver = context.Profiles.AsNoTracking().FirstOrDefault(e => e.UserName == chatFile.to);
            if(sender != null && receiver != null) { 
                if(chatFile.file != null)
                {
                    var file = chatFile.file;
                    string rootPath = _webHostEnvironment.WebRootPath;
                    string fileName = Guid.NewGuid().ToString();
                    var extension = Path.GetExtension(file.FileName);
                    var pathToSave = Path.Combine(rootPath, @"files");
                    var fullFile = fileName + extension;
                    var dbPath = Path.Combine(pathToSave, fullFile);
                    using (var fileStreams = new FileStream(dbPath, FileMode.Create))
                    {
                        file.CopyTo(fileStreams);
                    }
                    Chat chat = new()
                    {
                        From = sender.Id,
                        To = receiver.Id,
                        content = fullFile,
                        sentAt = DateTime.Now,
                        type = file.ContentType
                    };
                    context.Chats.Add(chat);
                    context.SaveChanges();
                    return true;
                }
            }
            return false;
        }
    }
}
