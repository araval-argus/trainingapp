using ChatApp.Context;
using ChatApp.Context.EntityClasses;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace ChatApp.Hubs
{
    public class ChatHub : Hub
    {
        public static int ActiveUser { get; set; } = 0;
        private readonly ChatAppContext _context;
        public ChatHub(ChatAppContext context) {
            _context =context;
        }

        public override Task OnDisconnectedAsync(Exception exception)
        {
            var conId = Context.ConnectionId;

            var con = _context.Connections.FirstOrDefault(e => e.ConnectionId == conId);

            if (con != null)
            {
                _context.Remove(con);
                _context.SaveChanges();
            }

            return Task.CompletedTask;

        }


        public async Task saveConnection(string authorization)
        {
            //Getting username from client and save connection to the table
            var handler = new JwtSecurityTokenHandler();
            var token = handler.ReadJwtToken(authorization.Replace("bearer", "").Trim());
            var claim = token.Claims.FirstOrDefault(e => e.Type == "sub");

            if (claim != null)
            {
                Profile profile = _context.Profiles.AsNoTracking().FirstOrDefault(e => e.UserName.Equals(claim.Value) && e.isDeleted == 0);
                if (profile == null) {
                    return;
                }
                Connections con = _context.Connections.FirstOrDefault(e => e.User == profile.Id);
                if(con != null)
                {
                    con.ConnectionId = Context.ConnectionId;
                    con.loginTime = DateTime.Now;
                    _context.Connections.Update(con);
                }
                else
                {
                    Connections connection = new()
                    {
                        ConnectionId = Context.ConnectionId,
                        User = profile.Id,
                        loginTime = DateTime.Now,
                    };
                    _context.Connections.Add(connection);
                }
                _context.SaveChanges();
                //List Of groups name in which user is present
                List<string> groups = _context.GroupMember.AsNoTracking().Where(e => e.MemberId == profile.Id).Include(e => e.Group).Select(e => e.Group.Name).ToList();
                if(groups.Count > 0)
                {
                    foreach (var group in groups)
                    {
                        await Groups.AddToGroupAsync(Context.ConnectionId, group);
                    }
                }
                await Clients.Others.SendAsync("userIsOnline", profile.UserName);
            }
        }

        public async Task deleteConnection(string authorization)
        {
            var handler = new JwtSecurityTokenHandler();
            var token = handler.ReadJwtToken(authorization.Replace("bearer", "").Trim());
            var claim = token.Claims.FirstOrDefault(e => e.Type == "sub");
            
            if (claim != null)
            {
                Profile profile = _context.Profiles.AsNoTracking().FirstOrDefault(e=> e.UserName == claim.Value && e.isDeleted == 0 );
                int Id = profile.Id;
                Connections connection = _context.Connections.FirstOrDefault(e => e.User == Id);
                _context.Connections.Remove(connection);
                _context.SaveChanges();
                List<string> groups = _context.GroupMember.AsNoTracking().Where(e => e.MemberId == profile.Id).Include(e => e.Group).Select(e => e.Group.Name).ToList();
                if (groups.Count > 0)
                {
                    foreach (var group in groups)
                    {
                        await Groups.RemoveFromGroupAsync(Context.ConnectionId, group);
                    }
                }
                await Clients.All.SendAsync("updateStatus", claim.Value + " is Offline");
            }
        }

        public bool sendChatToActive()
        {
            return true;
        }

        public async Task seenMessage(string senderUserName)
        {
            string connectionId = Context.ConnectionId;
            //In this scenario receiver is just who have received the chat on the screen
            Connections receiverConnection = _context.Connections.AsNoTracking().FirstOrDefault(e => e.ConnectionId == connectionId);
            Profile receiver = _context.Profiles.AsNoTracking().FirstOrDefault(e => e.Id == receiverConnection.User && e.isDeleted == 0);
            Profile sender = _context.Profiles.AsNoTracking().FirstOrDefault(e=> e.UserName == senderUserName && e.isDeleted == 0 );
            Connections sendersConnectiom = _context.Connections.AsNoTracking().FirstOrDefault(e => e.User == sender.Id);
            IEnumerable<Chat> chats = _context.Chats.Where(e => e.From == sender.Id && e.To == receiver.Id && e.isRead == 0);
            foreach (Chat chat in chats)
            {
                chat.isRead = 1;
            }
            _context.Chats.UpdateRange(chats);
            _context.SaveChanges();

            if(sendersConnectiom!= null)
            {
                await Clients.Client(sendersConnectiom.ConnectionId).SendAsync("seenMessageNotification", receiver.UserName);
            }
        }


    }
}
