using ChatApp.Context.EntityClasses;
using ChatApp.Models;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace ChatApp.Business.ServiceInterfaces
{
    public interface IProfileService
    {
        string CheckPassword(LoginModel loginModel);

        string RegisterUser(RegisterModel regModel);

        string UpdateUser(UpdateModel regModel, string username );

        Profile GetUser(string Filter);

        public string GetUserNameFromId(int id);

        public int GetIdFromUserName(string userName);

        public string GetDesignationFromId(int Id);

        public string GetStatusFromId(int Id);

        public void ChangeStatus(string userName, int status);

        public List<UserStatus> GetAllStatus();

        public UserStatus getUserStatus(string userName);

        public List<SelectedUserModel> getAllUsers(string userName);

        public bool CheckDeleted(string userName);

        public string GenerateJSONWebToken(Profile profileInfo);

        public bool ChangePassword(string userName, ChangePasswordModel password);

	}
}
