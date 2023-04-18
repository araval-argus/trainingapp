using ChatApp.Business.Helpers;
using ChatApp.Business.ServiceInterfaces;
using ChatApp.Context.EntityClasses;
using ChatApp.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;
using System;
using System.IO;
using System.Threading.Tasks;

namespace ChatApp.Hubs
{
    public class ChatHub: Hub
    {
        private IOnlineUserService onlineUserService;
        private IProfileService profileService;
        private IChatService chatService;
        private IWebHostEnvironment webHostEnvironment;

        public ChatHub(IOnlineUserService onlineUserService,
            IProfileService profileService,
            IChatService chatService,
            IWebHostEnvironment webHostEnvironment
            )
        {
            this.onlineUserService = onlineUserService;
            this.profileService = profileService;
            this.chatService = chatService;
            this.webHostEnvironment = webHostEnvironment;
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
            }
            if (exception != null)
            {
                Console.WriteLine(exception.Message);
            }
            return base.OnDisconnectedAsync(exception);
        }

        public async Task RegisterUser(string userName)
        {
            OnlineUserEntity onlineUserEntity = new OnlineUserEntity()
            {
                UserName = userName,
                ConnectionId = Context.ConnectionId
            };

            this.onlineUserService.RegisterOnlineUser(onlineUserEntity);
        }

        public async Task AddMessage(MessageModel messageModel)
        {
            Console.WriteLine("Addmessage");
            
            MessageEntity messageEntityFromDb = this.chatService.AddMessage(messageModel);

            MessageModel message = ConvertToMessageModel(messageEntityFromDb);

            string senderConnectionId = this.onlineUserService.FetchOnlineUser(messageModel.SenderUserName).ConnectionId;
            OnlineUserEntity reciever = this.onlineUserService.FetchOnlineUser(messageModel.RecieverUserName);
            if (reciever != null)
            {
                await Clients.Clients(senderConnectionId, reciever.ConnectionId).SendAsync("AddMessageToTheList", message);
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
        }


        #region helpermethods

        MessageModel ConvertToMessageModel(MessageEntity messageEntity)
        {
            MessageModel messageModel = new MessageModel()
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


        string FetchUserNameFromId(int id)
        {
            return this.profileService.FetchUserNameFromId(id);
        }
        #endregion
    }
}
