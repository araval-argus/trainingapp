using ChatApp.Business.ServiceInterfaces;
using ChatApp.Context;
using ChatApp.Context.EntityClasses;
using ChatApp.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChatApp.Infrastructure.ServiceImplementation
{
    public class UserService : IUserService
    {
        private readonly ChatAppContext context;

        public UserService(ChatAppContext context)
        {
            this.context = context;
        }

        public Profile GetUserByUsername(string username)
        {
            Profile user = null;
            return user;
        }

        public async Task<IEnumerable<UserModel>> GetUsers()
        {
            IEnumerable<Profile> users =  (IEnumerable<Profile>)await context.Profiles.ToListAsync();
            IEnumerable<UserModel> safeObject = (IEnumerable<UserModel>)ConvertToSafeUserObjects(users);

            return safeObject;
        }


        // helper functions 
        private static UserModel ConvertToSafeUserObjects(Profile profile)
        {
            UserModel userModel = new();

            userModel.Id = profile.Id;
            userModel.FirstName = profile.FirstName;
            userModel.LastName = profile.LastName;
            userModel.UserName = profile.UserName;
            userModel.Email = profile.Email;
            userModel.CreatedAt = profile.CreatedAt;
            userModel.CreatedBy = profile.CreatedBy;
            userModel.LastUpdatedAt = profile.LastUpdatedAt;
            userModel.LastUpdatedBy = profile.LastUpdatedBy;

            return userModel;
        }

        private static IEnumerable<UserModel> ConvertToSafeUserObjects(IEnumerable<Profile> profiles)
        {
            IEnumerable<UserModel> userModels = Enumerable.Empty<UserModel>();

            for (int i = 0; i < profiles.Count(); i++)
            {
                UserModel user = ConvertToSafeUserObjects(profiles.ElementAt(i));
                userModels = userModels.Append(user);
            }

            return userModels;
        }

    }
}
