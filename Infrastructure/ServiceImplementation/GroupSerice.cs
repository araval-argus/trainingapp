using ChatApp.Business.ServiceInterfaces;
using ChatApp.Context;
using ChatApp.Context.EntityClasses;
using ChatApp.Models;
using Microsoft.AspNetCore.Hosting;
using System.IO;
using System;
using System.Linq;
using ChatApp.Business.Helpers;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using ChatApp.Hubs;
using Microsoft.AspNetCore.SignalR;

namespace ChatApp.Infrastructure.ServiceImplementation
{
    public class GroupSerice : IGroupService
    {
        private readonly ChatAppContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IHubContext<ChatHub> _hubContext;
        private readonly INotificationService _notificationService;


        public GroupSerice(ChatAppContext context, IWebHostEnvironment webHostEnvironment, IHubContext<ChatHub> hubContext, INotificationService notificationService) {
            this._context = context;
            this._webHostEnvironment = webHostEnvironment;
            this._hubContext = hubContext;
            this._notificationService = notificationService;

        }

        public bool addFile(GroupChatFileModel message, string senderUserName)
        {
            Profile profile = _context.Profiles.AsNoTracking().FirstOrDefault(e => e.UserName == senderUserName);
            if (profile != null)
            {
                Groups group = _context.Groups.AsNoTracking().FirstOrDefault(e => e.Id == int.Parse(message.GroupId));
                if (group != null)
                {
                    var fullFile = "";
                    if (message.File != null)
                    {
                        var file = message.File;
                        string rootPath = _webHostEnvironment.WebRootPath;
                        string fileName = Guid.NewGuid().ToString();
                        var extension = Path.GetExtension(file.FileName);
                        var pathToSave = Path.Combine(rootPath, @"files");
                        fullFile = fileName + extension;
                        var dbPath = Path.Combine(pathToSave, fullFile);
                        using (var fileStreams = new FileStream(dbPath, FileMode.Create))
                        {
                            file.CopyTo(fileStreams);
                        }
                    }

                    _notificationService.addNotification(message.GroupId, true, profile.Id);

                    GroupMessage newMessage = new()
                    {
                        SenderId = profile.Id,
                        GroupID = int.Parse(message.GroupId),
                        Content = fullFile,
                        ReplyMessageID = message.RepliedId == null ? null : int.Parse(message.RepliedId),
                        Time = DateTime.Now,
                        Type = message.File.ContentType
                    };
                    _context.GroupMessage.Add(newMessage);
                    _context.SaveChanges();
                    string repliedMsgContent = null;
                    if (newMessage.ReplyMessageID != -1)
                    {
                        GroupMessage repliedMsg = _context.GroupMessage.AsNoTracking().FirstOrDefault(e => e.Id == newMessage.ReplyMessageID);
                        repliedMsgContent = repliedMsg.Content;
                    }
                    SentGroupMessage msg = new()
                    {
                        Id = newMessage.Id,
                        Content = newMessage.Content,
                        GroupId = newMessage.GroupID,
                        SenderUserName = profile.UserName,
                        ReplyingMessageId = newMessage.ReplyMessageID,
                        ReplyingContent = repliedMsgContent,
                        sentAt = newMessage.Time,
                        Type = newMessage.Type,
                    };
                    _hubContext.Clients.Group(group.Name).SendAsync("receivedChatForGroup", msg);
                    return true;
                }
            }
            return false;

        }

        public GroupDTO AddGroup(GroupModel newGroupModel, string userName)
        {
            Profile profile = _context.Profiles.FirstOrDefault(e => e.UserName == userName);
            int adminId = profile.Id;
            string fullFile = null;
            if (newGroupModel.Image != null)
            {
                var file = newGroupModel.Image;
                string rootPath = _webHostEnvironment.WebRootPath;
                string fileName = Guid.NewGuid().ToString();
                var extension = Path.GetExtension(file.FileName);
                var pathToSave = Path.Combine(rootPath, @"images");
                fullFile = fileName + extension;
                var dbPath = Path.Combine(pathToSave, fullFile);
                using (var fileStreams = new FileStream(dbPath, FileMode.Create))
                {
                    file.CopyTo(fileStreams);
                }
            }
            Groups group = new()
            {
                Name = newGroupModel.Name,
                Description = newGroupModel.Description,
                ProfileImage = fullFile,
                Admin = adminId,
            };
            _context.Groups.Add(group);
            //Need TO save group first so that we can add that group in GroupMember Entity
            _context.SaveChanges();
            GroupMember groupMember = new()
            {
                GroupId = group.Id,
                MemberId = adminId,
                AddedDate= DateTime.Now,
            };
            _context.GroupMember.Add(groupMember);
            _context.SaveChanges();
            GroupDTO groupDTO  =Mapper.groupToGroupDTO(group);
            return groupDTO;
        }

