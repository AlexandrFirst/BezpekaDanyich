using LAB4.Data;
using LAB4.Data.Models;
using LAB4.Dtos.Chat;
using LAB4.Utils;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Numerics;
using System.Security.Cryptography;
using System.Threading.Tasks;

namespace LAB4.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ChatController: ControllerBase
    {
        private readonly ChatContext context;
        private readonly PrimeNumberGenerator primeNumberGenerator;
        private readonly PrimitiveRootGenerator primitiveRootGenerator;
        private readonly RandomNumberGenerator randomNumberGenerator;

        public ChatController(ChatContext context, 
            PrimeNumberGenerator primeNumberGenerator,
            PrimitiveRootGenerator primitiveRootGenerator,
            RandomNumberGenerator randomNumberGenerator)
        {
            this.context = context;
            this.primeNumberGenerator = primeNumberGenerator;
            this.primitiveRootGenerator = primitiveRootGenerator;
            this.randomNumberGenerator = randomNumberGenerator;
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
                    SecretKey = key.ToByteArray() ,
                };

                await context.ChatInfos.AddAsync(chatToCreate);
                await context.SaveChangesAsync();

                return Ok(new CreateChatResponse() { Id = chatToCreate.Id, Name = chatToCreate.Name });
            }
        }


    }
}
