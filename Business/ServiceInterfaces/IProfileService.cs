using ChatApp.Context.EntityClasses;
using ChatApp.Models;
using System;
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
    }
}