        public bool addMembers(List<string> userName, string groupName, string adminUser)
        {
            Groups group = _context.Groups.AsNoTracking().Include(e => e.AdminProfile).FirstOrDefault(e => e.Name == groupName);
            if (group == null && group.AdminProfile.UserName != adminUser)
            {
                return false;
            }
            var memberList = "";
            List<int> presentMembersId = _context.GroupMember.AsNoTracking().Where(e => e.GroupId == group.Id).Select(e => e.MemberId).ToList();
            foreach(string user in userName)
            {
                Profile profile = _context.Profiles.AsNoTracking().FirstOrDefault(e => e.UserName== user);
                if(profile != null)
                {
                    if (!presentMembersId.Contains(profile.Id)){
                        GroupMember newMember = new()
                        {
                            GroupId = group.Id,
                            MemberId = profile.Id,
                            AddedDate = DateTime.Now,
                        };
                        _context.GroupMember.Add(newMember);
                        Connections isActiveConnection = _context.Connections.AsNoTracking().FirstOrDefault(e => e.User == profile.Id);
                        if (isActiveConnection != null)
                        {
                            _hubContext.Groups.AddToGroupAsync(isActiveConnection.ConnectionId, groupName);
                            _hubContext.Clients.Client(isActiveConnection.ConnectionId).SendAsync("addedToGroup", Mapper.groupToGroupDTO(group));
                        }
                    }
                }
            }
            foreach(var member in userName)
            {
                memberList += member + " ";
            }
            if(userName.Count> 0)
            {
                Profile adminProfile = _context.Profiles.AsNoTracking().FirstOrDefault(e => e.UserName == adminUser);
                GroupMessage addedNotification = new()
                {
                    Content = adminUser + " has added " + memberList,
                    GroupID = group.Id,
                    SenderId = adminProfile.Id,
                    ReplyMessageID = -1,
                    Type = "notification",
                    Time = DateTime.Now,
                };
                _context.GroupMessage.Add(addedNotification);
                _context.SaveChanges();
                SentGroupMessage msg = new()
                {
                    Id = addedNotification.Id,
                    Content = addedNotification.Content,
                    GroupId = group.Id,
                    SenderUserName = adminUser,
                    sentAt = addedNotification.Time,
                    ReplyingContent = null,
                    ReplyingMessageId = -1,
                    Type = addedNotification.Type
                };
                _hubContext.Clients.Group(groupName).SendAsync("addNotification", msg);
            }
            _context.SaveChanges();
            return true;
        }

        public bool addMessage(GroupReceiveChatModel message, string senderUserName)
        {
            Profile profile = _context.Profiles.AsNoTracking().FirstOrDefault(e => e.UserName == senderUserName);
            Groups group = _context.Groups.AsNoTracking().FirstOrDefault(e => e.Id == message.GroupId);
            if(profile != null)
            {
                if(_context.Groups.AsNoTracking().FirstOrDefault(e => e.Id == message.GroupId) != null)
                {
                    GroupMessage newMessage = new()
                    {
                        SenderId = profile.Id,
                        GroupID = message.GroupId,
                        Content= message.Content,
                        ReplyMessageID = message.RepliedId == 0 ? null : message.RepliedId,
                        Time = DateTime.Now,
                        Type = "text"
                    };
                    _context.GroupMessage.Add(newMessage);
                    _context.SaveChanges();
                    _notificationService.addNotification(group.Name, true, profile.Id);

                    string repliedMsgContent = null;
                    if(newMessage.ReplyMessageID != -1)
                    {
                        GroupMessage repliedMsg = _context.GroupMessage.AsNoTracking().FirstOrDefault(e => e.Id == newMessage.ReplyMessageID);
                        repliedMsgContent = repliedMsg.Content;
                    }
                    SentGroupMessage msg = new()
                    {
                        Id = newMessage.Id,
                        Content = newMessage.Content,
                        GroupId = newMessage.GroupID,
                        SenderUserName = profile.UserName,
                        ReplyingMessageId = newMessage.ReplyMessageID,
                        ReplyingContent = repliedMsgContent,
                        sentAt = newMessage.Time,
                        Type= newMessage.Type,
                    };
                    _hubContext.Clients.Group(group.Name).SendAsync("receivedChatForGroup", msg);
                    return true;
                }
            }
            return false
                ;
        }

