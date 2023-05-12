using ChatApp.Business.ServiceInterfaces;
using ChatApp.Context;
using Microsoft.AspNetCore.Hosting;
using System.IO;
using System;
using ChatApp.Context.EntityClasses;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.SignalR;
using ChatApp.Models.Group;

namespace ChatApp.Infrastructure.ServiceImplementation
{
    public class GroupService : IGroupService
	{
		private readonly ChatAppContext context;
		private readonly IWebHostEnvironment webHostEnvironment;
		private readonly IChatService chatService;
		private readonly IHubContext<chatHub> hubContext;
		private readonly INotificationServices notificationServices;

		public GroupService(ChatAppContext context, IWebHostEnvironment webHostEnvironment , 
			IChatService chatService , IHubContext<chatHub> hubContext , INotificationServices notificationServices)
		{
			this.context = context;
			this.webHostEnvironment = webHostEnvironment;
			this.chatService = chatService;
			this.hubContext = hubContext;
			this.notificationServices = notificationServices;
		}

		public RecentGroupModel CreateGroup(string userName, CreateGroupModel grp)
		{
			Groups newGroup = new Groups
			{
				GroupName = grp.GroupName,
				CreatedAt = DateTime.Now,
				CreatedBy = chatService.GetIdFromUserName(userName),
			};
			if (grp.Description != null)
			{
				newGroup.Description = grp.Description;
			}
			if (grp.ImageFile != null)
			{
				var filename = Guid.NewGuid().ToString(); // new generated image file name
				var uploads = Path.Combine(webHostEnvironment.WebRootPath, @"grpimages");
				var extension = Path.GetExtension(grp.ImageFile.FileName);// Get Extension Of the File

				using (var fileStreams = new FileStream(Path.Combine(uploads, filename + extension), FileMode.Create))
				{
					grp.ImageFile.CopyTo(fileStreams);
				}

				newGroup.ImagePath = "/grpimages/" + filename + extension;
			}
			context.Groups.Add(newGroup);
			context.SaveChanges();
			GroupMessages newMessage = new GroupMessages
			{
				GrpId = newGroup.Id,
				Content = userName + " created Group " + grp.GroupName ,
				MessageFrom = newGroup.CreatedBy,
				CreatedAt = DateTime.Now,
				RepliedTo = -1,
				Type = "heading",
			};
			context.GroupMessages.Add(newMessage);
			context.SaveChanges();
			GroupMembers GroupMember = new GroupMembers
			{
				GrpId = newGroup.Id,
				ProfileId = newGroup.CreatedBy,
				JoinedAt = DateTime.Now,
				Admin = 1
			};
			context.GroupMembers.Add(GroupMember);
			context.SaveChanges();
			RecentGroupModel response = new RecentGroupModel
			{
				Id = newGroup.Id,
				GroupName = newGroup.GroupName,
				Type = newMessage.Type,
				LastMsg = newMessage.Content,
				LastMsgAt = newMessage.CreatedAt,
			};
			if (grp.ImageFile != null)
			{	response.GroupImage = newGroup.ImagePath;	}
			else
			{	response.GroupImage = null;	}
			return response;
		}

		public IEnumerable<RecentGroupModel> GetRecentGroups(string userName)
		{
			var curUserId = this.chatService.GetIdFromUserName(userName);

			// Get All Groups Where user is a Part of 
			var userGroups = context.GroupMembers
				.Where(m => m.ProfileId == curUserId)
				.Select(m => m.GrpId)
				.Distinct();

			var recentGroupList = new List<RecentGroupModel>();
			var sortedMessages = context.GroupMessages.OrderByDescending(m => m.CreatedAt);
			if (userGroups != null)
			{
				foreach (var grpId in userGroups)
				{
					Groups group = context.Groups.FirstOrDefault(m => m.Id == grpId);
					var newObj = new RecentGroupModel
					{
						Id = grpId,
						GroupImage = group.ImagePath,
						GroupName = group.GroupName,		
					};
					var msg = sortedMessages.FirstOrDefault(m => m.GrpId == grpId);
					if (msg != null)
					{
						newObj.Type = msg.Type;
						newObj.LastMsgAt = msg.CreatedAt;
						newObj.LastMsg = msg.Content;
					}
					recentGroupList.Add(newObj);
				}
				var orderedRecentChatList = recentGroupList.OrderByDescending(m => m.LastMsgAt);
				return orderedRecentChatList;
			}
			return null;
		}

