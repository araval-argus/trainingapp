using ChatApp.Context.EntityClasses;
using ChatApp.Models;
using ChatApp.Models.Chat;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChatApp.Business.ServiceInterfaces
{
    public interface IChatService
    {
        IEnumerable<ChatModel> chatLists(int userFrom, int userTo, int limit=50);

        IEnumerable<UserModel> recentChatUsers(int userId);

        ChatModel sendTextMessage(string userFrom, string userTo, string content);
    }
}
