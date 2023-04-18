using ChatApp.Models;
using System.Collections.Generic;

namespace ChatApp.Business.ServiceInterfaces
{
    public interface IChatService
    {
        chatFormat AddChat(ChatModel chatModel, string userId);

        List<ChatDTO> GetAllChats(string from, string to);

        List<recentChatDTO> recent(string from);

        chatFormat addFile(string userName , ChatFileModel chatFile);
        bool markAsRead(string value, string user);
    }
}
