﻿namespace LAB4.Dtos.Chat
{
    public class ChatEncodingRequest
    {
        public int ChatId { get; set; }
        public int UserId { get; set;}
        public byte[] ClientsKey { get; set; }
    }
}
