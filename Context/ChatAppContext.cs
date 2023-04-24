using ChatApp.Context.EntityClasses;
using ChatApp.Models;
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
        /// <summary>
        /// Gets or sets Answers.
        /// </summary>
        public virtual DbSet<Profile> Profiles { get; set; }
        public virtual DbSet<Chat> Chats { get; set; }
        public virtual DbSet<Connections> Connections { get; set; }
        public virtual DbSet<Groups> Groups { get; set; }
        public virtual DbSet<GroupMember> GroupMember { get; set; }

        public virtual DbSet<GroupMessage> GroupMessage { get; set; }
        public virtual DbSet<Notifications> Notification { get; set; }
    }
}
