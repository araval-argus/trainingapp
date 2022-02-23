using ChatApp.Models;
using ChatApp.Models.Assets;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChatApp.Business.ServiceInterfaces
{
    public interface IAssetService
    {
        AssetModel SaveProfileImage(UserModel user, IFormFile profileImage);
    }
}
