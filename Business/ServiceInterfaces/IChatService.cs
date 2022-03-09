using ChatApp.Context.EntityClasses;
using ChatApp.Models.Chat;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChatApp.Business.ServiceInterfaces
{
    public interface IChatService
    {
        IEnumerable<Chat> chatLists(int userFrom, int userTo, int limit=50);
    }
}
