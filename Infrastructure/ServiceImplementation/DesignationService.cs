using ChatApp.Business.ServiceInterfaces;
using ChatApp.Context;
using ChatApp.Context.EntityClasses;

namespace ChatApp.Infrastructure.ServiceImplementation
{
    public class DesignationService : IDesignationService
    {
        private readonly ChatAppContext _context;

        public DesignationService(ChatAppContext context)
        {
            _context = context;
        }

        public Designation AddDesignation(string name)
        {
            Designation designation = new()
            {
                designation = name
            };
            _context.Designations.Add(designation);
            _context.SaveChanges();
            return designation;
        }
    }
}