		public IEnumerable<GroupMemberList> getAllMembers(int groupId)
		{
			var groupMemberLists = new List<GroupMemberList>();
			var members = context.GroupMembers.Where(m=>m.GrpId==groupId);
			foreach(var member in members)
			{
				var memberProfile = context.Profiles.FirstOrDefault(m => m.Id == member.ProfileId);
				var newObj = new GroupMemberList
				{
					ImagePath = memberProfile.ImagePath,
					Admin = member.Admin,
					FirstName = memberProfile.FirstName,
					LastName = memberProfile.LastName,
					UserName= memberProfile.UserName,
				};
				groupMemberLists.Add(newObj);
			}
			return groupMemberLists;
		}

		public GroupLoadModel getGroup(int groupId , string username)
		{
			var group = context.Groups.FirstOrDefault(u => u.Id == groupId);
			if (group != null)
			{
				int userId = chatService.GetIdFromUserName(username);
				var profile = context.Profiles.FirstOrDefault(u => u.Id == group.CreatedBy);
				if (context.GroupMembers.Any(u => u.GrpId == groupId && u.ProfileId == userId))
				{
					var groupModel = new GroupLoadModel
					{
						Id = group.Id,
						ImagePath = group.ImagePath,
						CreatedAt = group.CreatedAt,
						GroupName= group.GroupName,
						Description= group.Description,
						CreatedBy = profile.FirstName + " " + profile.LastName,
					};
					return groupModel;
				}
			}
			return null;
		}

		public IEnumerable<AddMemberToGroupModel> getAllProfiles(int groupId , string userName)
		{
			
			var allProfiles = new List<AddMemberToGroupModel>();
			List<int> groupMemberId = new List<int>();
			groupMemberId = context.GroupMembers.Where(u => u.GrpId == groupId).Select(u => u.ProfileId).ToList();
			int userId = chatService.GetIdFromUserName(userName);
			var profiles = context.Profiles.Where(u => !groupMemberId.Contains(u.Id) && u.IsDeleted==0);
			if (context.GroupMembers.FirstOrDefault(u => u.GrpId == groupId && u.ProfileId == userId).Admin==1)
			{
				foreach (var profile in profiles) {
					var profileModel = new AddMemberToGroupModel
					{
						UserName= profile.UserName,
						ImagePath = profile.ImagePath,
						FirstName = profile.FirstName,
						LastName = profile.LastName,
					};
					allProfiles.Add(profileModel);
				}
				return allProfiles;
			}
			return null;
		}

		public IEnumerable<GroupMemberList> addMembersToGroup(int grpId , string[] selUsers , string userName)
		{
			var allProfiles = new List<GroupMemberList>();
			int userId = chatService.GetIdFromUserName(userName);
			List<int> selUserIds = new List<int>();
			if (context.GroupMembers.FirstOrDefault(u => u.ProfileId == userId).Admin == 1) 
			{
				foreach (var selUser in selUsers)
				{
					int selUserId = chatService.GetIdFromUserName(selUser);
					selUserIds.Add(selUserId);
					if (!context.GroupMembers.Any(u=>u.GrpId==grpId && u.ProfileId==selUserId)) {
						GroupMembers newMember = new GroupMembers
						{
							GrpId = grpId,
							ProfileId = selUserId,
							JoinedAt = DateTime.Now,
							Admin = 0,
						};
						context.GroupMembers.Add(newMember);
						context.SaveChanges();

						Profile profile = context.Profiles.FirstOrDefault(u => u.UserName == selUser);
						//Response
						var newObj = new GroupMemberList
						{
							ImagePath = profile.ImagePath,
							Admin = newMember.Admin,
							FirstName = profile.FirstName,
							LastName = profile.LastName,
							UserName = profile.UserName,
						};
						allProfiles.Add(newObj);
					}
				}
				var sortedMessages = context.GroupMessages.OrderByDescending(m => m.CreatedAt);
				Groups group = context.Groups.FirstOrDefault(m => m.Id == grpId);
				var newGrp = new RecentGroupModel
				{
					Id = grpId,
					GroupImage = group.ImagePath,
					GroupName = group.GroupName,
				};
				var msg = sortedMessages.FirstOrDefault(m => m.GrpId == grpId);
				if (msg != null)
				{
					newGrp.Type = msg.Type;
					newGrp.LastMsgAt = msg.CreatedAt;
					newGrp.LastMsg = msg.Content;
				}
				else
				{
					newGrp.Type = null;
					newGrp.LastMsgAt = null;
					newGrp.LastMsg = null;
				}
				foreach (var selId in selUserIds)
				{
					var connect = context.Connections.FirstOrDefault(u => u.ProfileId == selId);
					if (connect != null)
					{
						this.hubContext.Clients.Client(connect.SignalId).SendAsync("iAmAddedToGroup", newGrp);
					}
				}
				this.notificationServices.addToGroup(selUserIds ,userId ,grpId);
				return allProfiles;
			}
			return null;
		}

