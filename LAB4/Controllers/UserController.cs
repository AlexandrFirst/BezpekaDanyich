using LAB4.Data;
using LAB4.Data.Models;
using LAB4.Dtos.User;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace LAB4.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UserController: ControllerBase
    {
        private readonly ChatContext context;
        public UserController(ChatContext context)
        {
            this.context = context;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] UserLoginRequest userLoginRequest) 
        {
            var nameToAuthenticate = userLoginRequest.UserName;

            if (string.IsNullOrEmpty(nameToAuthenticate)) {
                return BadRequest(new {Message = "Name can not be null or empty"});
            }

            var dbUser = await context.Users.FirstOrDefaultAsync(x => x.Name.ToLower().Equals(nameToAuthenticate.ToLower()));
            if (dbUser is null) 
            {
                var user = new User()
                {
                    Name = userLoginRequest.UserName
                };

                await context.Users.AddAsync(user);
                await context.SaveChangesAsync();

                dbUser = user;
            }

            var loginResponse = MapDbUserToResponse(dbUser);
            return Ok(loginResponse);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetUserById(int id) 
        {
            var dbUser = await context.Users.FirstOrDefaultAsync(x => x.Id == id);
            if (dbUser == null)
            {
                return NotFound(new { Message = "No authenticated user found" });
            }
            else 
            {
                var loginResponse = MapDbUserToResponse(dbUser);
                return Ok(loginResponse);
            }
        }

        private UserLoginResponse MapDbUserToResponse(User dbUser) 
        {
            var loginResponse = new UserLoginResponse()
            {
                UserId = dbUser.Id,
                UserChatInfos = dbUser.RUserChats.Select(x => new UserChatInfo()
                {
                    ChatId = x.ChatInfoId,
                    ChatName = x.ChatInfo.Name
                }).ToList()
            };
            return loginResponse;
        }
    }
}
