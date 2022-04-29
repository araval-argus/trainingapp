using ChatApp.Context.EntityClasses;
using ChatApp.Models;
using ChatApp.Models.Chat;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace ChatApp.Business.ServiceInterfaces
{
    public interface IChatService
    {
        IEnumerable<ChatModel> ChatLists(int userFrom, int userTo, int limit=50);

        IEnumerable<RecentChatUsers> RecentChatUsers(int userId);

        ChatModel SendTextMessage(string userFrom, string userTo, string content, int replyTo);

        ChatModel SendImageMessage(string userFrom, string userTo, IFormFile content, int replyTo);

        void MarkConversationAsRead(int userId, int friendId);
    }
}
