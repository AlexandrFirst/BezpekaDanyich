using System.Collections.Generic;

namespace LAB4.Data.Models
{
    public class User
    {
        public User()
        {
            RUserChats = new List<RUserChats>();
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public virtual List<RUserChats> RUserChats { get; set; }
    }
}
