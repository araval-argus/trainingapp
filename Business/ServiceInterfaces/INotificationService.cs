using ChatApp.Context.EntityClasses;
using ChatApp.Models;
using System.Collections.Generic;

namespace ChatApp.Business.ServiceInterfaces
{
    public interface INotificationService
    {
        Notification AddNotification(Notification notification);
        void DeleteNotifications(IList<NotificationModel> notificationModelList);
        IList<NotificationModel> GetAllNotifications(int userId);
        IList<NotificationModel> GetMessagesNotifications(int raisedFor, int raisedBy);
        IList<NotificationModel> GetGroupMessagesNotifications(int raisedFor, int raisedInGroup);
    }
}
