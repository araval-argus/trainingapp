using ChatApp.Context.EntityClasses;
using ChatApp.Models;
using System.Collections.Generic;

namespace ChatApp.Business.ServiceInterfaces
{
    public interface IGroupMessageService
    {
        IList<GroupMessageModel> FetchGroupMessages(int groupId);
        GroupMessage AddGroupMessage(GroupMessageModel groupMessage);
        GroupMessage FetchGroupMessageFromId(int groupMessageId);
        void DeleteAllGroupMessages(int groupId);
        void RemoveAllGroupMessagesSentByMember(int memberId, int groupId);
        GroupMessage FetchLastMessage(int groupId);
        IList<GroupMessage> FetchAllGroupMessages(int userId);
    }
}
