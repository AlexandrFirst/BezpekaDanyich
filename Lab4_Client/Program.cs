using Lab4_Client.Models;
using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace Lab4_Client
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("Welcome to chat\n Enter your name: ");

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

        }
    }
}
