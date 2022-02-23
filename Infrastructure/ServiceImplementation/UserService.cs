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
        private readonly IAssetService assetService;

        public UserService(ChatAppContext context, IAssetService assetService)
        {
            this.context = context;
            this.assetService = assetService;
        }

        public UserModel GetUserByUsername(string username)
        {
            Profile user = this.context.Profiles.FirstOrDefault((user) => String.Equals(user.UserName, username));

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
            IEnumerable<Profile> users = context.Profiles.ToList();
            IEnumerable<UserModel> safeObject = (IEnumerable<UserModel>)ConvertHelpers.ConvertToSafeUserObjects(users);

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

            UserModel safeObj = ConvertHelpers.ConvertToSafeUserObjects(updateObj);

            return safeObj;

        }


        public AssetModel UploadProfileImage(UserModel user, IFormFile profileImage) {


            AssetModel returnAsset = assetService.SaveProfileImage(user, profileImage);

            
            return returnAsset;
            

        }

        public string GetUserProfileUrlFromId(int Id)
        {
            Asset asset = context.Assets.FirstOrDefault(x => x.ProfileId == Id);

            return asset.FilePath;
        }

    }
}
