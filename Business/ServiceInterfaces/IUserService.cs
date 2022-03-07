using ChatApp.Context.EntityClasses;
using ChatApp.Models;
using ChatApp.Models.Assets;
using ChatApp.Models.Users;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChatApp.Business.ServiceInterfaces
{
    public interface IUserService
    {
        UserModel GetUserByUsername(string username);
        
        IEnumerable<UserModel> GetUsers();

        Task<UserModel> UpdateUser(UserUpdateModel user, string userName);

        AvatarModel UploadProfileImage(UserModel user, IFormFile profileImage);

        string GetUserProfileUrlFromId(int Id);

        IEnumerable<UserModel> SearchUser(string user);
    }
}
