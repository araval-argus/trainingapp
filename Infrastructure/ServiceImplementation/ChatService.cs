﻿using ChatApp.Business.Helpers;
using ChatApp.Business.ServiceInterfaces;
using ChatApp.Context;
using ChatApp.Context.EntityClasses;
using ChatApp.Hubs;
using ChatApp.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.SignalR;
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
        private readonly IHubContext<ChatHub> _hubContext;
        private readonly INotificationService _notificationService;

        public ChatService(ChatAppContext context, IProfileService profileService, IWebHostEnvironment webHostEnvironment, IHubContext<ChatHub> hubContext, INotificationService notificationService)
        {
            this.context = context;
            this._profileService = profileService;
            this._webHostEnvironment = webHostEnvironment;
            this._hubContext = hubContext;
            this._notificationService = notificationService;
        }

        public chatFormat AddChat(ChatModel chatModel, string userName)
        {
            Profile profile = context.Profiles.FirstOrDefault(p => p.UserName == userName);
            Profile receiver = context.Profiles.FirstOrDefault(p => p.UserName == chatModel.To);
            if(chatModel == null)
            {
                return null;
            }
            Chat chat = new()
            {
                From = profile.Id,
                To = receiver.Id,
                content = chatModel.content,
                sentAt = DateTime.Now,
                replyToChat = chatModel.replyToChat,
                type = "text"
            };
            context.Chats.Add(chat);
            context.SaveChanges();
            Connections activeUser = context.Connections.AsNoTracking().FirstOrDefault(e => e.User == receiver.Id);
            chatFormat chatContent = new()
            {
                id = chat.Id,
                content = chat.content,
                sentAt = chat.sentAt,
                replyToChat = chat.replyToChat,
                isRead = chat.isRead,
                type = chat.type,
            };
            _notificationService.addNotification(userName, false, receiver.Id);
            if (activeUser != null)
            {
                //Profile is sender's profile
                recentChatDTO recentChatDTO = new()
                {
                    to = Mapper.profileMapper(profile),
                    chatContent = chatContent
                };
                _hubContext.Clients.Client(activeUser.ConnectionId).SendAsync("receiveChat", recentChatDTO);
            }
            return chatContent;
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
            IEnumerable<Chat> sentChats = context.Chats.AsNoTracking().Where(e => e.From == id || e.To == id ).ToList();
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
                        isRead = chat.isRead,
                        type = chat.type,
                    },
                    unreadCount = unreadCountDictionary[receiver.Id]
                });
            }
            return recentChatDTOs;
        }


        public chatFormat addFile(string userName, ChatFileModel chatFile)
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
                    _notificationService.addNotification(userName, false, receiver.Id);
                    //In case user provide invalid replyToChat Id
                    var parse = int.TryParse(chatFile.replyToChat, out int replyToChat);
                    Chat chat = new()
                    {
                        From = sender.Id,
                        To = receiver.Id,
                        content = fullFile,
                        sentAt = DateTime.Now,
                        type = file.ContentType,
                        replyToChat = parse ? replyToChat : -1 
                    };
                    context.Chats.Add(chat);
                    context.SaveChanges();
                    Connections activeUser = context.Connections.AsNoTracking().FirstOrDefault(e => e.User == receiver.Id);
                    chatFormat chatContent = new()
                    {
                        id = chat.Id,
                        content = chat.content,
                        sentAt = chat.sentAt,
                        replyToChat = chat.replyToChat,
                        isRead = chat.isRead,
                        type = chat.type,
                    };
                    if (activeUser != null)
                    {
                        //Profile is sender's profile
                        recentChatDTO recentChatDTO = new()
                        {
                            to = Mapper.profileMapper(sender),
                            chatContent = chatContent
                        };
                        _hubContext.Clients.Client(activeUser.ConnectionId).SendAsync("receiveChat", recentChatDTO);
                    }
                    return chatContent;
                }
            }
            return null;
        }

        public bool markAsRead(string user, string markAsRead)
        {
            Profile receiver = context.Profiles.AsNoTracking().FirstOrDefault(e => e.UserName == user);
            Profile sender = context.Profiles.AsNoTracking().FirstOrDefault(e => e.UserName == markAsRead);
            if (sender != null && receiver != null)
            {
                IEnumerable<Chat> chatsToMarkRead = context.Chats.Where(e => e.From == sender.Id && e.To == receiver.Id && e.isRead == 0);
                if(chatsToMarkRead.Count() > 0)
                {
                    foreach (Chat chat in chatsToMarkRead)
                    {
                        chat.isRead = 1;
                    }
                    context.Chats.UpdateRange(chatsToMarkRead);
                    context.SaveChanges();
                    return true;
                }
            }
            return false;
        }
    }
}
