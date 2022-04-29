using ChatApp.Business.ServiceInterfaces;
using ChatApp.Context;
using ChatApp.Models;
using ChatApp.Models.Assets;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using ChatApp.Business.Helpers;
using ChatApp.Context.EntityClasses;

namespace ChatApp.Infrastructure.ServiceImplementation
{
    public class AvatarService : IAvatarService
    {
        private readonly ChatAppContext context;

        public AvatarService(ChatAppContext context)
        {
            this.context = context;
        }


        public AvatarModel SaveProfileImage(UserModel user, IFormFile profileImage)
        {

            // saving file with the name [username]_profile
            var profileFolder = FolderPaths.PathToProfileFolder;
            var pathToSave = Path.Combine(Directory.GetCurrentDirectory(), profileFolder);

            // save image to server
            var fileName = ContentDispositionHeaderValue.Parse(profileImage.ContentDisposition).FileName.Trim('"');
            var fileExtension = Path.GetExtension(fileName);
            var fileSize = profileImage.Length;
            var fileSaveName = user.UserName + "_" + "profile";

            var fullFileSaveName = fileSaveName + fileExtension;
            var fullPath = Path.Combine(pathToSave, fullFileSaveName);

            var dbPath = Path.Combine(profileFolder, fullFileSaveName);

            using (var stream = new FileStream(fullPath, FileMode.Create))
            {
                profileImage.CopyTo(stream);
            }

            // making entry in database

            // check if there is already a entry for the profile for this user
            Avatar asset = context.Avatars.FirstOrDefault(x => x.ProfileId == user.Id);
            Avatar returnAvatar;
            if ( asset == null)
            {
                // create a new entry
                //asset = 
                returnAvatar = new Avatar
                {
                    ProfileId = user.Id,
                    FileName = fileSaveName,
                    FileExtension = fileExtension,
                    FileSize = fileSize,
                    //FileType = "profile",
                    FilePath = dbPath,
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now
                };
                

                context.Avatars.Add(returnAvatar);
            }
            else
            {
                // update the existing entry
                // no need to update the filename or path as file will be overidden by the same name
                asset.UpdatedAt = DateTime.Now;
                asset.FileExtension = fileExtension;
                asset.FileSize = fileSize;

                returnAvatar = asset;
            }
                context.SaveChanges();

            return ConvertHelpers.ConvetAssetToAssetModel(returnAvatar);
        }

    }
}
