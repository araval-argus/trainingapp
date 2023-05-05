using ChatApp.Business.ServiceInterfaces;
using ChatApp.Context;
using ChatApp.Context.EntityClasses;
using ChatApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ChatApp.Infrastructure.ServiceImplementation
{
    public class OnlineUserService: IOnlineUserService
    {
        private readonly ChatAppContext context;

        public OnlineUserService(ChatAppContext context)
        {
            this.context = context;
        }

        public void RegisterOnlineUser(OnlineUserEntity onlineUserEntity)
        {

            if (this.context.OnlineUsers.Any(u => u.UserName == onlineUserEntity.UserName))
            {
                IEnumerable<OnlineUserEntity> users = this.context.OnlineUsers.Where(u => u.UserName == onlineUserEntity.UserName);
                this.context.OnlineUsers.RemoveRange(users);
            }
            
            this.context.OnlineUsers.Add(onlineUserEntity);
            this.context.SaveChanges();
        }

        public OnlineUserEntity FetchOnlineUser(string username)
        {
            OnlineUserEntity onlineUser = this.context.OnlineUsers.FirstOrDefault(user => user.UserName == username);
            return onlineUser;
        }

        public void RemoveOnlineUser(OnlineUserEntity onlineUserEntity)
        {
            this.context.OnlineUsers.Remove(onlineUserEntity);
            this.context.SaveChanges();
        }
    }
}