		public GroupLoadModel updateGroup(string userName, CreateGroupModel grp,int grpId)
		{
			int userId = chatService.GetIdFromUserName(userName);
			if (context.GroupMembers.FirstOrDefault(u => u.GrpId == grpId && u.ProfileId == userId).Admin==1){
				Groups existingGroup = context.Groups.FirstOrDefault(u => u.Id == grpId);
				existingGroup.GroupName = grp.GroupName;
				if (grp.Description != null)
				{ existingGroup.Description = grp.Description; }
				if (grp.ImageFile != null)
				{
					var filename = Guid.NewGuid().ToString(); // new generated image file name
					var uploads = Path.Combine(webHostEnvironment.WebRootPath, @"grpimages");
					var extension = Path.GetExtension(grp.ImageFile.FileName);// Get Extension Of the File

					if (existingGroup.ImagePath != null)
					{
						var oldImagePath = Path.Combine(webHostEnvironment.WebRootPath + existingGroup.ImagePath);
						if (System.IO.File.Exists(oldImagePath))
						{
							System.IO.File.Delete(oldImagePath);
						}
					}

					using (var fileStreams = new FileStream(Path.Combine(uploads, filename + extension), FileMode.Create))
					{
						grp.ImageFile.CopyTo(fileStreams);
					}
					existingGroup.ImagePath = "/grpimages/" + filename + extension;
				}
				context.Groups.Update(existingGroup);
				context.SaveChanges();
				Profile profile = context.Profiles.FirstOrDefault(u=> u.Id == userId);
				var groupModel = new GroupLoadModel
				{
					Id = grpId,
					ImagePath = existingGroup.ImagePath,
					CreatedAt = existingGroup.CreatedAt,
					GroupName = existingGroup.GroupName,
					Description = existingGroup.Description,
					CreatedBy = profile.FirstName + " " +profile.LastName,
				};
				//SignalR
				List<int> groupMembersId = context.GroupMembers.Where(u => u.GrpId == grpId).Select(u => u.ProfileId).ToList();
				var connect = context.Connections.Where(u => groupMembersId.Contains(u.ProfileId)).Select(u => u.SignalId);
				this.hubContext.Clients.Clients(connect).SendAsync("GroupUpdated", groupModel);
			}
			return null;
		}

		public void leaveGroup(string userName , int groupId)
		{
			int admins = context.GroupMembers.Where(u=>u.GrpId==groupId && u.Admin==1).Count();
			int userId = chatService.GetIdFromUserName(userName);
			int numberOfUsers = context.GroupMembers.Where(u => u.GrpId == groupId).Count();
			if (numberOfUsers == 1)
			{
				IEnumerable<GroupMessages> groupmessages = context.GroupMessages.Where(u=>u.GrpId==groupId).ToList();
				if (groupmessages != null) {
					context.GroupMessages.RemoveRange(groupmessages);
					context.SaveChanges();
				}		
			}
			GroupMembers member = context.GroupMembers.FirstOrDefault(u=>u.GrpId== groupId && u.ProfileId==userId);
			context.RemoveRange(member);
			context.SaveChanges();
			if(admins==1 && numberOfUsers>1 && member.Admin==1)
			{
				GroupMembers firstMember = context.GroupMembers.Where(u=>u.GrpId== groupId).OrderBy(u=>u.JoinedAt).First() ;
				firstMember.Admin = 1;
				context.GroupMembers.UpdateRange(firstMember);
				context.SaveChanges();
				var connect = context.Connections.FirstOrDefault(u => u.ProfileId == firstMember.ProfileId);
				if (connect != null)
				{
					this.hubContext.Clients.Client(connect.SignalId).SendAsync("MadeMeAdmin", groupId);
				}
			}
			if(numberOfUsers == 1)
			{
				var notifications = context.Notifications.Where(u => u.GroupId == groupId);
				context.Notifications.RemoveRange(notifications);
				context.SaveChanges();
				Groups group = context.Groups.FirstOrDefault(u => u.Id == groupId);
				context.Groups.RemoveRange(group);
				context.SaveChanges();
			}
		}
	
		public void makeAdmin(int groupId, string selUserName, string userName)
		{
			int userId = chatService.GetIdFromUserName(userName);
			int selUserId = chatService.GetIdFromUserName(selUserName);
			if (context.GroupMembers.Any(u => u.GrpId == groupId && u.ProfileId == userId && u.Admin == 1))
			{
				GroupMembers member = context.GroupMembers.FirstOrDefault(u => u.GrpId == groupId && u.ProfileId == selUserId && u.Admin == 0);
				if (member != null)
				{
					member.Admin = 1;
					context.GroupMembers.Update(member);
					context.SaveChanges();

					//Hub Req To Make User Admin Instantly
					var connect = context.Connections.FirstOrDefault(u => u.ProfileId == selUserId);
					if (connect != null)
					{
						this.hubContext.Clients.Client(connect.SignalId).SendAsync("MadeMeAdmin", groupId);
					}
					this.notificationServices.adminNoti(selUserId, userId, groupId);
				}
			}
		}

