using System;
using System.Collections.Generic;

namespace ChatApp.Business.ServiceInterfaces
{
	public interface IDashBoardService
	{
		public List<DateTime> chartDetails(out int[] chat, out int[] group);
	}
}
