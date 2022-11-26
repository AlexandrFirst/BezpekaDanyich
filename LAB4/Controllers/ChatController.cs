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
using System;
using System.Numerics;
using System.Security.Cryptography;
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
                return BadRequest(new { Message = "Chat with such name exists" });
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

                var user = await context.Users.FirstOrDefaultAsync(x => x.Id == chatCreateRequest.CreatorId);
                if (user == null)
                {
                    return NotFound(new { Message = "No created for new chat found" });
                }

                user.RUserChats.Add(new RUserChats() { ChatInfo = chatToCreate });

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
                ChatPublicKey = key.ToByteArray(),
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

            BigInteger clientSharedKey = new BigInteger(chatEncodingRequest.ClientsKey);

            var sharedKey = BigInteger.ModPow(clientSharedKey, rUserChatInfo.ConnectionSecretKey, chatDb.Prime);

            var encodingKey = cypher.EncryptMessage(sharedKey.ToByteArray(), chatDb.SecretKey);

            return Ok(new ChatEncodingResponse()
            {
                EncodedEncodingKey = encodingKey
            });
        }
    }
}
