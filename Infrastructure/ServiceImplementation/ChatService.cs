using ChatApp.Business.ServiceInterfaces;
using ChatApp.Context;
using ChatApp.Context.EntityClasses;
using ChatApp.Models;
using System.Collections.Generic;
using System.Linq;

namespace ChatApp.Infrastructure.ServiceImplementation
{
    public class ChatService : IChatService
    {
        private readonly ChatAppContext context;

        public ChatService(ChatAppContext context)
        {
            this.context = context;
        }

        //public IEnumerable<FriendProfileModel> Dummy(string searchTerm)
        //{
        //    return context.Profiles.Where(profile => profile.FirstName.ToLower().StartsWith(searchTerm)).Select(profile => new FriendProfileModel()
        //    {
        //        FirstName = profile.FirstName,
        //        LastName = profile.LastName,
        //        Email = profile.Email,
        //        UserName = profile.UserName,
        //        Designation = profile.Designation,
        //        imageUrl = profile.ImageUrl,
        //    }); ;
        //}

        public IEnumerable<FriendProfileModel> FetchFriendsProfiles(string searchTerm)
        {
            return context.Profiles
                .Where(profile => profile.FirstName.ToLower().StartsWith(searchTerm))
                .Select(profile => new FriendProfileModel()
                {
                    FirstName = profile.FirstName,
                    LastName = profile.LastName,
                    Email = profile.Email,
                    UserName = profile.UserName,
                    Designation = profile.Designation,
                    imageUrl = profile.ImageUrl,
                });
        }
    }
}
