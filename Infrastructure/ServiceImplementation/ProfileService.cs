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
using Microsoft.AspNetCore.SignalR;
using ChatApp.Hubs;

namespace ChatApp.Infrastructure.ServiceImplementation
{
    public class ProfileService : IProfileService
    {
        private readonly ChatAppContext context;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IHubContext<ChatHub> hubContext;

        public ProfileService(ChatAppContext context, IWebHostEnvironment webHostEnvironment, IHubContext<ChatHub> hubContext, IChatService chatService)
        {
            this.context = context;
            this._webHostEnvironment = webHostEnvironment;
            this.hubContext = hubContext;
        }

        // this method checks (username and password) or (email and password)
        public Profile CheckPassword(LoginModel model)
        {
            return this.context.Profiles.Include("Designation").Include("Status").FirstOrDefault(x => x.IsActive && (model.Password == x.Password
            && (x.Email.ToLower().Trim() == model.EmailAddress.ToLower().Trim() || x.UserName.ToLower().Trim() == model.Username.ToLower().Trim())));
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
                    DesignationID = regModel.DesignationID,
                    StatusID = 1,
                    IsActive = true
                };                
                context.Profiles.Add(newUser);
                context.SaveChanges();

                newUser = context.Profiles.Include("Designation").Include("Status").FirstOrDefault(p => p.IsActive && p.UserName == regModel.UserName);
            }
            return newUser;
        }

        public Profile UpdateProfile(UpdateModel updateModel, string username, bool updateImage = false)
        {
            Profile user = FetchProfileFromUserName(username);
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
            return context.Profiles.Any(x => x.IsActive && (x.Email.ToLower().Trim() == email.ToLower().Trim() || x.UserName.ToLower().Trim() == userName.ToLower().Trim()));
        }

        public Profile FetchProfileFromUserName(string userName)
        {
            Profile user = null;
            if (!string.IsNullOrEmpty(userName))
            {
                user = this.context.Profiles.Include("Designation").Include("Status").FirstOrDefault(u => u.IsActive && u.UserName.Trim() == userName.Trim());
            }
            return user;
        }

        public bool CheckUserNameExists(string? userName = null)
        {
            if (userName == null)
            {
                return false;
            }
            return context.Profiles.Any(p => p.IsActive && p.UserName.Trim() == userName.Trim());
        }

        private string SetDefaultImage()
        {
            string fileName = Guid.NewGuid().ToString() + ".jpg";

            var DefaultFileLoc = Path.Combine(_webHostEnvironment.WebRootPath, @"Default/default_profile.jpg");
            var FileLoc = Path.Combine(DefaultFileLoc, @"../../Images/Users", fileName);

            File.Copy(DefaultFileLoc, FileLoc);

            return fileName;
        }


        public int FetchIdFromUserName(string userName)
        {
            if (string.IsNullOrEmpty(userName))
            {
                return 0;
            }
            Profile profile =  this.context.Profiles.FirstOrDefault(
                profile => profile.IsActive && profile.UserName.Equals(userName));
            if(profile == null)
            {
                return 0;
            }
            return profile.Id;
        }

        public string FetchUserNameFromId(int id)
        {
            return this.context.Profiles.FirstOrDefault(
                profile => profile.IsActive && profile.Id == id
                ).UserName;
        }

        //This method will return a list of users who have interacted with the logged in user
        public IEnumerable<UserModel> FetchAllUsers(int userId)
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
            foreach (int id in Ids)
            {
                var profile = this.context.Profiles.Include("Designation").Include("Status").FirstOrDefault(p => p.IsActive && p.Id == id);
                if (profile != null)
                {
                    friendsProfiles.Add(profile);
                }
            }

            return friendsProfiles
                .Select(
                profile =>
                {

                    //for counting of unread messages:-
                    MessageEntity lastMessage = this.context.Messages.OrderBy(m => m.Id).LastOrDefault(m => (m.SenderID == profile.Id && m.RecieverID == userId) || (m.RecieverID == profile.Id && m.SenderID == userId));

                    return new UserModel()
                    {
                        UserName = profile.UserName,
                        Designation = profile.Designation,
                        Email = profile.Email,
                        FirstName = profile.FirstName,
                        LastName = profile.LastName,
                        ImageUrl = profile.ImageUrl,
                        LastMessage = lastMessage.Message,
                        LastMessageTimeStamp = lastMessage.CreatedAt,
                        UnreadMessageCount = UnreadMessageCount(profile.Id, userId),
                        Status = profile.Status
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
            Profile profile = this.context.Profiles.FirstOrDefault(p => p.IsActive && p.UserName == passwordModel.UserName && p.Password == passwordModel.OldPassword);
            if (profile != null)
            {
                profile.Password = passwordModel.NewPassword;
                this.context.Profiles.Update(profile);
                this.context.SaveChanges();
                return true;
            }
            return false;
        }


        public IEnumerable<Profile> FetchAllProfiles()
        {
            var employees = this.context.Profiles.Include("Designation").Include("Status").Where(p => p.IsActive);
            return employees;
        }

        public void DeleteProfile(Profile profile)
        {
            profile.IsActive = false;
            this.context.Profiles.Update(profile);
            this.context.SaveChanges();
        }

        public void UpdateEmployeeProfile(Profile employee)
        {
            this.context.Profiles.Update(employee);
            this.context.SaveChanges();
        }

        //fetches all users except the user with given username
        public IEnumerable<UserModel> FetchAllUsers(string userName)
        {
            var users = this.context.Profiles.Include("Designation").Include("Status")
                .Where(profile => profile.UserName != userName)
                .Select(profile => new UserModel
                {
                    UserName = profile.UserName,
                    FirstName = profile.FirstName,
                    LastName = profile.LastName,
                    Email = profile.Email,
                    ImageUrl = profile.ImageUrl,
                    Designation = profile.Designation,
                    Status = profile.Status,
                });
            return users;
        }

       public void ChangeStatus(Profile user, int statusId)
        {
            user.StatusID = statusId;
            this.context.Update(user);
            this.context.SaveChanges();
        }


        public IList<Status> FetchAllStatus()
        {
            IList<Status> allStatus= new List<Status>();
            allStatus = this.context.Status.ToList();
            return allStatus;
        }

        public Status FetchStatus(int userId)
        {
            Status statusToBeReturned = this.context.Profiles.Include("Status").FirstOrDefault(p => p.Id == userId).Status;
            return statusToBeReturned;
        }
    }
}

