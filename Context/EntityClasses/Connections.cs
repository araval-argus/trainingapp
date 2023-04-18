using Microsoft.VisualBasic;
using System;

namespace ChatApp.Context.EntityClasses
{
    public class Connections
    {
        public int Id { get; set; }
        public string ConnectionId { get; set; }
        public int User { get; set; }
        public DateTime loginTime { get; set; }
    }
}
