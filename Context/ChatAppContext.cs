using ChatApp.Context.EntityClasses;
using Microsoft.EntityFrameworkCore;


namespace ChatApp.Context
{
    public class ChatAppContext : DbContext
    {
        public ChatAppContext(DbContextOptions<ChatAppContext> options)
           : base(options)
        {
        }
        /// <summary>
        /// Gets or sets Answers.
        /// </summary>
        public virtual DbSet<Profile> Profiles { get; set; }
        public virtual DbSet<Message> Messages { get; set; }
        public virtual DbSet<Connections> Connections { get; set; }
        public virtual DbSet<GroupMembers> GroupMembers { get; set; }
        public virtual DbSet<GroupMessages> GroupMessages { get; set; }
        public virtual DbSet<Groups> Groups { get; set; }
        public virtual DbSet<Notifications> Notifications { get; set; }
        public virtual DbSet<Designation> Designation { get; set; }
		public virtual DbSet<UserStatus> UserStatus { get; set; }
	}
}
