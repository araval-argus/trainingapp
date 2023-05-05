namespace ChatApp.Models
{
    public class GroupMemberModel
    {
        public int groupId { get; set; } 
        public string FirstName { get; set; }        
        public string LastName { get; set; }
        public string UserName { get; set; }
        public string ImageUrl { get; set; }
        public bool IsAdmin { get; set; }    

    }
}
