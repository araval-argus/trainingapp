﻿using ChatApp.Models;
using System.Collections.Generic;

namespace ChatApp.Business.ServiceInterfaces
{
    public interface IChatService
    {
        bool AddChat(ChatModel chatModel, string userName);

        List<ChatDTO> GetAllChats(string from, string to);

        List<recentChatDTO> recent(string from);
    }
}