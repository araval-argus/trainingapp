using ChatApp.Context.EntityClasses;
using ChatApp.Models;
using System.Collections.Generic;

namespace ChatApp.Business.ServiceInterfaces
{
    public interface INotificationService
    {
        void addNotification(string name, bool isGroup, int user);
        List<NotificationModel> getAll(string userName);
    }
}
