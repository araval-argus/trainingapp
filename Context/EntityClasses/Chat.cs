﻿using System;

namespace ChatApp.Context.EntityClasses
{
    public class Chat
    {
        public int Id { get; set; }
        public int MessageFrom { get; set; }
        public int MessageTo { get; set; }
        public string Type { get; set; }
        public string Content { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
