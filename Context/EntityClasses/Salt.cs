namespace ChatApp.Context.EntityClasses
{
    public class Salt
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string HashSalt { get; set; }
    }
}
