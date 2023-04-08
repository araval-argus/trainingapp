using ChatApp.Context.EntityClasses;
using ChatApp.Models;
using System.Collections.Generic;

namespace ChatApp.Business.ServiceInterfaces
{
    public interface IChatService
    {
        IEnumerable<FriendProfileModel> FetchFriendsProfiles(string searchTerm);
        //IEnumerable<FriendProfileModel> Dummy(string searchTerm); 
    }
}
