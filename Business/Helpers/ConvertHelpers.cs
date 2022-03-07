using ChatApp.Context.EntityClasses;
using ChatApp.Models;
using ChatApp.Models.Assets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChatApp.Business.Helpers
{
    public class ConvertHelpers
    {
        // converts the profile object so that password and profile type were not sent to the client side
        public static UserModel ConvertToSafeUserObjects(Profile profile)
        {
            UserModel userModel = new();

            userModel.Id = profile.Id;
            userModel.FirstName = profile.FirstName;
            userModel.LastName = profile.LastName;
            userModel.UserName = profile.UserName;
            userModel.Email = profile.Email;
            userModel.CreatedAt = profile.CreatedAt;
            userModel.CreatedBy = profile.CreatedBy;
            userModel.LastUpdatedAt = profile.LastUpdatedAt;
            userModel.LastUpdatedBy = profile.LastUpdatedBy;

            if (profile.Avatar != null)
            {
                userModel.ProfileUrl = profile.Avatar.FilePath;
            }

            return userModel;
        }

        // converts the profile object so that password and profile type were not sent to the client side
        public static IEnumerable<UserModel> ConvertToSafeUserObjects(IEnumerable<Profile> profiles)
        {
            IEnumerable<UserModel> userModels = Enumerable.Empty<UserModel>();

            for (int i = 0; i < profiles.Count(); i++)
            {
                UserModel user = ConvertToSafeUserObjects(profiles.ElementAt(i));
                userModels = userModels.Append(user);
            }

            return userModels;
        }

        // converts asset type object to asset model
        public static AvatarModel ConvetAssetToAssetModel(Avatar avatar)
        {
            return new AvatarModel
            {
                Id = avatar.Id,
                ProfileId = avatar.ProfileId,
                FileName = avatar.FileName,
                FileExtension = avatar.FileExtension,
                FilePath = avatar.FilePath,
                FileSize = avatar.FileSize,
                //FileType = asset.FileType,
                CreatedAt = avatar.CreatedAt,
                UpdatedAt = avatar.UpdatedAt
            };
        }
    }
}
