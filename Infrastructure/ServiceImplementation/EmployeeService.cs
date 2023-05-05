using ChatApp.Business.Helpers;
using ChatApp.Business.ServiceInterfaces;
using ChatApp.Context;
using ChatApp.Context.EntityClasses;
using ChatApp.Hubs;
using ChatApp.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace ChatApp.Infrastructure.ServiceImplementation
{
    public class EmployeeService : IEmployeeService
    {
        private readonly ChatAppContext context;
        private readonly IHubContext<ChatHub> _hubContext;
        private readonly IGroupService _groupService;
        private IConfiguration _config;


        public EmployeeService(ChatAppContext context, IHubContext<ChatHub> hubContext, IGroupService groupService, IConfiguration config)
        {
            this.context = context;
            this._hubContext = hubContext;
            this._groupService = groupService;
            this._config = config;
        }
        public List<profileDTO> getAllUser()
        {
            List<Profile> profiles = context.Profiles.AsNoTracking().Where(e => e.isDeleted == 0).ToListAsync().GetAwaiter().GetResult();
            List<profileDTO> profileDTOs = Mapper.profilesMapper(profiles);
            return profileDTOs;  
        }

        public bool updateRole(string user, string profileType, string updatedBy)
        {
            bool isParsed = int.TryParse(profileType, out int profileTypeId);
            Profile profile = context.Profiles.Include(e => e.Designation).FirstOrDefault(e => e.UserName == user && e.isDeleted == 0);
            if(profile == null || profileTypeId < 3 || profileTypeId > 8 || !isParsed)
            {
                return false;
            }
            Profile updateByProfile = context.Profiles.FirstOrDefault(e => e.UserName == updatedBy && e.isDeleted == 0);
            profile.LastUpdatedAt= DateTime.Now;
            profile.LastUpdatedBy = updateByProfile.Id;
            profile.ProfileType = profileTypeId;
            context.Profiles.Update(profile);
            context.SaveChanges();
            Connections connection = context.Connections.AsNoTracking().FirstOrDefault(e => e.User == profile.Id);
            if (connection != null)
            {
                profile.Designation = context.ProfileType.AsNoTracking().FirstOrDefault(e => e.Id == profile.ProfileType);
                _hubContext.Clients.AllExcept(connection.ConnectionId).SendAsync("userUpdated", profile.UserName, profile.ProfileType);
                _hubContext.Clients.Client(connection.ConnectionId).SendAsync("loggedInUserUpdated", GenerateJSONWebToken(profile));
            }
            else
            {
                _hubContext.Clients.All.SendAsync("userUpdated", profile.UserName, profile.ProfileType);
            }

            return true;
        }

        public bool deleteUser(string userName, string updatedBy)
        {
            Profile deletingProfile = context.Profiles.FirstOrDefault(e => e.UserName == userName && e.isDeleted == 0);
            if(deletingProfile == null || deletingProfile.ProfileType < 4)
            {
                return false;
            }
            List<string> groupName = context.GroupMember.AsNoTracking().Include(e => e.Group).Where(e => e.MemberId == deletingProfile.Id).Select(e => e.Group.Name).ToList();
            IEnumerable<GroupMember> groupMember = context.GroupMember.Where(e => e.MemberId == deletingProfile.Id);
            context.GroupMember.RemoveRange(groupMember);
            foreach(string group in groupName)
            {
                _groupService.leaveGroup(deletingProfile.UserName, group);
            }
            IEnumerable<Chat> chats = context.Chats.Where(e => e.From == deletingProfile.Id || e.To == deletingProfile.Id);
            context.Chats.RemoveRange(chats);
            Connections connection = context.Connections.AsNoTracking().FirstOrDefault(e => e.User == deletingProfile.Id);
            if(connection != null)
            {
                _hubContext.Clients.AllExcept(connection.ConnectionId).SendAsync("userDeleted", userName);
                _hubContext.Clients.Client(connection.ConnectionId).SendAsync("loggedInUserDeleted");
            }
            else
            {
                _hubContext.Clients.All.SendAsync("userDeleted", userName);
            }
            Profile updateByProfile = context.Profiles.FirstOrDefault(e => e.UserName == updatedBy && e.isDeleted == 0);
            deletingProfile.LastUpdatedAt = DateTime.Now;
            deletingProfile.LastUpdatedBy = updateByProfile.Id;
            deletingProfile.isDeleted = 1;
            context.SaveChanges();
            return true;
        }

        public bool RegisterUser(RegisterModel regModel, string addedBy)
        {
            Profile addBy = context.Profiles.AsNoTracking().FirstOrDefault(e => e.UserName == addedBy && e.isDeleted == 0);
            Profile newUser = null;
            if (!CheckEmailOrUserNameExists(regModel.UserName, regModel.Email))
            {
                //generating salt
                byte[] bArray;
                new RNGCryptoServiceProvider().GetBytes(bArray = new byte[32]);
                string salt = Convert.ToHexString(bArray);
                var hashPassword = getHash(regModel.Password, salt);
                newUser = new Profile
                {
                    FirstName = regModel.FirstName,
                    LastName = regModel.LastName,
                    Password = hashPassword,
                    UserName = regModel.UserName,
                    Email = regModel.Email,
                    CreatedAt = DateTime.Now,
                    ProfileType = regModel.Type,
                    Status = 6,
                    CreatedBy = addBy.Id,
                };
                context.Profiles.Add(newUser);
                context.SaveChanges();

                //saving salt
                context.Salt.Add(new Salt { UserId = newUser.Id, HashSalt = salt });
                context.SaveChanges();
                newUser.Designation = context.ProfileType.AsNoTracking().FirstOrDefault(e => e.Id == regModel.Type);
                _hubContext.Clients.All.SendAsync("newUserAdded", newUser);
                return true;
            }
            return false;
        }

        private bool CheckEmailOrUserNameExists(string userName, string email)
        {
            return context.Profiles.Any(x => x.Email.ToLower().Trim() == email.ToLower().Trim() || x.UserName.ToLower().Trim() == userName.ToLower().Trim());
        }

        private string getHash(string password, string salt)
        {
            SHA256 hash = SHA256.Create();
            var passwordByte = Encoding.Default.GetBytes(password + salt);
            var hashedPassword = hash.ComputeHash(passwordByte);
            return Convert.ToHexString(hashedPassword);
        }

        private string GenerateJSONWebToken(Profile profileInfo)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new[] {
                    new Claim(JwtRegisteredClaimNames.Sub, profileInfo.UserName),
                    new Claim(JwtRegisteredClaimNames.Email, profileInfo.Email),
                    new Claim(ClaimsConstant.FirstNameClaim, profileInfo.FirstName),
                    new Claim(ClaimsConstant.LastNameClaim, profileInfo.LastName),
                    new Claim(ClaimsConstant.DesingnationClaim, profileInfo.Designation.Designation),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
                    };

            var token = new JwtSecurityToken(_config["Jwt:Issuer"],
            _config["Jwt:Issuer"],
            claims,
            expires: DateTime.Now.AddMinutes(240),
            signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

    }
}
