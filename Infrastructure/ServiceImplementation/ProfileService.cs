using ChatApp.Business.Helpers;
using ChatApp.Business.ServiceInterfaces;
using ChatApp.Context;
using ChatApp.Context.EntityClasses;
using ChatApp.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Drawing;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using Microsoft.Extensions.Configuration;

namespace ChatApp.Infrastructure.ServiceImplementation
{
    public class ProfileService : IProfileService
    {
        private readonly ChatAppContext context;
        private readonly IWebHostEnvironment webHostEnvironment;

		public ProfileService(ChatAppContext context , IWebHostEnvironment webHostEnvironment )
        {
            this.context = context;          
            this.webHostEnvironment = webHostEnvironment;
        }

		public Profile CheckPassword(LoginModel model)
        {
            return this.context.Profiles.FirstOrDefault(x => model.Password == x.Password
            && (x.Email.ToLower().Trim() == model.EmailAddress.ToLower().Trim() || x.UserName.ToLower().Trim() == model.Username.ToLower().Trim()));
        }
        //Get User Method
		public Profile GetUser(Expression<Func<Profile,bool>> Filter)
		{
            return context.Profiles.FirstOrDefault(Filter);
		}

		public Profile RegisterUser(RegisterModel regModel)
        {
            Profile newUser = null;
            if (!CheckEmailOrUserNameExists(regModel.UserName, regModel.Email))
            {
                newUser = new Profile
                {
                    FirstName = regModel.FirstName,
                    LastName = regModel.LastName,
                    Password = regModel.Password,
                    UserName = regModel.UserName,
                    Email = regModel.Email,
                    Designation = "Employee",
					ImagePath = GenerateDefaultImage(),
					CreatedAt = DateTime.UtcNow,
                    ProfileType = ProfileType.User
                };
                context.Profiles.Add(newUser);
                context.SaveChanges();
            }
            return newUser;
        }

		public Profile UpdateUser(UpdateModel updateModel, string userName)
		{
            Profile updateUser = GetUser(u => u.UserName == userName);
            if (updateUser == null)
            {
                return null;
            }
			if(updateUser.Email == context.Profiles.FirstOrDefault().Email && updateUser.UserName != userName)
            {
                return null;
			}
            //To upload Profile Image
            if (updateModel.ProfileImage != null )
            {
				var filename = Guid.NewGuid().ToString(); // new generated image file name
                var uploads = Path.Combine(webHostEnvironment.WebRootPath, @"Images");
                var extension = Path.GetExtension(updateModel.ProfileImage.FileName);// Get Extension Of the File

				//if image for book is already stored then we need to delete it first
				//Tried but doesn't work || updateuser.ImagePath != "\"/images/default.png" for default image
				if (updateUser.ImagePath != null)
                {
                    var oldImagePath = Path.Combine(webHostEnvironment.WebRootPath + updateUser.ImagePath );
                    if (System.IO.File.Exists(oldImagePath))
                    {
                        System.IO.File.Delete(oldImagePath);
                    }
                }

                using (var fileStreams = new FileStream(Path.Combine(uploads, filename + extension), FileMode.Create))
                {
                    updateModel.ProfileImage.CopyTo(fileStreams);
                }

				updateUser.ImagePath ="/images/" + filename + extension;
            }
			updateUser.FirstName = updateModel.FirstName;
			updateUser.LastName = updateModel.LastName;
			updateUser.Email = updateModel.Email;
			updateUser.Designation = updateModel.Designation;
			updateUser.LastUpdatedAt= DateTime.Now;

			context.Profiles.Update(updateUser);
            context.SaveChanges();
            return updateUser;
		}

        private string GenerateDefaultImage()
        {
			string fileName = Guid.NewGuid().ToString() + ".png";
			var defaultLocation = Path.Combine(webHostEnvironment.WebRootPath, @"Images\default.png");
			var NewLocation = Path.Combine(webHostEnvironment.WebRootPath, @"Images\", fileName);
			File.Copy(defaultLocation, NewLocation,true);
			return "/images/"+ fileName ;
		}

		private bool CheckEmailOrUserNameExists(string userName, string email)
        {
            return context.Profiles.Any(x => x.Email.ToLower().Trim() == email.ToLower().Trim() || x.UserName.ToLower().Trim() == userName.ToLower().Trim());
        }

		public int GetIdFromUserName(string userName)
		{
			Profile user = context.Profiles.FirstOrDefault(profile => profile.UserName == userName);
			return user.Id;
		}

		public string GetUserNameFromId(int id)
		{
			Profile user = context.Profiles.FirstOrDefault(profile => profile.Id == id);
			return user.UserName;
		}

	}
}
