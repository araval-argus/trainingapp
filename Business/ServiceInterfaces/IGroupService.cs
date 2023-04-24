using ChatApp.Context.EntityClasses;
using ChatApp.Models;
using System.Collections.Generic;

namespace ChatApp.Business.ServiceInterfaces
{
    public interface IGroupService
    {
        GroupDTO AddGroup(GroupModel model, string userName);
        List<GroupDTO> getAll(string userName);
        List<profileDTO> getMembers(string userName, string groupName);
        bool addMembers(List<string> userName, string groupName, string adminUser);

        bool addMessage(GroupReceiveChatModel message, string senderUserName);
        List<SentGroupMessage> getAllChat(string groupName);
        bool addFile(GroupChatFileModel message, string senderUserName);

        bool UpdateGroup(GroupModel model, string userName);

        bool removeMember(List<string> userNames, string groupName, string userName);

        bool leaveGroup(string userName, string groupName);
    }
}
