using ChatApp.Business.Helpers;
using ChatApp.Business.ServiceInterfaces;
using ChatApp.Context;
using ChatApp.Context.EntityClasses;
using ChatApp.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;

namespace ChatApp.Infrastructure.ServiceImplementation
{
    public class NotificationService: INotificationService
    {
        private readonly ChatAppContext context;
        public NotificationService(ChatAppContext context)
        {
            this.context = context;
        }
        public Notification AddNotification(Notification notification)
        {
            var entityEntry = this.context.Notifications.Add(notification);
            this.context.SaveChanges();
            entityEntry.Entity.NotificationType = this.context.NotificationTypes.FirstOrDefault(type => type.Id == notification.Type);
            return entityEntry.Entity;
        }

        public void DeleteNotifications(IList<NotificationModel> notificationModelList)
        {
            IList<Notification> notifications = new List<Notification>();
            foreach (var notification in notificationModelList)
            {
                notifications.Add(this.context.Notifications.FirstOrDefault(n => n.Id == notification.Id));
            }
            this.context.Notifications.RemoveRange(notifications);
            this.context.SaveChanges();
        }

        public IList<NotificationModel> GetAllNotifications(string userId)
        {
            IList<NotificationModel> notificationModels = new List<NotificationModel>();
            var notifications = this.context.Notifications.Include("NotificationType").
                Include("NotificationCreator").
                Include("NotificationReceiver").
                Where(notification => notification.RaisedFor == userId);
            foreach(var notification in notifications)
            {
                var notificationModel = EntityToModel.ConvertToNotificationModel(notification);
                if(notification.RaisedInGroup != null)
                {
                    notificationModel.RaisedInGroup = this.context.Groups.FirstOrDefault(group => group.Id == notification.RaisedInGroup).Name;
                }
                notificationModels.Add(notificationModel);
            }
            return notificationModels;
        }

        public IList<NotificationModel> GetMessagesNotifications(string raisedFor, string raisedBy)
        {
            IList<NotificationModel> notificationModels = new List<NotificationModel>();
            var notifications = this.context.Notifications.Include("NotificationType").
                Include("NotificationCreator").
                Include("NotificationReceiver").
                Where(notification => notification.RaisedFor == raisedFor && notification.RaisedBy == raisedBy);
            foreach (var notification in notifications)
            {                
                notificationModels.Add(EntityToModel.ConvertToNotificationModel(notification));
            }
            return notificationModels;
        }

        public IList<NotificationModel> GetGroupMessagesNotifications(string raisedFor, int raisedInGroup)
        {
            IList<NotificationModel> notificationModels = new List<NotificationModel>();
            var notifications = this.context.Notifications.Include("NotificationType").
                Include("NotificationCreator").
                Include("NotificationReceiver").
                Where(notification => notification.RaisedFor == raisedFor && notification.RaisedInGroup == raisedInGroup);
            foreach (var notification in notifications)
            {
                var notificationModel = EntityToModel.ConvertToNotificationModel(notification);
                if (notification.RaisedInGroup != null)
                {
                    notificationModel.RaisedInGroup = this.context.Groups.FirstOrDefault(group => group.Id == notification.RaisedInGroup).Name;
                }
                notificationModels.Add(notificationModel);
            }
            return notificationModels;
        }
    }
}
