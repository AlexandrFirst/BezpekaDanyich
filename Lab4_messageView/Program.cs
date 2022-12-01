using LAB4.Dtos.Message;
using LAB4.Services;
using Microsoft.AspNetCore.SignalR.Client;
using Newtonsoft.Json;
using System;
using System.Text;
using System.Threading.Tasks;

namespace Lab4_messageView
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            var key = args[0];
            var userId = args[1];
            var chatId = args[2];

            Cypher cypher = Cypher.CreateCypher("some salt message");

            var connection = new HubConnectionBuilder()
                .WithUrl("http://localhost:5000/chat", Microsoft.AspNetCore.Http.Connections.HttpTransportType.WebSockets, opt =>
                {
                    opt.SkipNegotiation = true;
                })
                .WithAutomaticReconnect(new[] { TimeSpan.FromSeconds(10), TimeSpan.FromSeconds(20) })
                .Build();

            var encryptionKey = Convert.FromBase64String(key);

            connection.On<string>("SendMessageToChat", (string data) =>
            {
                var decryptedMessage = cypher.DecryptMessage(encryptionKey, Convert.FromBase64String(data));
                var messageConetnt = Encoding.Unicode.GetString(decryptedMessage);

                OutputMessage outputMessage = JsonConvert.DeserializeObject<OutputMessage>(messageConetnt);
                Console.WriteLine($"{outputMessage.Name}({outputMessage.UserId}): ${outputMessage.Message}");
            });

            connection.On<string>("EnterGroup", (string username) =>
            {
                Console.WriteLine($"{username} entered the chat");
            });

            connection.On<string>("LeaveGroup", (string username) =>
            {
                Console.WriteLine($"{username} left the chat");
            });

            await connection.StartAsync();
            await connection.InvokeAsync("EnterGroup", int.Parse(chatId), "Chat for user: " + chatId);


            System.Console.WriteLine("Listening to messages...");

            Console.ReadKey();

            await connection.InvokeAsync("LeaveGroup", int.Parse(chatId), "Chat for user: " + chatId);
        }
    }
}
