using ChatApp.Business.ServiceInterfaces;
using ChatApp.Context;
using ChatApp.Context.EntityClasses;
using ChatApp.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Linq;

namespace ChatApp.Infrastructure.ServiceImplementation
{
	public class AdminService : IAdminService
	{
		private readonly ChatAppContext context;
		private readonly IProfileService profileService;
		private readonly IHubContext<chatHub> hubContext;
		public AdminService(ChatAppContext context, IProfileService profileService , IHubContext<chatHub> hubContext)
		{
			this.context = context;
			this.profileService = profileService;
			this.hubContext = hubContext;
		}
		public List<Designation> getAllDesignation(string userName)
		{
			var profile = context.Profiles.FirstOrDefault(u => u.UserName == userName);
			if (profile != null && profile.Designation > 7)
			{
				return context.Designation.Where(u => u.Id < profile.Designation).ToList();
			}
			return new List<Designation>();
		}

		public bool DeleteUser(string selUserName, string loginUserName)
		{
			var selProfile = context.Profiles.FirstOrDefault(u => u.UserName == selUserName);
			var loginProfile = context.Profiles.FirstOrDefault(u => u.UserName == loginUserName);
			if (selProfile != null && loginUserName != null && selProfile.IsDeleted == 0 && loginProfile.Designation > selProfile.Designation)
			{
				selProfile.IsDeleted = 1;
				selProfile.LastUpdatedAt= DateTime.Now;
				selProfile.LastUpdatedBy = loginProfile.Id;
				context.Profiles.Update(selProfile);
				context.SaveChanges();
				return true;
			}
			return false;
		}

		public void UpdateUser(UpdateModel updateModel, string userName)
		{
			Profile updateUser = context.Profiles.FirstOrDefault(u=>u.UserName==updateModel.UserName);
			if (updateUser == null)
			{}
			else if (updateUser.Email == context.Profiles.FirstOrDefault().Email && updateUser.UserName != userName)
			{}
			else
			{
				updateUser.FirstName = updateModel.FirstName;
				updateUser.LastName = updateModel.LastName;
				updateUser.Email = updateModel.Email;
				updateUser.LastUpdatedAt = DateTime.Now;
				updateUser.LastUpdatedBy = profileService.GetIdFromUserName(userName);
				updateUser.Designation = context.Designation.FirstOrDefault(u => u.Position == updateModel.Designation).Id;
				context.Profiles.Update(updateUser);
				context.SaveChanges();
				var response = profileService.GenerateJSONWebToken(updateUser);
				if (updateUser.UserName != userName)
				{
					var connection = context.Connections.FirstOrDefault(u => u.ProfileId == updateUser.Id);
					if (connection != null)
					{
						this.hubContext.Clients.Client(connection.SignalId).SendAsync("myProfileChanged", response);
					}
				}
			}
		}

	}
}
