using ChatApp.Context.EntityClasses;
using ChatApp.Models;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChatApp.Business.ServiceInterfaces
{
    public interface IProfileService
    {
        Profile CheckPassword(LoginModel loginModel);

        Profile RegisterUser(RegisterModel regModel);

        Profile FetchProfile(string UserName);

        Profile UpdateProfile(UpdateModel updateModel, string username, bool updateImage = false);

        bool CheckUserNameExists(string? userName);

        int FetchIdFromUserName(string userName);
        string FetchUserNameFromId(int id);

        IEnumerable<FriendProfileModel> FetchAllUsers(int id);

    }
}
