using ChatApp.Context.EntityClasses;
using ChatApp.Models;
using System.Collections.Generic;

namespace ChatApp.Business.ServiceInterfaces
{
	public interface IAdminService
	{
		public List<Designation> getAllDesignation(string userName);
		public bool DeleteUser(string selUserName, string loginUserName);
		void UpdateUser(UpdateModel regModel, string username);
	}
}
