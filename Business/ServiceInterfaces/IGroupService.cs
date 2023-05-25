using ChatApp.Context.EntityClasses;
using ChatApp.Models;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;
using System.Collections.Generic;

namespace ChatApp.Business.ServiceInterfaces
{
    public interface IGroupService
    {
        GroupModel CreateGroup(GroupModel groupModel, string creatorId);
        IEnumerable<GroupModel> FetchGroups(string userId);
        Group FetchGroupFromId(int groupId);
        void DeleteGroup(int groupId);
        void UpdateGroup(GroupModel groupModel, Profile user);
    }
}
