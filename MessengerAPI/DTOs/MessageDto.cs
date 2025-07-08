﻿namespace MessengerAPI.DTOs
{
    public class MessageDto
    {
        public int Id { get; set; }
        public string Content { get; set; } = string.Empty;
        public DateTime Timestamp { get; set; }
        public int SenderId { get; set; }
        public int ReceiverId { get; set; }
    }
}