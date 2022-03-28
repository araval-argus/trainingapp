using ChatApp.Business.ServiceInterfaces;
using ChatApp.Context;
using ChatApp.Context.EntityClasses;
using ChatApp.Models;
using ChatApp.Models.Assets;
using ChatApp.Models.Users;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using ChatApp.Business.Helpers;

namespace ChatApp.Infrastructure.ServiceImplementation
{
    public class UserService : IUserService
    {
        private readonly ChatAppContext context;
        private readonly IAvatarService assetService;

        public UserService(ChatAppContext context, IAvatarService assetService)
        {
            this.context = context;
            this.assetService = assetService;
        }

        public UserModel GetUserByUsername(string username)
        {
            Profile user = this.context.Profiles.Include(p => p.Avatar).FirstOrDefault((user) => String.Equals(user.UserName, username));

            // if not found
            if (user == null)
            {
                return null;
            }

            UserModel safeObj = ConvertHelpers.ConvertToSafeUserObjects(user);

            return safeObj;
        }

        public IEnumerable<UserModel> GetUsers()
        {
            IEnumerable<Profile> users = context.Profiles.Include(p => p.Avatar).ToList();
            IEnumerable<UserModel> safeObject = (IEnumerable<UserModel>)ConvertHelpers.ConvertToSafeUserObjects(users);

            return safeObject;
        }

        public string GetUserNameFromUserId(int id)
        {
            var username = context.Profiles.FirstOrDefault(u => u.Id == id); 

            if ( username == null)
            {
                return "";
            }

            return username.UserName;
        }

        public int GetUserIdFromUserName(string username)
        {
            var user = context.Profiles.FirstOrDefault(u => u.UserName == username);

            if (user == null)
            {
                return 0;
            }

            return user.Id;
        }

        public async Task<UserModel> UpdateUser(UserUpdateModel user, string userName)
        {
            Profile updateObj = context.Profiles.Include(p => p.Avatar).FirstOrDefault((u) => String.Equals(u.UserName, userName));

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
            updateObj.StatusText = user.StatusText;

            // temporary -- will be updated after profile type will be implemented
            updateObj.LastUpdatedBy = updateObj.Id;

            _ = await context.SaveChangesAsync();

            UserModel safeObj = ConvertHelpers.ConvertToSafeUserObjects(updateObj);

            return safeObj;

        }

        public UserModel GetUserFromUserId(int userId)
        {
            var user = context.Profiles.Include(p => p.Avatar).FirstOrDefault(u => u.Id == userId);

            return ConvertHelpers.ConvertToSafeUserObjects(user);
        }

        public AvatarModel UploadProfileImage(UserModel user, IFormFile profileImage)
        {
            AvatarModel returnAsset = assetService.SaveProfileImage(user, profileImage);
            return returnAsset;
        }

        public string GetUserProfileUrlFromId(int Id)
        {
            Avatar asset = context.Avatars.FirstOrDefault(x => x.ProfileId == Id);

            if (asset == null)
            {
                return "";
            }

            return asset.FilePath;
        }

        public IEnumerable<UserModel> SearchUser(string user)
        {
            

            IEnumerable<Profile> allUsers = context.Profiles
                                        .Include(p => p.Avatar)
                                        .Where(p => p.UserName.Contains(user) 
                                            || p.FirstName.Contains(user) 
                                            || p.LastName.Contains(user) 
                                            || p.Email.Contains(user)).ToList();

            return ConvertHelpers.ConvertToSafeUserObjects(allUsers);

        }
    }
}
