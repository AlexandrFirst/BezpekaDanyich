
using LAB4.Data;
using LAB4.Dtos.Message;
using LAB4.Services;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Text;
using System.Threading.Tasks;

namespace LAB4.Hub
{
    public class UsersHub: Hub<IUserHub>
    {
        private readonly ChatContext context;
        private readonly ICypher cyper;
  
        public UsersHub(ChatContext context, ICypher cyper)
        {
            this.context = context;
            this.cyper = cyper;
        }

        public async Task SendMessageToGroup(int chatId, string encodedMessageInfo) 
        {
            var chat = await context.ChatInfos.FirstOrDefaultAsync(x => x.Id == chatId);

            var b_decodedMessage = cyper.DecryptMessage(chat.SecretKey, Encoding.Unicode.GetBytes(encodedMessageInfo));
            var decodedMessage = Encoding.Unicode.GetString(b_decodedMessage);

            InputMessage inputMessage = JsonConvert.DeserializeObject<InputMessage>(decodedMessage);

            var user = await context.Users.FirstOrDefaultAsync(x => x.Id == inputMessage.UserId);

            var messageToSend = new OutputMessage() { Message = inputMessage.Message, Name = user.Name, UserId = user.Id };
            var serializedMessage = JsonConvert.SerializeObject(messageToSend);
            var encodedMessage = cyper.EncryptMessage(chat.SecretKey, Encoding.Unicode.GetBytes(serializedMessage));

            await Clients.Group(chatId.ToString()).SendMessageToChat(Encoding.Unicode.GetString(encodedMessage));
        }

        public async Task EnterGroup(int chatId, string userName) 
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, chatId.ToString());
            await Clients.Group(chatId.ToString()).EnterGroup(userName);
        }

        public async Task LeaveGroup(int chatId, string userName)
        {
            await Clients.Group(chatId.ToString()).LeaveGroup(userName);
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, chatId.ToString());
        }
    }
}
