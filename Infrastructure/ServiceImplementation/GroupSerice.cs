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

namespace ChatApp.Infrastructure.ServiceImplementation
{
    public class GroupSerice : IGroupService
    {
        private readonly ChatAppContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public GroupSerice(ChatAppContext context, IWebHostEnvironment webHostEnvironment) {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }

        public SentGroupMessage addFile(GroupChatFileModel message, string senderUserName)
        {
            Profile profile = _context.Profiles.AsNoTracking().FirstOrDefault(e => e.UserName == senderUserName);
            if (profile != null)
            {
                if (_context.Groups.AsNoTracking().FirstOrDefault(e => e.Id == int.Parse(message.GroupId)) != null)
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
                    return msg;
                }
            }
            return null;

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
                    }
                }
            }
            _context.SaveChanges();
            return true;
        }

        public SentGroupMessage addMessage(GroupReceiveChatModel message, string senderUserName)
        {
            Profile profile = _context.Profiles.AsNoTracking().FirstOrDefault(e => e.UserName == senderUserName);
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
                    string repliedMsgContent = null;
                    if(newMessage.ReplyMessageID != null)
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
                    return msg;
                }
            }
            return null;
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
    }
}
