using ChatApp.Context.EntityClasses;
using ChatApp.Models;
using System.Collections.Generic;

namespace ChatApp.Business.Helpers
{
    public static class profileMapper
    {
        public static List<profileDTO> Mapper(List<Profile> profiles)
        {
            List<profileDTO> profileDTOs = new List<profileDTO>();
            foreach (Profile profile in profiles)
            {
                profileDTOs.Add(new profileDTO
                {
                    FirstName= profile.FirstName,
                    LastName= profile.LastName,
                    UserName= profile.UserName,
                    Email= profile.Email,
                    ProfileType= profile.ProfileType,
                    CreatedAt= profile.CreatedAt,
                    CreatedBy= profile.CreatedBy,
                    LastUpdatedAt= profile.LastUpdatedAt,
                    LastUpdatedBy= profile.LastUpdatedBy,
                    imagePath= profile.imagePath
                });
            }
            return profileDTOs;
        }
    }
}
