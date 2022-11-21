using LAB4.Dtos.Message;
using System.Threading.Tasks;

namespace LAB4.Hub
{
    public interface IUserHub
    {
        Task SendMessageToChat(string encodedMessage);
        Task LeaveGroup(string userName);
        Task EnterGroup(string userName);

    }
}
