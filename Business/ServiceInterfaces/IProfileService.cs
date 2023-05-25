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
        Profile FetchProfileFromUserName(string UserName);

        string FetchIdFromUserName(string userName);
        string FetchUserNameFromId(string id);

        IEnumerable<UserModel> FetchAllInteractedUsers(string id);

        IEnumerable<DesignationEntity> FetchAllDesignations();

        IEnumerable<Profile> FetchAllProfiles();

        void DeleteProfile(Profile profile);

        void UpdateEmployeeProfile(Profile employee);

        IEnumerable<UserModel> FetchAllUsers(string userName);

        void ChangeStatus(Profile user, int statusId);

        IList<Status> FetchAllStatus();

        Status FetchStatus(string userId);
    }
}
