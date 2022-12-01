using System;
using System.Collections.Generic;
using System.Text;

namespace Lab4_Client.Models
{
    public class UserChatInfo
    {
        public int ChatId { get; set; }
        public string ChatName { get; set; }
    }
    public class UserLoginResponse
    {
        public int UserId { get; set; }
        public List<UserChatInfo> UserChatInfos { get; set; }
    }
}
