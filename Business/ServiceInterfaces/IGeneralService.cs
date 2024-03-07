using ChatApp.Context.EntityClasses;
using ChatApp.Models.Chat;
using ChatApp.Models.GeneralMessages;
using System.Collections.Generic;

namespace ChatApp.Business.ServiceInterfaces
{
    public interface IGeneralService
    {

        IEnumerable<Areas> GetAreasList();

        IEnumerable<GeneralMessageDTO> createPost(int userId, CreatePostModel postModel);
        IEnumerable<GeneralMessageDTO> getAllMessages(int? AreaId);
    }
}
