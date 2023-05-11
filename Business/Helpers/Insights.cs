using ChatApp.Context.EntityClasses;
using ChatApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ChatApp.Business.Helpers
{
    public static class Insights
    {
        public static object PrepareInsights(IList<MessageEntity> personalMessages, IList<GroupMessage> groupMessages)
        {
            IList<DateTime> timestamps = new List<DateTime>();
            IList<int> personalMessagesCount = new List<int>(); 
            IList<int> groupMessagesCount = new List<int>();
            
            foreach(var personalMessage in personalMessages)
            {
                timestamps.Add(personalMessage.CreatedAt.Date);
            }
            foreach(var grupMessage in groupMessages)
            {
                timestamps.Add(grupMessage.CreatedAt.Date);
            }

            timestamps = timestamps.Distinct().OrderBy( d => d.Date).ToList();

            IList<string> datesToBeSent = new List<string>();

            foreach(var timestamp in timestamps)
            {
                datesToBeSent.Add(timestamp.ToString("MMMM dd"));
            }

            foreach(var date in timestamps)
            {
                personalMessagesCount.Add(personalMessages.Where(message => message.CreatedAt.Date == date).Count());
            }

            foreach (var date in timestamps)
            {
                groupMessagesCount.Add(groupMessages.Where(message => message.CreatedAt.Date == date).Count());
            }

            //count number of messages per day based upon these timestamps
            return (new { datesToBeSent, personalMessagesCount, groupMessagesCount });
        }
    }
}
