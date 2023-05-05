using ChatApp.Context.EntityClasses;
using ChatApp.Models;
using System;

namespace ChatApp.Business.Helpers
{
    public static class EntityToModel
    {
        public static GroupModel ConvertToGroupModel(Group group)
        {
            GroupModel groupModel = new()
            {
                Id = group.Id,
                Name = group.Name,
                Description = group.Description,
                GroupIconUrl = group.GroupIcon,
                CreatorUserName = group.Creator.UserName,
                CreatedAt = group.CreatedAt,
                lastUpdatedAt = group.LastUpdatedAt,
                lastUpdatedBy = group.LastModifier.UserName
            };

            return groupModel;
        }

        //before using this method make sure that you have included the designation property in the profile model
        public static UserModel ConvertToUserModel(Profile profile)
        {
            return new UserModel()
            {
                UserName = profile.UserName,
                FirstName = profile.FirstName,
                LastName = profile.LastName,
                Designation = profile.Designation,
                Email = profile.Email,
                ImageUrl = profile.ImageUrl,
            };
        }

        public static GroupMessageModel ConvertToGroupMessageModel(GroupMessage groupMessage)
        {
            GroupMessageModel groupMessageModel = new()
            {
                Id = groupMessage.Id,
                Message = groupMessage.Message,
                SenderUserName = groupMessage.Sender.UserName,
                GroupId = groupMessage.GroupID,
                MessageType = groupMessage.MessageType,
                CreatedAt = groupMessage.CreatedAt
            };
            return groupMessageModel;
        }
    }
}
