using ChatApp.Business.Helpers;
using ChatApp.Business.ServiceInterfaces;
using ChatApp.Context;
using ChatApp.Context.EntityClasses;
using ChatApp.Models;
using Microsoft.AspNetCore.Hosting;
using System;
using System.Collections.Generic;

using System.Collections;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Runtime.CompilerServices;
using DesignationEntity = ChatApp.Context.EntityClasses.DesignationEntity;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;

namespace ChatApp.Infrastructure.ServiceImplementation
{
    public class ProfileService : IProfileService
    {
        private readonly ChatAppContext context;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public ProfileService(ChatAppContext context, IWebHostEnvironment webHostEnvironment)
        {
            this.context = context;
            this._webHostEnvironment = webHostEnvironment;
        }

        // this method checks (username and password) or (email and password)
        public Profile CheckPassword(LoginModel model)
        {
            return this.context.Profiles.Include("Designation").FirstOrDefault(x => model.Password == x.Password
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
                    ImageUrl = SetDefaultImage(),
                    DesignationID = regModel.DesignationID
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


        public int FetchIdFromUserName(string userName)
        {
            return this.context.Profiles.FirstOrDefault(
                profile => profile.UserName.Equals(userName)).Id;
        }

        public string FetchUserNameFromId(int id)
        {
            return this.context.Profiles.FirstOrDefault(
                profile => profile.Id == id
                ).UserName;
        }

       //This method will return a list of users who have interacted with the logged in user
        public IEnumerable<FriendProfileModel> FetchAllUsers(int userId)
        {
            
            // Fetch distinct user ids with whome the user with 'userId' had interacted
            IEnumerable<int> Ids = this.context.Messages.Where(
                m => m.SenderID == userId || m.RecieverID == userId
                )
                .Select(
                    m => m.SenderID == userId ? m.RecieverID : m.SenderID
                ).Distinct();

            IList<Profile> friendsProfiles = new List<Profile>(); 

            //for fetching the profiles
            foreach(int id in Ids)
            {
                var profile = this.context.Profiles.FirstOrDefault(p => p.Id == id);
                friendsProfiles.Add(profile);
            }

            return friendsProfiles
                .Select(
                profile => {
                    
                     //for counting of unread messages:-
                    

                    MessageEntity lastMessage = this.context.Messages.OrderBy(m => m.Id).LastOrDefault(m => m.SenderID == profile.Id || m.RecieverID == profile.Id);

                    return new FriendProfileModel()
                    {
                        UserName = profile.UserName,
                        Designation = Business.Helpers.Designation.getDesignationType(profile.DesignationID),
                        Email = profile.Email,
                        FirstName = profile.FirstName,
                        LastName = profile.LastName,
                        ImageUrl = profile.ImageUrl,
                        LastMessage = lastMessage.Message,
                        LastMessageTimeStamp = lastMessage.CreatedAt,
                        UnreadMessageCount = UnreadMessageCount(profile.Id, userId)
                    };
                })
                .OrderByDescending(profile => profile.LastMessageTimeStamp);
        }

        public int UnreadMessageCount(int senderID, int recieverID)
        {
            return this.context.Messages.Where(message =>
                        (message.SenderID == senderID && message.RecieverID == recieverID && !message.IsSeen)
                     ).Count();
        }

        public IEnumerable<DesignationEntity> FetchAllDesignations()
        {
            return this.context.Designations.AsEnumerable<DesignationEntity>();
        }


        public bool ChangePassword(PasswordModel passwordModel)
        {
            Profile profile = this.context.Profiles.FirstOrDefault(p => p.UserName == passwordModel.UserName && p.Password == passwordModel.OldPassword);
            if(profile != null)
            {
                profile.Password = passwordModel.NewPassword;
                this.context.Profiles.Update(profile);
                this.context.SaveChanges();
                return true;
            }
            return false;
        }
    }
}

