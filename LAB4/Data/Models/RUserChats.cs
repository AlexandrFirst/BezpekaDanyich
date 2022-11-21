using System.ComponentModel.DataAnnotations.Schema;

namespace LAB4.Data.Models
{
    public class RUserChats
    {
        public int UserChatId { get; set; }

        [ForeignKey(nameof(User))]
        public int UserId { get; set; }
        public virtual User User { get; set; }

        [ForeignKey(nameof(ChatInfo))]
        public int ChatInfoId { get; set; }
        public virtual ChatInfo ChatInfo { get; set; }
        
        public int ConnectionSecretKey { get; set; }
    }
}
