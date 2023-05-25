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
using Microsoft.AspNetCore.Identity;

namespace ChatApp.Infrastructure.ServiceImplementation
{
    public class ProfileService : IProfileService
    {
        private readonly ChatAppContext context;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public ProfileService(ChatAppContext context, 
            IWebHostEnvironment webHostEnvironment )
        {
            this.context = context;
            this._webHostEnvironment = webHostEnvironment;
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

        public string FetchIdFromUserName(string userName)
        {
            if (string.IsNullOrEmpty(userName))
            {
                return null;
            }
            Profile profile =  this.context.Profiles.FirstOrDefault(
                profile => profile.IsActive && profile.UserName.Equals(userName));
            if(profile == null)
            {
                return null;
            }
            return profile.Id;
        }

        public string FetchUserNameFromId(string id)
        {
            return this.context.Profiles.FirstOrDefault(
                profile => profile.IsActive && profile.Id == id
                ).UserName;
        }

        //This method will return a list of users who have interacted with the logged in user
        public IEnumerable<UserModel> FetchAllInteractedUsers(string userId)
        {

            // Fetch distinct user ids with whome the user with 'userId' had interacted
            IEnumerable<string> Ids = this.context.Messages.Where(
                m => m.SenderID == userId || m.RecieverID == userId
                )
                .Select(
                    m => m.SenderID == userId ? m.RecieverID : m.SenderID
                ).Distinct();

            IList<Profile> friendsProfiles = new List<Profile>();

            //for fetching the profiles
            foreach (string id in Ids)
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

        public int UnreadMessageCount(string senderID, string recieverID)
        {
            return this.context.Messages.Where(message =>
                        (message.SenderID == senderID && message.RecieverID == recieverID && !message.IsSeen)
                     ).Count();
        }

        public IEnumerable<DesignationEntity> FetchAllDesignations()
        {
            return this.context.Designations.AsEnumerable<DesignationEntity>();
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

        public Status FetchStatus(string userId)
        {
            Status statusToBeReturned = this.context.Profiles.Include("Status").FirstOrDefault(p => p.Id == userId).Status;
            return statusToBeReturned;
        }

        
    }
}

