using ChatApp.Business.Helpers;
using ChatApp.Business.ServiceInterfaces;
using ChatApp.Context.EntityClasses;
using ChatApp.Hubs;
using ChatApp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace ChatApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class GroupController : ControllerBase
    {
        private readonly IGroupService groupService;
        private readonly IGroupMemberService groupMemberService;
        private readonly IProfileService profileService;
        private readonly IGroupMessageService groupMessageService;
        private readonly IWebHostEnvironment webHostEnvironment;
        private readonly IHubContext<ChatHub> hubContext;
        private readonly IOnlineUserService onlineUserService;
        private readonly INotificationService notificationService;

        public GroupController(IGroupService groupService,
            IGroupMemberService groupMemberService,
            IProfileService profileService,
            IGroupMessageService groupMessageService,
            IWebHostEnvironment webHostEnvironment,
            IHubContext<ChatHub> hubContext,
            IOnlineUserService onlineUserService,
            INotificationService notificationService
            )
        {
            this.groupService = groupService;
            this.groupMemberService = groupMemberService;
            this.profileService = profileService;
            this.groupMessageService = groupMessageService;
            this.webHostEnvironment = webHostEnvironment;
            this.hubContext = hubContext;
            this.onlineUserService = onlineUserService;
            this.notificationService = notificationService;
        }

        [HttpPost("CreateGroup")]
        public IActionResult CreateGroup([FromForm] GroupModel GroupModel)
        {
            var profile = this.profileService.FetchProfileFromUserName(GroupModel.CreatorUserName);

            if(profile == null)
            {
                return BadRequest("User is not valid");
            }

            var groupModel = this.groupService.CreateGroup(GroupModel, profile.Id);
            if(groupModel == null)
            {
                return BadRequest("Something went wrong");
            }

            this.groupMemberService.AddGroupMember(profile.Id, groupModel.Id, true);
            return Ok(groupModel);
        }

        [HttpGet("FetchGroups")]
        public IActionResult FetchGroups([FromQuery] string UserName)
        {
            if(UserName == null)
            {
                return BadRequest("username is missing");
            }
            int userId = this.profileService.FetchIdFromUserName(UserName);
            IEnumerable<GroupModel> groups = this.groupService.FetchGroups(userId);
            return Ok(groups);
        }

        [HttpGet("GetNotJoinedUsers")]
        public IActionResult FetchNotJoinedUsers([FromQuery] int GroupId, [FromHeader] string Authorization)
        { 
            if(GroupId <= 0)
            {
                return NotFound("group does not exist");
            }

            var userName = CustomAuthorization.GetUsernameFromToken(Authorization);
            int userId = this.profileService.FetchIdFromUserName(userName);

            if (!this.groupMemberService.IsAdmin(userId, GroupId))
            {
                return Unauthorized("Only Admin can see the list");
            }

            var notJoinedUsers = this.groupMemberService.ListOfNotJoinedMembers(GroupId);

            if (notJoinedUsers== null)
            {
                return BadRequest("group does not exist");
            }
            return Ok(notJoinedUsers);
        }

        [HttpPost("AddMember")]
        public IActionResult AddMember([FromBody] GroupMemberModel User, [FromQuery] int GroupId, [FromHeader] string Authorization)
        {
            string senderUsername = CustomAuthorization.GetUsernameFromToken(Authorization);
            int senderId = this.profileService.FetchIdFromUserName(senderUsername);

            if(senderId <= 0)
            {
                return NotFound("User Does Not Exist");
            }

            Group group = this.groupService.FetchGroupFromId(GroupId);

            if (group == null)
            {
                return NotFound("No Group Found");
            }

            if (!this.groupMemberService.IsAdmin(senderId, GroupId))
            {
                return Unauthorized("Only Admin Can Add New Member");
            }

            int memberId = this.profileService.FetchIdFromUserName(User.UserName);
            this.groupMemberService.AddGroupMember(memberId, group.Id, false);

            Notification notification = new()
            {
                Type = 11,
                RaisedBy = senderId,
                CreatedAt = DateTime.UtcNow,
                RaisedInGroup = GroupId
            };

            OnlineUserEntity newMember = this.onlineUserService.FetchOnlineUser(User.UserName);

            if(newMember != null)
            { 
                this.hubContext.Clients.Client(newMember.ConnectionId).SendAsync("NewGroupAdded", group.Id);
            }

            IList<GroupMember> members = this.groupMemberService.ListOfJoinedMembers(group.Id);
            IList<string> onlineMembersConnectionIds = new List<string>();

            foreach (GroupMember member in members)
            {
                string memberUserName = this.profileService.FetchUserNameFromId(member.ProfileID);

                notification.RaisedFor = member.ProfileID;
                notification = this.notificationService.AddNotification(notification);

                OnlineUserEntity onlineUser = this.onlineUserService.FetchOnlineUser(memberUserName);
                if (onlineUser != null)
                {
                    if(onlineUser.UserName != senderUsername)
                    {
                        var notificationModel = EntityToModel.ConvertToNotificationModel(notification);
                        notificationModel.RaisedInGroup = group.Name;
                        this.hubContext.Clients.Client(onlineUser.ConnectionId).SendAsync("AddNotification",
                            notificationModel);
                    }
                    onlineMembersConnectionIds.Add(onlineUser.ConnectionId);
                }
                // As id is identity and expects its value as 0 
                notification.Id = 0;
            }

            this.hubContext.Clients.Clients(onlineMembersConnectionIds).SendAsync("AddGroupMember", User);

            return Ok(User);
        }

        [HttpDelete("RemoveMember")]
        public IActionResult RemoveMember([FromQuery] string MemberUserName, [FromQuery] int GroupId, [FromHeader] string Authorization)
        {
            string senderUserName = CustomAuthorization.GetUsernameFromToken(Authorization);
            int senderId = this.profileService.FetchProfileFromUserName(senderUserName).Id;            

            if(senderUserName != MemberUserName && !this.groupMemberService.IsAdmin(senderId, GroupId))
            {
                return Unauthorized("Only member itself and admin can remove from the group");
            }

            var user = this.profileService.FetchProfileFromUserName(MemberUserName);

            if(user == null)
            {
                return NotFound("User not found");
            }

            var group = this.groupService.FetchGroupFromId(GroupId);

            if(group == null)
            {
                return NotFound("Group not found");
            }

            int userId = user.Id;

            this.groupMessageService.RemoveAllGroupMessagesSentByMember(userId, GroupId);

            var member = this.groupMemberService.RemoveMember(userId, GroupId);

            if(member == null)
            {
                return BadRequest("Something went wrong");
            }

            Notification notification = new()
            {
                RaisedBy = senderId,
                CreatedAt = DateTime.UtcNow,
                RaisedInGroup = GroupId
            };

            if (member.IsAdmin)
            {

                IList<GroupMember> joinedMembers = this.groupMemberService.ListOfJoinedMembers(GroupId);
                if(joinedMembers.Count <= 0)
                {
                    this.groupMessageService.DeleteAllGroupMessages(GroupId);
                    this.groupService.DeleteGroup(GroupId);
                    return Ok(new {message = "Group Deleted as no more members left"});
                }
                if (!this.groupMemberService.IsThereAnyAdminLeftInTheGroup(GroupId))
                {
                    this.groupMemberService.MakeAdmin(joinedMembers[0]);
                }                
            }

            notification.Type = senderUserName == MemberUserName ? 9 : 10;

            OnlineUserEntity removedMember = this.onlineUserService.FetchOnlineUser(MemberUserName);
            if(removedMember != null)
            {
                this.hubContext.Clients.Client(removedMember.ConnectionId).SendAsync("RemoveGroup", MemberUserName);
            }

            IList<GroupMember> members = this.groupMemberService.ListOfJoinedMembers(GroupId);
            IList<string> onlineMembersConnectionIds = new List<string>();

            foreach (GroupMember groupMember in members)
            {
                string memberUserName = this.profileService.FetchUserNameFromId(groupMember.ProfileID);
                OnlineUserEntity onlineUser = this.onlineUserService.FetchOnlineUser(memberUserName);

                notification.RaisedFor = groupMember.ProfileID;
                notification = this.notificationService.AddNotification(notification);

                if (onlineUser != null)
                {
                    onlineMembersConnectionIds.Add(onlineUser.ConnectionId);
                    if(onlineUser.UserName != senderUserName)
                    {
                        var notificationModel = EntityToModel.ConvertToNotificationModel(notification);
                        notificationModel.RaisedInGroup = group.Name;
                        this.hubContext.Clients.Client(onlineUser.ConnectionId).SendAsync("AddNotification",
                            notificationModel);
                    }
                }

                notification.Id = 0;
            }

            GroupMemberModel removedMemberToBeSent = new()
            {
                FirstName = user.FirstName,
                LastName = user.LastName,
                UserName = user.UserName,
                ImageUrl = user.ImageUrl,
                IsAdmin = member.IsAdmin,
                groupId = GroupId,                
            };

            this.hubContext.Clients.Clients(onlineMembersConnectionIds).SendAsync("MemberRemoved", removedMemberToBeSent);

            string message = senderUserName == MemberUserName ? "You left the group" : "You Removed the user";

            return Ok(new { message });
        }

        [HttpGet("FetchJoinedMembers")]
        public IActionResult FetchJoinedMembers([FromQuery] int GroupId, [FromHeader] string Authorization)
        {
            if(GroupId == null)
            {
                return BadRequest("Group is not provided");
            }

            var group = this.groupService.FetchGroupFromId(GroupId);

            if(group == null)
            {
                return NotFound("Group does not exist");
            }

            string userName = CustomAuthorization.GetUsernameFromToken(Authorization);

            var profile = this.profileService.FetchProfileFromUserName(userName);

            if(profile == null)
            {
                return BadRequest("UnAuthorized User");
            }

            if(!this.groupMemberService.IsMember(profile.Id, GroupId))
            {
                return Unauthorized("Only Group Members Can Fetch The List");
            }

            IList<GroupMemberModel> joinedMembers = this.groupMemberService.ListOfJoinedGroupMemberModels(GroupId);

            return Ok(joinedMembers);

        }

        [HttpPost("UpdateGroup")]
        public IActionResult UpdateGroup([FromForm] GroupModel Group, [FromHeader] string Authorization)
        {
            if(Group == null)
            {
                return BadRequest("No Group is Provided");
            }

            var groupFromDb = this.groupService.FetchGroupFromId(Group.Id);

            if (groupFromDb == null)
            {
                return NotFound("Group not found");
            }

            string userName = CustomAuthorization.GetUsernameFromToken(Authorization);
            var user = this.profileService.FetchProfileFromUserName(userName);
            
            if(!this.groupMemberService.IsAdmin(user.Id, Group.Id))
            {
                return Unauthorized("Only Admins Can Edit The Details Of The Group");
            }

            this.groupService.UpdateGroup(Group, user);

            Group = EntityToModel.ConvertToGroupModel(this.groupService.FetchGroupFromId(Group.Id));
            Group.LoggedInUserIsAdmin = true;
            return Ok(Group);
        }

        [HttpGet("FetchGroupMessages")]
        public IActionResult FetchGroupMessages(string UserName, int GroupId)
        {
            if (UserName == null || GroupId == 0)
            {
                return BadRequest("Required parameters are not provided");
            }

            var user = this.profileService.FetchProfileFromUserName(UserName);

            if(user == null)
            {
                return NotFound("User not found");
            }

            var group = this.groupService.FetchGroupFromId(GroupId);

            if(group == null)
            {
                return NotFound("Group does not exist");
            }

            if(!this.groupMemberService.IsMember(user.Id, GroupId))
            {
                return Unauthorized("Only group members can see the messages");
            }

            IList<GroupMessageModel> messages = this.groupMessageService.FetchGroupMessages(GroupId);

            var groupNotifications = this.notificationService.GetGroupMessagesNotifications(user.Id, GroupId);
            this.notificationService.DeleteNotifications(groupNotifications);

            return Ok(messages);
        }

        [HttpPost("SendFileInGroup")]
        public IActionResult SendFileInGroup([FromForm] GroupFileMessageModel FileModel, [FromHeader] string Authorization)
        {
            string userName = CustomAuthorization.GetUsernameFromToken(Authorization);

            var profile = this.profileService.FetchProfileFromUserName(userName);

            if (profile == null)
            {
                return Unauthorized("Unauthorized User");
            }

            if(!this.groupMemberService.IsMember(profile.Id, FileModel.GroupId))
            {
                return Unauthorized("Only Group Members Can Send Files");
            }

            string path = webHostEnvironment.WebRootPath + @"/SharedFiles/";

            IFormFile file = FileModel.File;

            GroupMessageModel groupMessageModel = new()
            {
                GroupId = FileModel.GroupId,
                SenderUserName = FileModel.SenderUserName,
                RepliedToMsg = "-1",
                CreatedAt = DateTime.Now
            };

            Notification notification = new()
            {
                RaisedInGroup = FileModel.GroupId,
                CreatedAt = DateTime.Now
            };
            if (file.ContentType.StartsWith("image"))
            {
                groupMessageModel.MessageType = MessageType.Image;
                groupMessageModel.Message = FileManagement.CreateUniqueFile(path + "Images", file);
                notification.Type = 6;
            }
            else if (file.ContentType.StartsWith("video"))
            {
                groupMessageModel.MessageType = MessageType.Video;
                groupMessageModel.Message = FileManagement.CreateUniqueFile(path + "Videos", file);
                notification.Type = 7;
            }
            else if (file.ContentType.StartsWith("audio"))
            {
                groupMessageModel.MessageType = MessageType.Audio;
                groupMessageModel.Message = FileManagement.CreateUniqueFile(path + "Audios", file);
                notification.Type = 8;
            }
            else
            {
                return BadRequest(new { message = "only images, videos and audios can be shared" });
            }

            GroupMessage groupMessageFromDb = this.groupMessageService.AddGroupMessage(groupMessageModel);
            GroupMessageModel message = EntityToModel.ConvertToGroupMessageModel(groupMessageFromDb);            

            IList<GroupMember> members = this.groupMemberService.ListOfJoinedMembers(groupMessageModel.GroupId);
            IList<string> onlineMembersConnectionIds = new List<string>();

            notification.RaisedBy = groupMessageFromDb.SenderID;
            foreach (GroupMember member in members)
            {
                string memberUserName = this.profileService.FetchUserNameFromId(member.ProfileID);
                notification.RaisedFor = member.ProfileID;
                OnlineUserEntity onlineUser = this.onlineUserService.FetchOnlineUser(memberUserName);
                if (onlineUser != null)
                {
                    onlineMembersConnectionIds.Add(onlineUser.ConnectionId);
                    if(onlineUser.UserName != FileModel.SenderUserName)
                    {
                        notification = this.notificationService.AddNotification(notification);
                        var notificationModel = EntityToModel.ConvertToNotificationModel(notification);
                        notificationModel.RaisedInGroup = this.groupService.FetchGroupFromId(FileModel.GroupId).Name;
                        this.hubContext.Clients.Client(onlineUser.ConnectionId).SendAsync("AddNotification",
                            notificationModel);
                        // As id column is set to identity so it expects null value for identity column but
                        // the data type is integer so 0 is act as a null
                        notification.Id = 0;
                    }
                    
                }
            }

            this.hubContext.Clients.Clients(onlineMembersConnectionIds).SendAsync("AddGroupMessageToTheList", message);

            return Ok(new {Message = "message sent"});
        }

        [HttpPatch("MakeAdmin")]
        public IActionResult MakeAdmin([FromBody] GroupMemberModel groupMemberModel, [FromHeader] string Authorization)
        {
            string senderUsername = CustomAuthorization.GetUsernameFromToken(Authorization);
            var sender = this.profileService.FetchProfileFromUserName(senderUsername);

            if(sender == null)
            {
                return Unauthorized("Unauthorized User");
            }

            int senderId = sender.Id;

            if(!this.groupMemberService.IsAdmin(senderId, groupMemberModel.groupId))
            {
                return Unauthorized("Only admin can make other members admin");
            }

            Notification notification = new()
            {
                Type = 12,
                RaisedBy = senderId,
                CreatedAt = DateTime.UtcNow,
                RaisedInGroup = groupMemberModel.groupId
            };

            var memberProfile = this.profileService.FetchProfileFromUserName(groupMemberModel.UserName);
            var member = this.groupMemberService.FetchMember(memberProfile.Id, groupMemberModel.groupId);
            this.groupMemberService.MakeAdmin(member);

            IList<GroupMember> members = this.groupMemberService.ListOfJoinedMembers(groupMemberModel.groupId);
            IList<string> onlineMembersConnectionIds = new List<string>();

            foreach (GroupMember groupMember in members)
            {
                string memberUserName = this.profileService.FetchUserNameFromId(groupMember.ProfileID);
                OnlineUserEntity onlineUser = this.onlineUserService.FetchOnlineUser(memberUserName);

                notification.RaisedFor = groupMember.ProfileID;
                notification = this.notificationService.AddNotification(notification);

                if (onlineUser != null)
                {
                    onlineMembersConnectionIds.Add(onlineUser.ConnectionId);
                    if (onlineUser.UserName != senderUsername)
                    {
                        var notificationModel = EntityToModel.ConvertToNotificationModel(notification);
                        notificationModel.RaisedInGroup = this.groupService.FetchGroupFromId(groupMemberModel.groupId).Name;
                        this.hubContext.Clients.Client(onlineUser.ConnectionId).SendAsync("AddNotification",
                            notificationModel);
                    }
                }

                notification.Id = 0;
            }

            groupMemberModel.IsAdmin = true;
            return Ok(groupMemberModel);
        }

        [HttpGet("FetchGroup")]
        public IActionResult FetchGroup([FromQuery] int GroupId, [FromHeader] string Authorization)
        {
            if(GroupId <= 0)
            {
                return BadRequest("GroupId not valid");
            }

            string senderUserName = CustomAuthorization.GetUsernameFromToken(Authorization);
            var sender = this.profileService.FetchProfileFromUserName(senderUserName);

            if(!this.groupMemberService.IsMember(sender.Id, GroupId))
            {
                return Unauthorized("Not a member of the group");
            }

            Group group = this.groupService.FetchGroupFromId(GroupId);
            if(group == null)
            {
                return NotFound("Group not found");
            }
            GroupModel groupModel = EntityToModel.ConvertToGroupModel(group);

            var lastMessage = this.groupMessageService.FetchLastMessage(groupModel.Id);
            if(lastMessage != null)
            {
                groupModel.LastMessage = lastMessage.Message;
                groupModel.LastMessageTimeStamp = lastMessage.CreatedAt;
            }

            return Ok(groupModel);
        }
    

    }
}
