using ChatApp.Context.EntityClasses;

namespace ChatApp.Business.ServiceInterfaces
{
    public interface IDesignationService
    {
        Designation AddDesignation(string name);
    }
}
