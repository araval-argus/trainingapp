using ChatApp.Business.Helpers;
using ChatApp.Business.ServiceInterfaces;
using ChatApp.Context;
using ChatApp.Context.EntityClasses;
using ChatApp.Hubs;
using ChatApp.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ChatApp.Infrastructure.ServiceImplementation
{
    public class NotificationService : INotificationService
    {
        private readonly ChatAppContext context;
        private readonly IHubContext<ChatHub> _hubContext;


        public NotificationService(ChatAppContext context, IHubContext<ChatHub> hubContext)
        {
            this.context = context;
            this._hubContext = hubContext;

        }
        public void addNotification(string from, bool isGroup, int user)
        {
            //from will be sender either from user or from group
            //user will be userId of user, who will receive the notification

            //sent the notification to the user
            if(isGroup)
            {
                List<int> GroupUsers = context.GroupMember.AsNoTracking().Include(e => e.Group).Where(e => e.Group.Name == from).Select(e => e.MemberId).ToList();
                foreach(int userId in GroupUsers)
                {
                    if(userId != user)
                    {
                        createAndSendNotification(from, true, userId);
                    }
                }
            }
            else
            {
                createAndSendNotification(from, false, user);
            }
        }


        public List<NotificationModel> getAll(string userName)
        {
            List<Notifications> notifications = context.Notification.AsNoTracking().Include(e => e.Profile).Where(e => e.Profile.UserName == userName).ToList();
            List<NotificationModel> notificationModels = Mapper.notificationsMapper(notifications);
            return notificationModels;
        }

        public bool deleteAll(string userName)
        {
            Profile profile = context.Profiles.AsNoTracking().FirstOrDefault(e => e.UserName.Equals(userName));
            if (profile != null)
            {
                IEnumerable<Notifications> notifications = context.Notification.Where(e => e.User == profile.Id);
                context.RemoveRange(notifications);
                context.SaveChanges();
                return true;
            }
            return false;
        }

        public bool markAsSeen(string userName) {
            Profile profile = context.Profiles.AsNoTracking().FirstOrDefault( e => e.UserName.Equals(userName));
            if(profile != null)
            {
                IEnumerable<Notifications> notifications = context.Notification.Where(e => e.User == profile.Id && e.isSeen == 0);
                foreach(Notifications notification in notifications)
                {
                    notification.isSeen = 1;
                }
                context.UpdateRange(notifications); 
                context.SaveChanges();
                return true;
            }
            return false;
        }
        private void createAndSendNotification(string from, bool isGroup, int user)
        {
            //Notification to be save
            Notifications newNotification = new Notifications();
            newNotification.Content = from;
            newNotification.User = user;
            newNotification.Time = DateTime.Now;
            newNotification.isGroup = isGroup ? 1 : 0;
            context.Notification.Add(newNotification);
            context.SaveChanges();

            //send to individual
            
            Connections isActiveConnection = context.Connections.AsNoTracking().FirstOrDefault(e => e.User == user);
            if (isActiveConnection != null)
            {
                //Notification to be sent   
                NotificationModel toBeSentNotication = new NotificationModel();
                toBeSentNotication.Content = newNotification.Content;
                toBeSentNotication.Id = newNotification.Id;
                toBeSentNotication.Time = newNotification.Time;
                toBeSentNotication.isSeen = newNotification.isSeen;
                toBeSentNotication.isGroup = isGroup ? 1 : 0;

                //senf Notificaiton
                _hubContext.Clients.Client(isActiveConnection.ConnectionId).SendAsync("newNotification", toBeSentNotication);
            }

        }

        public bool delete(string userName, int id)
        {
            Notifications notification = context.Notification.Include(e => e.Profile).FirstOrDefault(e => e.Id == id);
            if (notification != null && notification.Profile.UserName == userName)
            {
                context.Notification.Remove(notification);
                context.SaveChanges();
                return true;
            }
            return false;
        }


        public bool view(string userName, int id)
        {
            Notifications notification = context.Notification.Include(e => e.Profile).FirstOrDefault(e => e.Id == id);
            if (notification != null && notification.Profile.UserName == userName)
            {
                notification.isSeen = 1;
                context.Update(notification);
                context.SaveChanges();
                return true;
            }
            return false;
        }
    }
}