		public void removeUser(int groupId,string selUserName, string userName)
		{
			int selUserId = chatService.GetIdFromUserName(selUserName);
			int userId = chatService.GetIdFromUserName(userName);
			GroupMembers selMember = context.GroupMembers.FirstOrDefault(u => u.ProfileId == selUserId);
			GroupMembers userMember = context.GroupMembers.FirstOrDefault(u => u.ProfileId == userId);
			if(selMember!= null && selMember.Admin!=1 && userMember!=null && userMember.Admin==1) 
			{ 
				context.GroupMembers.Remove(selMember);
				context.SaveChanges();
				Connections connect = context.Connections.FirstOrDefault(s => s.ProfileId == selUserId);
				if (connect != null)
				{
					this.hubContext.Clients.Client(connect.SignalId).SendAsync("iAmRemovedFromGroup", groupId);
				}
				this.notificationServices.removeNoti(selUserId, userId, groupId);
			}
		}

		public IEnumerable<GMessageDTO> GetAllMessage(int groupId) 
		{
			var returnList = new List<GMessageDTO>();
			var response = new List<GMessageDTO>();
			var groupMessages = context.GroupMessages.Where(u=>u.GrpId== groupId).ToList();
			if (groupMessages.Count > 0)
			{
				foreach (var groupMessage in groupMessages)
				{
					var profile = context.Profiles.FirstOrDefault(u => u.Id == groupMessage.MessageFrom);
					var newObj = new GMessageDTO
					{
						Id= groupMessage.Id,
						Content= groupMessage.Content,
						MessageFrom = profile.UserName,
						MessageFromImage = profile.ImagePath,
						CreatedAt = (DateTime)groupMessage.CreatedAt,
						Type= groupMessage.Type,
					};
					if (groupMessage.RepliedTo == -1)
					{
						newObj.RepliedTo = null;
					}
					else
					{
						var message = context.GroupMessages.FirstOrDefault(u => u.Id == groupMessage.RepliedTo);
						newObj.RepliedTo = message.Content;
					}
					returnList.Add(newObj);
				}
				response = returnList.OrderBy(e => e.CreatedAt).ToList();
				return response;
			}
			return null;
		}

		public void SendFileMessage(GMessageInModel msg)
		{
			var message = new GroupMessages();
			var filename = Guid.NewGuid().ToString(); // new generated image file name
			var extension = Path.GetExtension(msg.File.FileName);// Get Extension Of the File
			var filetype = msg.File.ContentType.Split('/')[0];
			string uploads;
			if (filetype == "audio")
			{
				uploads = Path.Combine(webHostEnvironment.WebRootPath, @"group/audio");
				message.Content = "/group/audio/" + filename + extension;
				message.Type = "audio";
			}
			else if (filetype == "video")
			{
				uploads = Path.Combine(webHostEnvironment.WebRootPath, @"group/videos");
				message.Content = "/group/videos/" + filename + extension;
				message.Type = "video";
			}
			else
			{
				uploads = Path.Combine(webHostEnvironment.WebRootPath, @"group/images");
				message.Content = "/group/images/" + filename + extension;
				message.Type = "image";
			}

			using (var fileStreams = new FileStream(Path.Combine(uploads, filename + extension), FileMode.Create))
			{
				msg.File.CopyTo(fileStreams);
			}

			message.GrpId = msg.GroupId;
			message.MessageFrom = chatService.GetIdFromUserName(msg.MessageFrom);
			message.RepliedTo = -1;
			message.CreatedAt = DateTime.Now;

			context.GroupMessages.Add(message);
			context.SaveChanges();

			var Profile = context.Profiles.FirstOrDefault(u=>u.UserName==msg.MessageFrom);

			var response = new GMessageDTO()
			{
				Id = message.Id,
				Content = message.Content,
				CreatedAt = (DateTime)message.CreatedAt,
				MessageFrom = msg.MessageFrom,
				MessageFromImage = Profile.ImagePath,
				RepliedTo = null,
				Type = message.Type,
			};
			var groupMemberIds = context.GroupMembers.Where(u => u.GrpId == msg.GroupId).Select(u => u.ProfileId).ToList();
			var connections = context.Connections.Where(u => groupMemberIds.Contains(u.ProfileId)).Select(u => u.SignalId).ToList();
			this.hubContext.Clients.Clients(connections).SendAsync("RecieveMessageGroup", response);
			this.notificationServices.groupMsgNoti(message.MessageFrom, msg.GroupId, message.Type);
		}
	}
}
