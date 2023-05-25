using ChatApp.Business.Helpers;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace ChatApp.Context.EntityClasses
{
    public class Profile: IdentityUser
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public ProfileType ProfileType { get; set; }
        public DateTime? CreatedAt { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? LastUpdatedAt { get; set; }
        public int? LastUpdatedBy { get; set; }
        public string? ImageUrl { get; set; }

        [Display(Name ="Designation")]
        public int DesignationID { get; set; }

        [ForeignKey("DesignationID")]
        public virtual DesignationEntity Designation { get; set; }

        public int StatusID { get; set; }

        [ForeignKey("StatusID")]
        public virtual Status Status { get; set; }

        public bool IsActive { get; set; }

        public IEnumerable<Group> CreatedGroups { get; set; }
        public IEnumerable<Group> ModifiedGroups { get; set; }
        public IEnumerable<Notification> NotificationsRaisedBy { get; set; }
        public IEnumerable<Notification> NotificationsRaisedFor { get; set; }
        public IEnumerable<GroupMember> GroupMembers { get; set; }
        public IEnumerable<GroupMessage> SentGroupMessages { get; set; }
        public IEnumerable<GroupMessage> RecievedGroupMessages { get; set; }


    }
}
