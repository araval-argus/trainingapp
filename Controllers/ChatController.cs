using ChatApp.Business.ServiceInterfaces;
using ChatApp.Context.EntityClasses;
using ChatApp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using Microsoft.AspNetCore.Http;
using System.IO;
using Microsoft.AspNetCore.Hosting;
using ChatApp.Business.Helpers;

namespace ChatApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ChatController : ControllerBase
    {

        private readonly IChatService chatService;
        private readonly IProfileService profileService;
        private readonly IWebHostEnvironment webHostEnvironment;

        public ChatController(IChatService chatService, IProfileService profileService, IWebHostEnvironment webHostEnvironment)
        {
            this.chatService = chatService;
            this.profileService = profileService;
            this.webHostEnvironment = webHostEnvironment;
        }

        #region endpoints
        //this is for searching the users starting their firstname with searchTerm
        [HttpGet("fetchFriends")]
        public IActionResult FetchFriends(string searchTerm)
        {

            Console.WriteLine(searchTerm);
            IEnumerable<FriendProfileModel> en = this.chatService.FetchFriendsProfiles(searchTerm);
            foreach (FriendProfileModel profile in en)
            {
                Console.WriteLine(profile.FirstName);
            }
            return Ok(new { message = en });
        }

        [HttpPost("addMessage")]
        public IActionResult AddMessage([FromBody] MessageModelToSendMessage message)
        {
            MessageModel messageModel = new()
            {
                Message = message.Message,
                RecieverID = FetchIdFromUserName(message.Reciever),
                SenderID = FetchIdFromUserName(message.Sender),
                RepliedToMsg = Convert.ToInt32(message.RepliedToMsg)
            };
            var data = this.chatService.AddMessage(messageModel);
            return Ok(new { data = data });
        }

        [HttpGet("fetchMessages")]
        public IActionResult FetchMessages(string loggedInUserName, string friendUserName)
        {
            int loggedInUserId = FetchIdFromUserName(loggedInUserName);
            int friendId = FetchIdFromUserName(friendUserName);
            IEnumerable<MessageEntity> messages = this.chatService.FetchMessages(loggedInUserId, friendId).OrderBy(m => m.CreatedAt);
            
            // for marking each message as seen
            foreach(var message in messages)
            {
                if(message.SenderID == friendId && message.RecieverID == loggedInUserId)
                {
                    message.IsSeen = true;
                }
            }

            /*
                method to update all the messages
           
             */

            this.chatService.MarkMsgsAsSeen(messages);

            IEnumerable messagesToBeSent = messages.Select(
                m => new
                {
                    id = m.Id,
                    message = m.Message,
                    sender = FetchUserNameFromId(m.SenderID),
                    reciever = FetchUserNameFromId(m.RecieverID),
                    createdAt = m.CreatedAt,
                    isSeen = m.IsSeen,
                    repliedToMsg = FetchMessageFromId(m.RepliedToMsg),
                }
                ); ;
            return Ok(new { messages = messagesToBeSent });
        }


        [HttpGet("fetchAll")]
        public IActionResult FetchAllUsers(string loggedInUsername)
        {
            int id = FetchIdFromUserName(loggedInUsername);

            //list of friends that have interacted with logged in user before
            IEnumerable<FriendProfileModel> friends = this.profileService.FetchAllUsers(id);
            return Ok(new { data = friends });
        }

        [HttpPost("addFile")]
        [Authorize]
        public IActionResult AddFile([FromForm] FileMessageModel fileModel)
        {

            IFormFile file = fileModel.File;
            MessageModel messageModel = new()
            {
                RecieverID = FetchIdFromUserName(fileModel.Reciever),
                SenderID = FetchIdFromUserName(fileModel.Sender),
                RepliedToMsg = -1,
            };

            if (file.ContentType.StartsWith("image"))
            {
                messageModel.MessageType = MessageType.Image;
                messageModel.Message = CreateUniqueFile(file,"Images");
            }
            else if (file.ContentType.StartsWith("video"))
            {
                messageModel.MessageType = MessageType.Video;
                messageModel.Message = CreateUniqueFile(file, "Videos");
            }
            else if (file.ContentType.StartsWith("audio"))
            {
                messageModel.MessageType = MessageType.Audio;
                messageModel.Message = CreateUniqueFile(file, "Audios");
            }
            else
            {
                return BadRequest(new { message = "only images, videos and audios can be shared" });
            }

            this.chatService.AddMessage(messageModel);

            return Ok(new {message = "file added"});
        }

        [HttpGet("markAsRead")]
        public IActionResult MarkMsgs(string loggedInUserName, string friendUserName)
        {
            int loggedInUserId = FetchIdFromUserName(loggedInUserName);
            int friendId = FetchIdFromUserName(friendUserName);
            IEnumerable<MessageEntity> messages = this.chatService.FetchMessages(loggedInUserId, friendId).OrderBy(m => m.CreatedAt);

            // for marking each message as seen
            foreach (var message in messages)
            {
                if (message.SenderID == friendId && message.RecieverID == loggedInUserId)
                {
                    message.IsSeen = true;
                }
            }

            /*
                method to update all the messages
           
             */

            this.chatService.MarkMsgsAsSeen(messages);

            return Ok(new { messages= "message read"});
        }

        #endregion

        #region helpermethods
        int FetchIdFromUserName(string userName)
        {
            return this.profileService.FetchIdFromUserName(userName);
        }

        string FetchUserNameFromId(int id)
        {
            return this.profileService.FetchUserNameFromId(id);
        }

        string? FetchMessageFromId(int id)
        {
            return this.chatService.FetchMessageFromId(id);
        }

        //copies all data into newly created unique file and returns the name of it
        string CreateUniqueFile(IFormFile file, string folderName)
        {
            string path = webHostEnvironment.WebRootPath + @"/SharedFiles/" + folderName;
            string newFileName = Guid.NewGuid().ToString();
            string extension = Path.GetExtension(file.FileName);
            using (FileStream fileStream = new FileStream(Path.Combine(path, newFileName + extension), FileMode.Create))
            {
                file.CopyTo(fileStream);
            }
            return newFileName + extension;
        }
        #endregion

    }

}
