namespace LAB4.Dtos.Chat
{
    public class ChatEncodingRequest
    {
        public int ChatId { get; set; }
        public int UserId { get; set;}
        public int[] ClientsKey { get; set; }
    }
}