        public List<GroupDTO> getAll(string userName)
        {
            IEnumerable<Groups> groups = _context.GroupMember.Where(e => e.Profile.UserName == userName).Include(e => e.Group.AdminProfile).Select(e => e.Group);
            List<GroupDTO> groupDTOs = groups.Select(e => Mapper.groupToGroupDTO(e)).ToList();
            return groupDTOs;
        }

        public List<SentGroupMessage> getAllChat(string groupName)
        {
            Groups group = _context.Groups.AsNoTracking().FirstOrDefault(e => e.Name.Equals(groupName));
            List<SentGroupMessage> chats = new();
            if(group != null)
            {
                List<GroupMessage> msgs = _context.GroupMessage.AsNoTracking().Where(e => e.GroupID == group.Id).ToList();
                foreach(GroupMessage msg in msgs)
                {
                    Profile temp = _context.Profiles.AsNoTracking().FirstOrDefault(e => e.Id == msg.SenderId);
                    GroupMessage groupMsg = new();
                    if (msg.ReplyMessageID != -1)
                    {
                        groupMsg = _context.GroupMessage.AsNoTracking().FirstOrDefault(e => e.Id == msg.ReplyMessageID);
                    }
                    chats.Add(new SentGroupMessage()
                    {
                        Id = msg.Id,
                        Content = msg.Content,
                        GroupId = group.Id,
                        SenderUserName = temp.UserName,
                        sentAt = msg.Time,
                        ReplyingContent = msg.ReplyMessageID != -1 ? groupMsg.Content : null,
                        ReplyingMessageId = msg.ReplyMessageID,
                        Type = msg.Type
                    });
                }
            }
            return chats;
        }

        public List<profileDTO> getMembers(string userName, string groupName)
        {
            Groups group = _context.Groups.AsNoTracking().FirstOrDefault(e => e.Name == groupName);
            if(group == null)
            {
                return null;
            }
            List<Profile> memberProfile = _context.GroupMember.AsNoTracking().Where(e => e.Group.Id == group.Id).Select(e => e.Profile).ToList();
            List<profileDTO> profileDTOs = Mapper.profilesMapper(memberProfile);
            return profileDTOs;
        }

        public bool removeMember(List<string> userNames, string groupName, string userName)
        {
            //Find the group
            //Check if given user is admin
            //if admin is in remove list skip it
            Groups group = _context.Groups.AsNoTracking().FirstOrDefault(e => e.Name == groupName);
            Profile profile = _context.Profiles.AsNoTracking().FirstOrDefault(e => e.UserName == userName);
            if(group != null) { 
                if(profile != null)
                {
                    if(group.Admin == profile.Id)
                    {
                        if(userNames.Contains(userName)) {
                            userNames.Remove(userName);
                        }
                        List<GroupMember> removingProfiles = new List<GroupMember>();
                       string removingMemberListSent  = "";
                        foreach (string name in userNames)
                        {
                            GroupMember removingMember = _context.GroupMember.Include(e => e.Group).Include(e => e.Profile).FirstOrDefault( e=> e.Group.Name == groupName && e.Profile.UserName == name);
                            if(removingMember != null)
                            {
                                removingMemberListSent += name + ", ";
                                removingProfiles.Add(removingMember);
                                Connections connection = _context.Connections.FirstOrDefault(e => e.User == removingMember.Profile.Id);
                                if(connection != null)
                                {
                                    _hubContext.Clients.Client(connection.ConnectionId).SendAsync("removedFromGroup", groupName);
                                    _hubContext.Groups.RemoveFromGroupAsync(connection.ConnectionId, groupName);
                                }
                            }
                        }
                        if(removingProfiles.Count() > 0)
                        {
                            _context.GroupMember.RemoveRange(removingProfiles);
                            GroupMessage addedNotification = new()
                            {
                                Content = userName + " has removed " + removingMemberListSent,
                                GroupID = group.Id,
                                SenderId = profile.Id,
                                ReplyMessageID = -1,
                                Type = "notification",
                                Time = DateTime.Now,
                            };
                            _context.GroupMessage.Add(addedNotification);
                            _context.SaveChanges();

                            //Creating new Message notification 
                            SentGroupMessage msg = new()
                            {
                                Id = addedNotification.Id,
                                Content = addedNotification.Content,
                                GroupId = group.Id,
                                SenderUserName = userName,
                                sentAt = addedNotification.Time,
                                ReplyingContent = null,
                                ReplyingMessageId = -1,
                                Type = addedNotification.Type
                            };
                            _hubContext.Clients.Group(groupName).SendAsync("addNotification", msg);
                            return true;
                        }
                    }
                }
            }
            return false;
        }

