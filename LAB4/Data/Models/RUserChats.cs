namespace LAB4.Data.Models
{
    public class RUserChats
    {
        public int UserChatId { get; set; }
        public virtual User User { get; set; }
        public virtual ChatInfo ChatInfo { get; set; }
    }
}
