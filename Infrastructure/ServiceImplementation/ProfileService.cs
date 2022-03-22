using ChatApp.Business.Helpers;
using ChatApp.Business.ServiceInterfaces;
using ChatApp.Context;
using ChatApp.Context.EntityClasses;
using ChatApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChatApp.Infrastructure.ServiceImplementation
{
    public class ProfileService : IProfileService
    {
        private readonly ChatAppContext context;
        private readonly IUserService userService;

        public ProfileService(ChatAppContext context, IUserService userService)
        {
            this.context = context;
            this.userService = userService;
        }

        public Profile CheckPassword(LoginModel model)
        {
            return this.context.Profiles.FirstOrDefault(x => model.Password == x.Password
            && (x.Email.ToLower().Trim() == model.EmailAddress.ToLower().Trim() || x.UserName.ToLower().Trim() == model.Username.ToLower().Trim()));
        }

        public Profile RegisterUser(RegisterModel regModel)
        {
            Profile newUser = null;
            if (!CheckEmailOrUserNameExists(regModel.UserName, regModel.Email))
            {
                newUser = new Profile
                {
                    FirstName = regModel.FirstName,
                    LastName = regModel.LastName,
                    Password = regModel.Password,
                    UserName = regModel.UserName,
                    Email = regModel.Email,
                    CreatedAt = DateTime.UtcNow,
                    ProfileType = ProfileType.User
                };
                context.Profiles.Add(newUser);
                context.SaveChanges();
            }
            return newUser;
        }

        private bool CheckEmailOrUserNameExists(string userName, string email)
        {
            return context.Profiles.Any(x => x.Email.ToLower().Trim() == email.ToLower().Trim() || x.UserName.ToLower().Trim() == userName.ToLower().Trim());
        }


        public bool LoginUser(int userId)
        {
            var userObj = context.Profiles.FirstOrDefault(u => u.Id == userId);

            userObj.IsLoggedIn = 1;
            //userObj.LastSeen = DateTime.Now;

            if (context.SaveChanges() > 0)
            {
                return true;
            }

            return false;
        }

        public bool LogoutUser(int userId)
        {
            var userObj = context.Profiles.FirstOrDefault(u => u.Id == userId);

            userObj.IsLoggedIn = 0;
            userObj.LastSeen = DateTime.Now;

            if (context.SaveChanges() > 0)
            {
                return true;
            }

            return false;
        }
    }
}
