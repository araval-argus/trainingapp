using ChatApp.Context.EntityClasses;
using ChatApp.Models;
using System.Collections.Generic;

namespace ChatApp.Business.ServiceInterfaces
{
    public interface INotificationService
    {
        void addNotification(string name, bool isGroup, int user);
        List<NotificationModel> getAll(string userName);

        bool markAsSeen(string userName);

        bool deleteAll(string userName);

        bool delete(string userName,int id);
        bool view(string userName,int id);
    }
}
