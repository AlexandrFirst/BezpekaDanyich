using System.Collections.Generic;

namespace LAB4.Dtos.User
{
    public class UserChatInfo { 
        public int ChatId { get; set; }
        public string ChatName { get; set; }
    }
    public class UserLoginResponse
    {
        public int UserId { get; set; }
        public List<UserChatInfo> UserChatInfos { get; set; }
    }
}
