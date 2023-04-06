using ChatApp.Business.Helpers;
using ChatApp.Business.ServiceInterfaces;
using ChatApp.Context;
using ChatApp.Context.EntityClasses;
using ChatApp.Models;
using Microsoft.AspNetCore.Hosting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace ChatApp.Infrastructure.ServiceImplementation
{
    public class ProfileService : IProfileService
    {
        private readonly ChatAppContext context;
        private IWebHostEnvironment _webHostEnvironment;

        public ProfileService(ChatAppContext context, IWebHostEnvironment webHostEnvironment)
        {
            this.context = context;
            this._webHostEnvironment = webHostEnvironment;
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
                    ProfileType = ProfileType.User,
                    ImageUrl = SetDefaultImage()
                };
                context.Profiles.Add(newUser);
                context.SaveChanges();
            }
            return newUser;
        }

        public Profile UpdateProfile(UpdateModel updateModel, string username, bool updateImage = false)
        {
            Profile user = FetchProfile(username);
            if (user != null)
            {
                user.UserName = updateModel.UserName;
                user.FirstName = updateModel.FirstName;
                user.LastName = updateModel.LastName;
                user.Email = updateModel.Email;

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
            user = context.Profiles.FirstOrDefault(u => u.UserName.Trim() == userName.Trim());
            return user;
        }

        public bool CheckUserNameExists(string? userName = null)
        {
            if (userName == null)
            {
                return false;
            }
            return context.Profiles.Any(p => p.UserName.Trim() == userName.Trim());
        }

        private string SetDefaultImage()
        {
            string fileName = Guid.NewGuid().ToString() + ".jpg";
            
            var DefaultFileLoc = Path.Combine(_webHostEnvironment.WebRootPath, @"Images/default_profile.jpg");
            var FileLoc = Path.Combine(DefaultFileLoc, @"../Users", fileName );

            File.Copy(DefaultFileLoc, FileLoc);

            return fileName;
        }

    }
}

