using ChatApp.Context.EntityClasses;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChatApp.Context
{


    public class ChatAppContext : IdentityDbContext
    {


        public ChatAppContext(DbContextOptions<ChatAppContext> options)
           : base(options)
        {

        }
        public virtual DbSet<DesignationEntity> Designations { get; set; }
        public virtual DbSet<Profile> Profiles { get; set; }
        public virtual DbSet<MessageEntity> Messages { get; set; }
        public virtual DbSet<OnlineUserEntity> OnlineUsers { get; set; }
        public virtual DbSet<Group> Groups { get; set; }
        public virtual DbSet<GroupMessage> GroupMessages { get; set; }
        public virtual DbSet<GroupMember> GroupMembers { get; set; }
        public virtual DbSet<NotificationType> NotificationTypes { get; set; }
        public virtual DbSet<Notification> Notifications { get; set; }
        public virtual DbSet<Status> Status { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<Group>().HasOne(group => group.LastModifier)
                .WithMany(profile => profile.ModifiedGroups)
                .HasForeignKey(group => group.LastUpdatedBy)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Notification>().HasOne(notification => notification.NotificationCreator)
                .WithMany(profile => profile.NotificationsRaisedBy)
                .HasForeignKey(notification => notification.RaisedBy)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Notification>().HasOne(notification => notification.NotificationReceiver)
                .WithMany(profile => profile.NotificationsRaisedFor)
                .HasForeignKey(notification => notification.RaisedFor)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<GroupMember>().HasOne(member => member.Member)
                .WithMany(profile => profile.GroupMembers)
                .HasForeignKey(member => member.ProfileID)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<GroupMessage>().HasOne(message => message.Sender)
                .WithMany(profile => profile.SentGroupMessages)
                .HasForeignKey(message => message.SenderID)
                .OnDelete(DeleteBehavior.Restrict);

            
            //Data Seeding in designations
            builder.Entity<Status>().HasData(
            new Status()
            {
                Id = 1,
                Type = "Available",
                TagClasses = "mdi mdi-check-circle",
                TagStyle = "color: green"
            },
            new Status()
            {
                Id = 2,
                Type = "Busy",
                TagClasses = " mdi mdi-checkbox-blank-circle",
                TagStyle = "color: red"
            },
            new Status()
            {
                Id = 3,
                Type = "Do not disturb",
                TagClasses = "mdi mdi-minus-circle",
                TagStyle = "color: red"
            },
            new Status()
            {
                Id = 4,
                Type = "Be right back",
                TagClasses = "mdi mdi-clock",
                TagStyle = "color: #d6d900"
            },
            new Status()
            {
                Id = 5,
                Type = "Appear away ",
                TagClasses = "mdi mdi-clock",
                TagStyle = "color: #d6d900"
            },
            new Status()
            {
                Id = 6,
                Type = "Appear offline",
                TagClasses = "mdi mdi-close-circle-outline",
                TagStyle = "color: grey"
            }
            );

            //Data Seeding in notificationtypes
            builder.Entity<NotificationType>().HasData(
            new NotificationType{
                Id = 1,
                Type = "text messsage"
            },
            new NotificationType
            {
                Id = 2,
                Type = "image shared"
            },
            new NotificationType
            {
                Id = 3,
                Type = "video shared"
            },
            new NotificationType
            {
                Id = 4,
                Type = "Audio Shared"
            },
            new NotificationType
            {
                Id = 5,
                Type = "group text message"
            },
            new NotificationType
            {
                Id = 6,
                Type = "image shared in the group"
            },
            new NotificationType
            {
                Id = 7,
                Type = "video shared in the group"
            },
            new NotificationType
            {
                Id = 8,
                Type = "audio shared in the group"
            },
            new NotificationType
            {
                Id = 9,
                Type = "group member left"
            },
            new NotificationType
            {
                Id = 10,
                Type = "group member removed"
            },
            new NotificationType
            {
                Id = 11,
                Type = "group member added"
            },
            new NotificationType
            {
                Id = 12,
                Type = "new admin"
            }
            );

            //Data Seeding in designations
            builder.Entity<DesignationEntity>().HasData(
                new DesignationEntity
                {
                    Id = 1,
                    Designation = "Intern"
                },
                new DesignationEntity
                {
                    Id = 2,
                    Designation = "Probationer"
                },
                new DesignationEntity
                {
                    Id = 3,
                    Designation = "Programmer Analyst"
                },
                new DesignationEntity
                {
                    Id = 4,
                    Designation = "Solution Analyst"
                },
                new DesignationEntity
                {
                    Id = 5,
                    Designation = "Lead Solution Analyst"
                },
                new DesignationEntity
                {
                    Id = 6,
                    Designation = "Group Lead"
                },
                new DesignationEntity
                {
                    Id = 7,
                    Designation = "Group Director"
                },
                new DesignationEntity
                {
                    Id = 8,
                    Designation = "Chief Technical Officer"
                },
                new DesignationEntity
                {
                    Id = 9,
                    Designation = "Chief Executive Officer"
                }
                );

            string userId = Guid.NewGuid().ToString();
            string adminId = Guid.NewGuid().ToString();

            var roles = new List<IdentityRole> { 
                new IdentityRole()
                {
                    Id = userId,
                    ConcurrencyStamp = userId,
                    Name = "User",
                    NormalizedName = "USER"
                },
                new IdentityRole()
                {
                    Id = adminId,
                    ConcurrencyStamp = adminId,
                    Name = "Admin",
                    NormalizedName = "ADMIN"
                }
            };

            builder.Entity<IdentityRole>().HasData(roles);



        }
    }
}
