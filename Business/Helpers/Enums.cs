namespace ChatApp.Business.Helpers
{
    public static class ClaimsConstant
    {
        public const string FirstNameClaim = "firstName";
        public const string LastNameClaim = "lastName";
        public const string ImagePathClaim = "imagePath";
        public const string DesignationClaim = "designation";
	}

    public enum ProfileType
    {
        User = 1,
        Administrator = 2
    }
    
}
