using ChatApp.Context.EntityClasses;
using ChatApp.Models;

namespace ChatApp.Business.ServiceInterfaces
{
    public interface IOnlineUserService
    {
        void RegisterOnlineUser(OnlineUserEntity onlineUserEntity);

        OnlineUserEntity FetchOnlineUser(string username); 

        void RemoveOnlineUser(OnlineUserEntity onlineUserEntity);
    }
}
