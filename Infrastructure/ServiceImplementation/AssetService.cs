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
    public class AssetService : IAssetService
    {
        private readonly ChatAppContext context;

        public AssetService(ChatAppContext context)
        {
            this.context = context;
        }


        public AssetModel SaveProfileImage(UserModel user, IFormFile profileImage)
        {
            // no validation done here
            // should be done before calling the function

            // tasks done here
            // entry in db for the asset
            // actual saving of the file


            // saving file with the name [username]_profile
            var profileFolder = FolderPaths.PathToProfileFolder;
            var pathToSave = Path.Combine(Directory.GetCurrentDirectory(), profileFolder);

            //if (profileImage.Length == 0)
            //{
            //    return null;
            //}

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
            Asset asset = context.Assets.FirstOrDefault(x => x.Id == user.Id);
            Asset returnAsset;
            if ( asset == null)
            {
                // create a new entry
                //asset = 
                returnAsset = new Asset
                {
                    ProfileId = user.Id,
                    FileName = fileSaveName,
                    FileExtension = fileExtension,
                    FileSize = fileSize,
                    FileType = "profile",
                    FilePath = dbPath,
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now
                };
                

                context.Assets.Add(returnAsset);
            }
            else
            {
                // update the existing entry
                // no need to update the filename or path as file will be overidden by the same name
                asset.UpdatedAt = DateTime.Now;
                asset.FileExtension = fileExtension;
                asset.FileSize = fileSize;

                returnAsset = asset;
            }
                context.SaveChanges();

            return ConvertHelpers.ConvetAssetToAssetModel(returnAsset);
        }

    }
}
