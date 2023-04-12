﻿using ChatApp.Context.EntityClasses;
using ChatApp.Models;
using System.Collections.Generic;

namespace ChatApp.Business.ServiceInterfaces
{
    public interface IChatService
    {
        IEnumerable<FriendProfileModel> FetchFriendsProfiles(string searchTerm);

        MessageEntity AddMessage(MessageModel messageModel);

        IEnumerable<MessageEntity> FetchMessages(int senderID, int recieverID);

        string FetchMessageFromId(int id);

        void MarkMsgsAsSeen(IEnumerable<MessageEntity> messages);
    }
}
