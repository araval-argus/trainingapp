using ChatApp.Business.Helpers;
using ChatApp.Business.ServiceInterfaces;
using ChatApp.Context;
using ChatApp.Context.EntityClasses;
using ChatApp.Hubs;
using ChatApp.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace ChatApp.Infrastructure.ServiceImplementation
{
    public class ProfileService : IProfileService
    {
        private readonly ChatAppContext context;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IHubContext<ChatHub> _hubContext;

        public ProfileService(ChatAppContext context, IWebHostEnvironment webHostEnvironment, IHubContext<ChatHub> hubContext)
        {
            this.context = context;
            this._webHostEnvironment = webHostEnvironment;
            this._hubContext = hubContext; 
        }
        public Profile CheckPassword(LoginModel model)
        {
            Profile profile = context.Profiles.FirstOrDefault(e => (e.UserName == model.Username || e.Email == model.EmailAddress) && e.isDeleted == 0);
            if (profile == null)
            {
                return null;
            }
            string salt = context.Salt.AsNoTracking().FirstOrDefault(e => e.UserId == profile.Id).HashSalt;
            string hashedPWD = getHash(model.Password, salt);
            if(hashedPWD == profile.Password)
            {
                profile.Designation = context.ProfileType.AsNoTracking().FirstOrDefault(e => e.Id == profile.ProfileType);
                profile.Status = 1;
                context.Profiles.Update(profile);
                return profile;
            }
            return null;
        }

        public Profile RegisterUser(RegisterModel regModel)
        {
            Profile newUser = null;
            if (!CheckEmailOrUserNameExists(regModel.UserName, regModel.Email))
            {
                //generating salt
                byte[] bArray;
                new RNGCryptoServiceProvider().GetBytes(bArray = new byte[32]);
                string salt = Convert.ToHexString(bArray);
                var hashPassword = getHash(regModel.Password, salt);
                newUser = new Profile
                {
                    FirstName = regModel.FirstName,
                    LastName = regModel.LastName,
                    Password = hashPassword,
                    UserName = regModel.UserName,
                    Email = regModel.Email,
                    CreatedAt = DateTime.Now,
                    ProfileType = regModel.Type,
                    Status = 1
                };
                context.Profiles.Add(newUser);
                context.SaveChanges();

                //saving salt
                context.Salt.Add(new Salt { UserId = newUser.Id, HashSalt = salt });
                context.SaveChanges();
                newUser.Designation = context.ProfileType.AsNoTracking().FirstOrDefault(e => e.Id == regModel.Type);
            }
            return newUser;
        }

        public Profile UpdateUser(UpdateModel updateModel, string username)
        {
            Profile profile = getUserFromUserName(username);
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
            profile.Designation = context.ProfileType.AsNoTracking().FirstOrDefault(e => e.Id == profile.ProfileType);
            return profile;
        }


        public string GetImage(int userId)
        {
            Profile profile = getUser(userId);
            if(profile == null)
            {
                return null;
            }
            return profile.imagePath;
        }

        public List<profileDTO> getAll()
        {
            List<Profile> profiles =  context.Profiles.Where(e => e.isDeleted == 0).ToList();
            List<profileDTO> profileDTOs = Mapper.profilesMapper(profiles);
            return profileDTOs;
        }

        public Profile getUserFromUserName(string userName)
        {
            return context.Profiles.FirstOrDefault(x => x.UserName == userName && x.isDeleted == 0);
        }
        public Profile getUser(int userId)
        {
            return context.Profiles.FirstOrDefault(x => x.Id == userId && x.isDeleted == 0);
        }

        public List<profileDTO> GetProfileDTOs(string s, string user)
        {
            List<Profile> profiles = context.Profiles.Where(e => (e.FirstName.Contains(s) || e.LastName.Contains(s)) && e.UserName != user && e.isDeleted == 0).ToList();
            List<profileDTO> profileDTOs = Mapper.profilesMapper(profiles);
            return profileDTOs;
        }

        private bool CheckEmailExists(string email, string userName) 
        {
            return context.Profiles.Any(e => e.Email == email && e.UserName != userName);
        }

        private bool CheckEmailOrUserNameExists(string userName, string email)
        {
            return context.Profiles.Any(x => x.Email.ToLower().Trim() == email.ToLower().Trim() || x.UserName.ToLower().Trim() == userName.ToLower().Trim());
        }

        public bool setStatus(string userName, int id)
        {
            Profile profile = context.Profiles.FirstOrDefault(p => p.UserName == userName && p.isDeleted == 0);
            if(profile != null)
            {
                profile.Status = id;
                context.Profiles.Update(profile);
                context.SaveChanges();
                _hubContext.Clients.All.SendAsync("userStatusUpdated", userName, id);
                return true;
            }
            return false;
        }


        public bool changePassword(ChangePasswordModel newPasswordModel, string userName)
        {
            Profile profile = context.Profiles.FirstOrDefault(e => e.UserName == userName && e.isDeleted == 0);
                if(profile != null)
            {
                string salt = context.Salt.AsNoTracking().FirstOrDefault(e => e.UserId == profile.Id).HashSalt;
                string newHash = getHash(newPasswordModel.OldPassword, salt);
                if(newHash == profile.Password)
                {
                    var hashPassword = getHash(newPasswordModel.NewPassword, salt);
                    profile.Password = hashPassword;
                    context.Profiles.Update(profile);
                    context.SaveChanges();
                    return true;
                }
            }
            return false;
        }


        public List<bool> checkRoles()
        {
            List<bool> roles = new List<bool>
            {
                context.Profiles.AsNoTracking().Any(e => e.ProfileType == 2 && e.isDeleted == 0),
                context.Profiles.AsNoTracking().Any(e => e.ProfileType == 3 && e.isDeleted == 0)
            };
            return roles;
        }
        private string getHash(string password, string salt)
        {
            SHA256 hash = SHA256.Create();
            var passwordByte = Encoding.Default.GetBytes(password + salt);
            var hashedPassword = hash.ComputeHash(passwordByte);
            return Convert.ToHexString(hashedPassword);
        }
    }
}
