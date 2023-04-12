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

        Profile UpdateUser(UpdateModel updateModel, string userName);

        string GetImage(int userId);

        List<profileDTO> getAll();

        List<profileDTO> GetProfileDTOs(string s, string user);

        Profile getUser(int userName);
        Profile getUserFromUserName(string userName);
    }
}
