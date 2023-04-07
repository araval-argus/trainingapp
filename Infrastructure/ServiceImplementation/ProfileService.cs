﻿using ChatApp.Business.Helpers;
using ChatApp.Business.ServiceInterfaces;
using ChatApp.Context;
using ChatApp.Context.EntityClasses;
using ChatApp.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

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

        public Profile UpdateUser(UpdateModel updateModel, string username)
        {
            Profile profile = getUser(username);
            // If user not found
            if(profile == null)
            {
                return null;
            }
            // If entered Email is already in use 
            if (CheckEmailExists(updateModel.Email, username))
            {
                return new Profile();
            }
            profile.FirstName = updateModel.FirstName;
            profile.LastName = updateModel.LastName;
            profile.Email = updateModel.Email;
            if(updateModel.file!= null)
            {
                var file = updateModel.file;
                string rootPath = _webHostEnvironment.WebRootPath;
                string fileName = Guid.NewGuid().ToString();
                var extension = Path.GetExtension(file.FileName);
                var pathToSave = Path.Combine(rootPath, @"images");
           
                //delete old file
                if (profile.imagePath != null)
                {
                    var currFile = Path.Combine(pathToSave, profile.imagePath);
                    
                    if(File.Exists(currFile))
                    {
                        File.Delete(currFile);
                    }
                }
                var fullFile = fileName+extension;
                var dbPath = Path.Combine(pathToSave, fullFile);
                using (var fileStreams = new FileStream(dbPath, FileMode.Create))
                {
                    file.CopyTo(fileStreams);
                }

                profile.imagePath = fullFile;
            }
            context.Profiles.Update(profile);
            context.SaveChanges();
            return profile;
        }


        public string GetImage(string username)
        {
            Profile profile = getUser(username);
            if(profile == null)
            {
                return null;
            }
            return profile.imagePath;
        }

        public List<profileDTO> getAll()
        {
            List<Profile> profiles =  context.Profiles.ToList();
            List<profileDTO> profileDTOs = Mapper.profileMapper(profiles);
            return profileDTOs;
        }

        public List<profileDTO> GetProfileDTOs(string s)
        {
            List<Profile> profiles = context.Profiles.Where(e => e.FirstName.Contains(s) || e.LastName.Contains(s)).ToList();
            List<profileDTO> profileDTOs = Mapper.profileMapper(profiles);
            return profileDTOs;
        }
        private Profile getUser(string username)
        {
            return context.Profiles.FirstOrDefault(x => x.UserName == username);
        }

        private bool CheckEmailExists(string email, string username) 
        {
            return context.Profiles.Any(e => e.Email == email && e.UserName != username);
        }

        private bool CheckEmailOrUserNameExists(string userName, string email)
        {
            return context.Profiles.Any(x => x.Email.ToLower().Trim() == email.ToLower().Trim() || x.UserName.ToLower().Trim() == userName.ToLower().Trim());
        }
    }
}
