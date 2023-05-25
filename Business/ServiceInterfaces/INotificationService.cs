using ChatApp.Context.EntityClasses;
using ChatApp.Models;
using System.Collections.Generic;

namespace ChatApp.Business.ServiceInterfaces
{
    public interface INotificationService
    {
        Notification AddNotification(Notification notification);
        void DeleteNotifications(IList<NotificationModel> notificationModelList);
        IList<NotificationModel> GetAllNotifications(string userId);
        IList<NotificationModel> GetMessagesNotifications(string raisedFor, string raisedBy);
        IList<NotificationModel> GetGroupMessagesNotifications(string raisedFor, int raisedInGroup);
    }
}
