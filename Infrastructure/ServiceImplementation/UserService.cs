using ChatApp.Business.ServiceInterfaces;
using ChatApp.Context;
using ChatApp.Context.EntityClasses;
using ChatApp.Models;
using ChatApp.Models.Users;
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

        public UserModel GetUserByUsername(string username)
        {
            Profile user = this.context.Profiles.FirstOrDefault( (user) => String.Equals(user.UserName, username));

            // if not found
            if (user == null)
            {
                return null;
            }

            UserModel safeObj = ConvertToSafeUserObjects(user);

            return safeObj;
        }

        public  IEnumerable<UserModel> GetUsers()
        {
            IEnumerable<Profile> users =  context.Profiles.ToList();
            IEnumerable<UserModel> safeObject = (IEnumerable<UserModel>)ConvertToSafeUserObjects(users);

            return safeObject;
        }


        public async Task<UserModel> UpdateUser(UserUpdateModel user, string userName)
        {
            // check if the username provided is not an existing one
            //Profile userCheck = this.context.Profiles.FirstOrDefault((u) => String.Equals(u.UserName, user.UserName));

            //// another user exist of same name
            //if (userCheck != null)
            //{
            //    return null;
            //}

            Profile updateObj = context.Profiles.FirstOrDefault((u) => String.Equals(u.UserName, userName));

            // if user to update is not found
            if (updateObj == null)
            {
                return null;
            }

            updateObj.UserName = user.UserName;
            updateObj.FirstName = user.FirstName;
            updateObj.LastName = user.LastName;
            updateObj.Email = user.Email;
            updateObj.LastUpdatedAt = DateTime.Now;

            // temporary -- will be updated after profile type will be implemented
            updateObj.LastUpdatedBy = updateObj.Id;

            _ = await context.SaveChangesAsync();

            UserModel safeObj = ConvertToSafeUserObjects(updateObj);

            return safeObj;

        }


        // helper functions 


        // converts the profile object so that password and profile type were not sent to the client side
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
