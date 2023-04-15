using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChatApp.Business.Helpers
{
    public static class ClaimsConstant
    {
        public const string FirstNameClaim = "firstName";
        public const string LastNameClaim = "lastName";
        public const string ImageUrlClaim = "imageUrl";
        public const string DesignationClaim = "designation";

    }
    public enum ProfileType
    {
        User = 1,
        Administrator = 2
    }

    public enum MessageType
    {
        Text = 1,
        Audio = 2,
        Image = 3,
        Video = 4,
        Other = 5
    }
    
}
