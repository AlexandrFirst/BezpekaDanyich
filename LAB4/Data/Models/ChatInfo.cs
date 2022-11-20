using System.Collections.Generic;

namespace LAB4.Data.Models
{
    public class ChatInfo
    {
        public ChatInfo()
        {
            RUserChats = new List<RUserChats>();
        }

        public int Id { get; set; }
        public string Name { get; set; } 

        public byte[] SecretKey { get; set; } // Key to e/d messages

        public int Prime { get; set; } // params to establish secure connection
        public int PrimitiveRootModule { get; set; } // params to establish secure connection
        public virtual List<RUserChats> RUserChats { get; set; }
    }
}
