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

        public ProfileService(ChatAppContext context)
        {
            this.context = context;
        }

        // this method checks (username and password) or (email and password)
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

        public Profile UpdateProfile(RegisterModel regModel)
        {
            Profile user = FetchProfile(regModel.UserName);
            if(user != null)
            {

                user.FirstName = regModel.FirstName;
                user.LastName = regModel.LastName;
                user.Email = regModel.Email;

                context.Profiles.Update(user);
                context.SaveChanges();
            }
            return user;
        }

        private bool CheckEmailOrUserNameExists(string userName, string email)
        {
            return context.Profiles.Any(x => x.Email.ToLower().Trim() == email.ToLower().Trim() || x.UserName.ToLower().Trim() == userName.ToLower().Trim());
        }

        public Profile FetchProfile(string userName)
        {
            Profile user = null;
            user =  context.Profiles.FirstOrDefault(u => u.UserName.Trim() == userName.Trim());
            return user;
        }

        
    }
}
