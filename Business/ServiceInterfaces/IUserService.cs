using ChatApp.Context.EntityClasses;
using ChatApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChatApp.Business.ServiceInterfaces
{
    public interface IUserService
    {
        Profile GetUserByUsername(string username);
        
        Task<IEnumerable<UserModel>> GetUsers();
    }
}
