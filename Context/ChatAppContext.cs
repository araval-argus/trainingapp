using ChatApp.Context.EntityClasses;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChatApp.Context
{
    public class ChatAppContext : DbContext
    {
        public ChatAppContext(DbContextOptions<ChatAppContext> options)
           : base(options)
        {
        }
        public virtual DbSet<Profile> Profiles { get; set; }
        public virtual DbSet<DesignationEntity> Designations { get; set; }
        public virtual DbSet<MessageEntity> Messages { get; set; }
        public virtual DbSet<OnlineUserEntity> OnlineUsers { get; set; }
        public virtual DbSet<Group> Groups { get; set; }
        public virtual DbSet<GroupMessage> GroupMessages { get; set; }
        public virtual DbSet<GroupMember> GroupMembers { get; set; }
        public virtual DbSet<NotificationType> NotificationTypes { get; set; }
        public virtual DbSet<Notification> Notifications { get; set; }
    }
}
