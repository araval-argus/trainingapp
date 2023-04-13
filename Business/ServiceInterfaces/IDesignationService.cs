using ChatApp.Context.EntityClasses;

namespace ChatApp.Business.ServiceInterfaces
{
    public interface IDesignationService
    {
        DesignationEntity AddDesignation(string name);
    }
}
