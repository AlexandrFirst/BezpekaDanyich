using LAB4.Dtos.Chat;
using LAB4.Dtos.Message;
using LAB4.Services;
using Lab4_Client.Models;
using Microsoft.AspNetCore.SignalR.Client;
using Newtonsoft.Json;
using System;
using System.Diagnostics;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Lab4_Client
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            Console.ReadKey();
            Console.WriteLine("Welcome to chat\nEnter your name: ");

            string userName = Console.ReadLine();

            using HttpClient client = new HttpClient();

            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            client.BaseAddress = new Uri("http://localhost:5000/");

            UserLoginRequest userLoginRequest = new UserLoginRequest()
            {
                UserName = userName,
            };

            string loginPayload = JsonConvert.SerializeObject(userLoginRequest);
            HttpContent loginContent = new StringContent(loginPayload, Encoding.UTF8, "application/json");
            var enterResponse = await client.PostAsync("user/login", loginContent);

            if (!enterResponse.IsSuccessStatusCode)
            {
                Console.WriteLine("Something went wrong, rerun program");
            }

            var loginResponseContent = await enterResponse.Content.ReadAsStringAsync();
            var loginResponseModel = JsonConvert.DeserializeObject<UserLoginResponse>(loginResponseContent);

            var userId = loginResponseModel.UserId;


            CreateChatRequest chatCreateRequest = new CreateChatRequest()
            {
                Name = "My chat"
            };

            string createChatPayload = JsonConvert.SerializeObject(chatCreateRequest);
            HttpContent createChatContent = new StringContent(createChatPayload, Encoding.UTF8, "application/json");
            var createChatResponse = await client.PostAsync("chat/create", createChatContent);

            if (!createChatResponse.IsSuccessStatusCode)
            {
                Console.WriteLine("Error while creating chat");
            }

            var createChatResponseContent = await createChatResponse.Content.ReadAsStringAsync();
            var createChatResponseModal = JsonConvert.DeserializeObject<CreateChatResponse>(createChatResponseContent);

            var chatId = createChatResponseModal.Id;
            var chatName = createChatResponseModal.Name;

            Console.WriteLine("Welcome to chat " + chatName + $"({chatId})");


            ChatInfoRequest chatInfoRequest = new ChatInfoRequest()
            {
                ChatId = chatId,
                UserId = userId
            };

            string chatInfoRequestPayload = JsonConvert.SerializeObject(chatInfoRequest);
            HttpContent chatInfoRequestContent = new StringContent(chatInfoRequestPayload, Encoding.UTF8, "application/json");
            var chatInfoResponse = await client.PostAsync("chat/info", chatInfoRequestContent);

            if (!chatInfoResponse.IsSuccessStatusCode)
            {
                Console.WriteLine("Error while retrieving chat info");
            }

            var chatInfoResponseContent = await chatInfoResponse.Content.ReadAsStringAsync();
            var chatInfoResponseModel = JsonConvert.DeserializeObject<ChatInfoResponse>(chatInfoResponseContent);

            //createing secret key
            Random random = new Random(Guid.NewGuid().GetHashCode());

            var secretKey = random.Next(1, chatInfoResponseModel.Prime);
            var chatSharedKey_BA = Convert.FromBase64String(chatInfoResponseModel.ChatPublicKey);
            var chatSharedKey = new BigInteger(chatSharedKey_BA);

            BigInteger mySharedKey = BigInteger.ModPow(chatInfoResponseModel.PrimitiveRootModule,
                secretKey, chatInfoResponseModel.Prime);

            ChatEncodingRequest chatEncodingRequest = new ChatEncodingRequest()
            {
                ChatId = chatId,
                ClientsKey = Convert.ToBase64String(mySharedKey.ToByteArray()),
                UserId = userId
            };

            string chatEncodingRequestPayload = JsonConvert.SerializeObject(chatEncodingRequest);
            HttpContent chatEncodingRequestContent = new StringContent(chatEncodingRequestPayload, Encoding.UTF8, "application/json");
            var chatEncodingResponse = await client.PostAsync("chat/chatEncoding", chatEncodingRequestContent);

            if (!chatEncodingResponse.IsSuccessStatusCode)
            {
                Console.WriteLine("Error while getting chat encoding");
            }

            var chatEncodingResponseContent = await chatEncodingResponse.Content.ReadAsStringAsync();
            var chatEncodingResponseModel = JsonConvert.DeserializeObject<ChatEncodingResponse>(chatEncodingResponseContent);

            var sharedKey = BigInteger.ModPow(chatSharedKey, secretKey, chatInfoResponseModel.Prime);

            Console.WriteLine("Shared key:" + sharedKey);
            Cypher cypher = Cypher.CreateCypher("some salt message");

            var chatEncoding_Encoded = Convert.FromBase64String(chatEncodingResponseModel.EncodedEncodingKey);

            var decoded_chatEncoding = cypher.DecryptMessage(sharedKey.ToByteArray(), chatEncoding_Encoded);

            var origEncoding = chatEncodingResponseModel.NotEncodedEncodingKey;

            Console.WriteLine("decoded_chatEncoding:" + Convert.ToBase64String(decoded_chatEncoding));
            Console.WriteLine("origEncoding: " + origEncoding);


            ProcessStartInfo startinfo = new ProcessStartInfo();
            startinfo.FileName = @"E:\\nure\\BezpekaDanych\\BezpekaDanyich\\Lab4_messageView\\bin\\Debug\\netcoreapp3.1\\Lab4_messageView.exe";
            startinfo.CreateNoWindow = true;
            startinfo.UseShellExecute = true;
            startinfo.ArgumentList.Add(Convert.ToBase64String(decoded_chatEncoding));
            startinfo.ArgumentList.Add(userId.ToString());
            startinfo.ArgumentList.Add(chatId.ToString());
            Process myProcess = Process.Start(startinfo);

            var connection = new HubConnectionBuilder()
                .WithUrl("http://localhost:5000/chat", Microsoft.AspNetCore.Http.Connections.HttpTransportType.WebSockets, opt =>
                {
                    opt.SkipNegotiation = true;
                })
                .WithAutomaticReconnect(new[] { TimeSpan.FromSeconds(10), TimeSpan.FromSeconds(20) })
                .Build();

            await connection.StartAsync();

            await connection.InvokeAsync("EnterGroup", chatId, userName);

            while (true) 
            {
                Console.Write("Enter message: ");
                var message = Console.ReadLine();
                if (message.Equals("q"))
                {
                    break;
                }
                else 
                {
                    
                    InputMessage inputMessage = new InputMessage()
                    {
                        Message = message,
                        UserId = userId
                    };

                    var serializedMessage = JsonConvert.SerializeObject(inputMessage);
                    var encodedMessage = cypher.EncryptMessage(decoded_chatEncoding, Encoding.Unicode.GetBytes(serializedMessage));

                    var messageToSend = Convert.ToBase64String(encodedMessage);

                    await connection.InvokeAsync("SendMessageToGroup", chatId, messageToSend);
                }
            }
            Console.WriteLine("Press any key to exit");
            Console.ReadKey();

            await connection.InvokeAsync("LeaveGroup", chatId, userName);

            myProcess.Kill();
        }
    }
}
