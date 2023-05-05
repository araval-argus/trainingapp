using ChatApp.Models;
using System.Collections.Generic;

namespace ChatApp.Business.ServiceInterfaces
{
    public interface IEmployeeService
    {
        List<profileDTO> getAllUser();
        bool updateRole(string userName, string profileType, string updatedBy);

        bool deleteUser(string userName, string updatedBy);
        bool RegisterUser(RegisterModel regModel, string addedBy);
    }
}