        public bool UpdateGroup(GroupModel model, string userName)
        {
            Groups group = _context.Groups.Include(e => e.AdminProfile).FirstOrDefault(e => e.Name == model.Name);
            if(group != null && group.AdminProfile.UserName == userName)
            {
                group.Description = model.Description;
                if(model.Image != null)
                {
                    var file = model.Image;
                    string rootPath = _webHostEnvironment.WebRootPath;
                    string fileName = Guid.NewGuid().ToString();
                    var extension = Path.GetExtension(file.FileName);
                    var pathToSave = Path.Combine(rootPath, @"images");
                    var fullFile = fileName + extension;
                    var dbPath = Path.Combine(pathToSave, fullFile);
                    using (var fileStreams = new FileStream(dbPath, FileMode.Create))
                    {
                        file.CopyTo(fileStreams);
                    }
                    group.ProfileImage = fullFile;
                }
                _context.Groups.Update(group);
                _context.SaveChanges();
                var updatedGroup = Mapper.groupToGroupDTO(group);
                _hubContext.Clients.Group(group.Name).SendAsync("groupUpdate", updatedGroup);
                return true;
            }
            return false;
        }

        public bool leaveGroup(string userName, string groupName)
        {
            Groups groupProfile = _context.Groups.FirstOrDefault(e => e.Name == groupName);
            Profile member = _context.Profiles.AsNoTracking().FirstOrDefault(e => e.UserName.Equals(userName));
            if(groupProfile != null && member != null)
            {
                GroupMember leavingMember = _context.GroupMember.FirstOrDefault(e => e.MemberId == member.Id);
                if(leavingMember!= null)
                {
                    _context.GroupMember.Remove(leavingMember);
                    _context.SaveChanges();
                    string notificationMessage = userName + " has leave " + groupName;
                    if (groupProfile.Admin == member.Id)
                    {
                        GroupMember newAdmin = _context.GroupMember.AsNoTracking().Include(e => e.Profile).Where(e => e.GroupId == groupProfile.Id).OrderBy(e => e.AddedDate).FirstOrDefault();
                        groupProfile.Admin = newAdmin.MemberId;
                        notificationMessage += " and new Admin is " + newAdmin.Profile.UserName;
                        _context.Groups.Update(groupProfile);
                    }

                    //Adding Notification Message
                    GroupMessage addedNotification = new()
                    {
                        Content = notificationMessage,
                        GroupID = groupProfile.Id,
                        SenderId = member.Id,
                        ReplyMessageID = -1,
                        Type = "notification",
                        Time = DateTime.Now,
                    };
                    _context.GroupMessage.Add(addedNotification);
                    _context.SaveChanges();

                    //Creating new Message notification 
                    SentGroupMessage msg = new()
                    {
                        Id = addedNotification.Id,
                        Content = addedNotification.Content,
                        GroupId = addedNotification.GroupID,
                        SenderUserName = userName,
                        sentAt = addedNotification.Time,
                        ReplyingContent = null,
                        ReplyingMessageId = -1,
                        Type = addedNotification.Type
                    };
                    _hubContext.Clients.Group(groupName).SendAsync("addNotification", msg);

                    //remove Connection from group && clear group (hide inbox if open and remove from group List)
                    Connections connection = _context.Connections.FirstOrDefault(e => e.User == leavingMember.MemberId);
                    if (connection != null)
                    {
                        _hubContext.Clients.Client(connection.ConnectionId).SendAsync("removedFromGroup", groupName);
                        _hubContext.Groups.RemoveFromGroupAsync(connection.ConnectionId, groupName);
                    }
                    return true;
                }
            }
            return false;
        }

    }
}
