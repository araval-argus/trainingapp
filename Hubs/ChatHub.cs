using ChatApp.Business.Helpers;
using ChatApp.Business.ServiceInterfaces;
using ChatApp.Context.EntityClasses;
using ChatApp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Threading.Tasks;

namespace ChatApp.Hubs
{
    
    public class ChatHub: Hub
    {
        private readonly IOnlineUserService onlineUserService;
        private readonly IProfileService profileService;
        private readonly IChatService chatService;
        private readonly IGroupMessageService groupMessageService;
        private readonly IGroupMemberService groupMemberService;
        private readonly INotificationService notificationService;
        private readonly IGroupService groupService;

        public ChatHub(IOnlineUserService onlineUserService,
            IProfileService profileService,
            IChatService chatService,
            IGroupMessageService groupMessageService,
            IGroupMemberService groupMemberService,
            INotificationService notificationService,
            IGroupService groupService
            )
        {
            this.onlineUserService = onlineUserService;
            this.profileService = profileService;
            this.chatService = chatService;
            this.groupMessageService = groupMessageService;
            this.groupMemberService = groupMemberService;
            this.notificationService = notificationService;
            this.groupService = groupService;
        }

        public override Task OnConnectedAsync()
        {
            return base.OnConnectedAsync();
        }

        public override Task OnDisconnectedAsync(Exception exception)
        {
            string userConnectionId = Context.ConnectionId;
            OnlineUserEntity onlineUser = this.onlineUserService.FetchOnlineUser( userConnectionId );
            if (onlineUser != null)
            {
                this.onlineUserService.RemoveOnlineUser(onlineUser);
                Console.WriteLine("disconnected");
            }
            if (exception != null)
            {
                Console.WriteLine(exception.Message);
            }
            return base.OnDisconnectedAsync(exception);
        }

        public void RegisterUser(string userName)
        {
            OnlineUserEntity onlineUserEntity = new()
            {
                UserName = userName,
                ConnectionId = Context.ConnectionId
            };

            this.onlineUserService.RegisterOnlineUser(onlineUserEntity);
        }

        public async Task AddMessage(MessageModel messageModel)
        {
            
            MessageEntity messageEntityFromDb = this.chatService.AddMessage(messageModel);

            MessageModel message = ConvertToMessageModel(messageEntityFromDb);
            Notification notification = new()
            {
                Type = 1,
                RaisedBy = messageEntityFromDb.SenderID,
                RaisedFor = messageEntityFromDb.RecieverID,
                CreatedAt = messageEntityFromDb.CreatedAt
                
            };

            notification = this.notificationService.AddNotification(notification);

            string senderConnectionId = this.onlineUserService.FetchOnlineUser(messageModel.SenderUserName).ConnectionId;
            OnlineUserEntity reciever = this.onlineUserService.FetchOnlineUser(messageModel.RecieverUserName);

            if (reciever != null)
            {
                await Clients.Clients(senderConnectionId, reciever.ConnectionId).SendAsync("AddMessageToTheList", message);               
                NotificationModel notificationModel = EntityToModel.ConvertToNotificationModel(notification);
                await Clients.Client(reciever.ConnectionId).SendAsync("AddNotification", notificationModel);
            }
            else
            {
                await Clients.Client(senderConnectionId).SendAsync("AddMessageToTheList", message);
            }

        }

        public async Task LogoutUser(string userName)
        {
            OnlineUserEntity onlineUserEntity = this.onlineUserService.FetchOnlineUser(userName);
            this.onlineUserService.RemoveOnlineUser(onlineUserEntity);
            await Clients.Caller.SendAsync("StopConnection");
        }

        public async Task AddGroupMessage(GroupMessageModel groupMessageModel)
        {

            GroupMessage groupMessageFromDb = this.groupMessageService.AddGroupMessage(groupMessageModel);

            GroupMessageModel message = EntityToModel.ConvertToGroupMessageModel(groupMessageFromDb);            

            if (groupMessageFromDb.RepliedToMsg >= 0)
            {
                message.RepliedToMsg = this.groupMessageService.FetchGroupMessageFromId(groupMessageFromDb.RepliedToMsg).Message;
            }

            IList<GroupMember> members = this.groupMemberService.ListOfJoinedMembers(groupMessageModel.GroupId);
            IList<string> onlineMembersConnectionIds = new List<string>();
            Notification notification = new()
            {
                Type = 5,
                RaisedBy = groupMessageFromDb.SenderID,
                CreatedAt = groupMessageFromDb.CreatedAt,
                RaisedInGroup = groupMessageModel.GroupId
            };

            foreach (GroupMember member in members)
            {
                string userName = this.profileService.FetchUserNameFromId(member.ProfileID);
                OnlineUserEntity onlineUser = this.onlineUserService.FetchOnlineUser(userName);

                notification.RaisedFor = member.ProfileID;
                if(notification.RaisedFor != notification.RaisedBy)
                {
                    notification = this.notificationService.AddNotification(notification);
                }
                
                if (onlineUser!= null)
                {
                    onlineMembersConnectionIds.Add(onlineUser.ConnectionId);
                    //because each user has his seperate notification
                    if(onlineUser.UserName != groupMessageModel.SenderUserName)
                    {
                        var notificationModel = EntityToModel.ConvertToNotificationModel(notification);
                        notificationModel.RaisedInGroup = this.groupService.FetchGroupFromId(groupMessageModel.GroupId).Name;
                        await Clients.Client(onlineUser.ConnectionId).SendAsync("AddNotification", notificationModel);
                    }
                    
                }
                notification.Id = 0;

            }
            if(onlineMembersConnectionIds.Count > 0)
            {
                await Clients.Clients(onlineMembersConnectionIds).SendAsync("AddGroupMessageToTheList", message);
            }
        }
        
        public async Task MarkMessageAsSeen(MessageModel messageModel)
        {
            this.chatService.MarkMsgAsSeen(messageModel.Id);
        }


        #region helpermethods

        MessageModel ConvertToMessageModel(MessageEntity messageEntity)
        {
            MessageModel messageModel = new()
            {
                Id = messageEntity.Id,
                MessageType = messageEntity.MessageType,
                Message = messageEntity.Message,
                CreatedAt = messageEntity.CreatedAt,
                IsSeen = messageEntity.IsSeen,
                RecieverUserName = FetchUserNameFromId(messageEntity.RecieverID),
                SenderUserName = FetchUserNameFromId(messageEntity.SenderID),
                RepliedToMsg = this.chatService.FetchMessageFromId(messageEntity.RepliedToMsg)
            };

            return messageModel;
        }


        string FetchUserNameFromId(string id)
        {
            return this.profileService.FetchUserNameFromId(id);
        }
        #endregion
    }
}
