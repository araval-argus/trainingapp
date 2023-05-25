using ChatApp.Context.EntityClasses;
using ChatApp.Models;
using System.Collections.Generic;

namespace ChatApp.Business.ServiceInterfaces
{
    public interface IChatService
    {
        IEnumerable<UserModel> FetchFriendsProfiles(string searchTerm);

        MessageEntity AddMessage(MessageModel messageModel);

        IEnumerable<MessageEntity> FetchMessages(string senderID, string recieverID);

        string FetchMessageFromId(int id);

        void MarkMsgsAsSeen(IEnumerable<MessageEntity> messages);

        void MarkMsgAsSeen(int messageId);

        //fetches all messages of all interacted users
        IList<MessageEntity> FetchAllMessages(string userID);
    }
}
