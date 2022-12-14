using LAB4.Data;
using LAB4.Data.Models;
using LAB4.Dtos.Chat;
using LAB4.Dtos.Message;
using LAB4.Hub;
using LAB4.Services;
using LAB4.Utils;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using System;
using System.Linq;
using System.Numerics;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace LAB4.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ChatController : ControllerBase
    {
        private readonly ChatContext context;
        private readonly PrimeNumberGenerator primeNumberGenerator;
        private readonly PrimitiveRootGenerator primitiveRootGenerator;
        private readonly Utils.RandomNumberGenerator randomNumberGenerator;
        private readonly ICypher cypher;

        public ChatController(ChatContext context,
            PrimeNumberGenerator primeNumberGenerator,
            PrimitiveRootGenerator primitiveRootGenerator,
            Utils.RandomNumberGenerator randomNumberGenerator,
            ICypher cypher)
        {
            this.context = context;
            this.primeNumberGenerator = primeNumberGenerator;
            this.primitiveRootGenerator = primitiveRootGenerator;
            this.randomNumberGenerator = randomNumberGenerator;
            this.cypher = cypher;
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreateChat([FromBody] CreateChatRequest chatCreateRequest)
        {
            var chatNameToCreate = chatCreateRequest.Name;

            if (string.IsNullOrEmpty(chatNameToCreate))
            {
                return BadRequest(new { Message = "Name can not be null or empty" });
            }

            var dbChat = await context.ChatInfos.FirstOrDefaultAsync(x => x.Name.ToLower().Equals(chatNameToCreate.ToLower()));
            if (dbChat != null)
            {
                return Ok(new CreateChatResponse() { Id = dbChat.Id, Name = dbChat.Name });
            }
            else
            {
                var primeNumber = primeNumberGenerator.Next();
                var rootModule = primitiveRootGenerator.Generator(primeNumber);

                var randomNumber = randomNumberGenerator.GetRandomNumber(primeNumber);

                var key = BigInteger.ModPow(rootModule, randomNumber, primeNumber);


                var chatToCreate = new ChatInfo()
                {
                    Name = chatNameToCreate,
                    Prime = primeNumber,
                    PrimitiveRootModule = rootModule,
                    SecretKey = key.ToByteArray(),
                };

                await context.ChatInfos.AddAsync(chatToCreate);

                await context.SaveChangesAsync();

                return Ok(new CreateChatResponse() { Id = chatToCreate.Id, Name = chatToCreate.Name });
            }
        }


        [HttpPost("info")]
        public async Task<IActionResult> GetChatInfo([FromBody] ChatInfoRequest chatInfoRequest)
        {
            var chatDb = await context.ChatInfos.FirstOrDefaultAsync(x => x.Id == chatInfoRequest.ChatId);
            if (chatDb == null)
            {
                return NotFound(new { Message = "Chat with id: " + chatInfoRequest.ChatId + " is not found" });
            }

            var connectionSecretKey = randomNumberGenerator.GetRandomNumber(chatDb.Prime);
            var key = BigInteger.ModPow(chatDb.PrimitiveRootModule, connectionSecretKey, chatDb.Prime);

            var response = new ChatInfoResponse()
            {
                Id = chatDb.Id,
                ChatPublicKey = Convert.ToBase64String(key.ToByteArray()),
                Prime = chatDb.Prime,
                Name = chatDb.Name,
                PrimitiveRootModule = chatDb.PrimitiveRootModule
            };


            var rUserChatInfo = new RUserChats()
            {
                ChatInfoId = chatInfoRequest.ChatId,
                UserId = chatInfoRequest.UserId,
                ConnectionSecretKey = connectionSecretKey
            };

            var connections = await context.RUserChats.Where(x => x.ChatInfoId == chatInfoRequest.ChatId &&
                x.UserId == chatInfoRequest.UserId).ToListAsync();

            if (connections.Any())
            {
                context.RUserChats.RemoveRange(connections);
            }

            await context.RUserChats.AddAsync(rUserChatInfo);
            await context.SaveChangesAsync();

            return Ok(response);
        }

        [HttpPost("chatEncoding")]
        public async Task<IActionResult> GetChatEncoding([FromBody] ChatEncodingRequest chatEncodingRequest)
        {
            int chatId = chatEncodingRequest.ChatId;

            var chatDb = await context.ChatInfos.FirstOrDefaultAsync(x => x.Id == chatId);
            if (chatDb == null) 
            {
                return NotFound(new { Message = "Chat with id: " + chatId + " is not found" });
            }

            var rUserChatInfo = await context.RUserChats.FirstOrDefaultAsync(x => x.ChatInfoId == chatEncodingRequest.ChatId &&
                x.UserId == chatEncodingRequest.UserId);
            if (rUserChatInfo is null)
            {
                return BadRequest(new { Message = "Unable to verify connection" });
            }

            var userByteArray = Convert.FromBase64String(chatEncodingRequest.ClientsKey);

            BigInteger clientSharedKey = new BigInteger(userByteArray);

            var sharedKey = BigInteger.ModPow(clientSharedKey, rUserChatInfo.ConnectionSecretKey, chatDb.Prime);

            Console.WriteLine("Shared key:" + sharedKey);

            var encodingKey = cypher.EncryptMessage(sharedKey.ToByteArray(), chatDb.SecretKey);


            Console.WriteLine("EncodedEncodingKey:" + Convert.ToBase64String(encodingKey));
            Console.WriteLine("origEncoding: " + Convert.ToBase64String(chatDb.SecretKey));

            return Ok(new ChatEncodingResponse()
            {
                EncodedEncodingKey = Convert.ToBase64String(encodingKey),
                NotEncodedEncodingKey = Convert.ToBase64String(chatDb.SecretKey)
            });
        }
    }
}
