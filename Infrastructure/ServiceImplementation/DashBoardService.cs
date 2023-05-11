using ChatApp.Business.ServiceInterfaces;
using ChatApp.Context;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace ChatApp.Infrastructure.ServiceImplementation
{
	public class DashBoardService : IDashBoardService
	{
		private readonly ChatAppContext context;
		public DashBoardService(ChatAppContext context) {
			this.context = context;
		}

		public List<DateTime> chartDetails(out int[] chat , out int[] group , out int[] total)
		{
			DateTime startDate = new DateTime(2023, 5, 1);
			DateTime endDate = DateTime.Now.Date;
			TimeSpan totalDays = endDate - startDate;

			int[] array1 = new int[totalDays.Days + 1];
			int[] array2 = new int[totalDays.Days + 1];
			int[] array3 = new int[totalDays.Days + 1];

			List<DateTime> dateList = new List<DateTime>();

			// Loop through each date between start and end dates
			for (DateTime date = startDate; date <= endDate; date = date.AddDays(1))
			{
				dateList.Add(date);

				int count1 = context.Messages.Count(m => m.CreatedAt.Date == date);

				int count2 = context.GroupMessages.Count(m => m.CreatedAt.Date == date);

				array3[(date - startDate).Days] = count1+count2;
				array1[(date - startDate).Days] = count1;
				array2[(date - startDate).Days] = count2;
			}
			total = array3;
			chat = array1;
			group = array2;
			return dateList;
		}
	}
}
