using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using API.Data;
using API.Data.Entities;
using API.DTos;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
    public class AccountController:BaseApiController
    {
        private readonly ApplicationDbContext context;

        public AccountController(ApplicationDbContext context)
        {
            this.context = context;
        }

        [HttpPost("register")]
        public async Task<ActionResult<AppUser>> Register(RegisterDto registerDto)
        {

            if(await UserExists(registerDto.UserName)) 
            {
                return BadRequest("Username is taken");
            }
            using HMACSHA512 hmac= new HMACSHA512();

            var user= new AppUser
            {
                UserName = registerDto.UserName.ToLower(),
                PasswordHash=hmac.ComputeHash(Encoding.UTF8.GetBytes(registerDto.Password)),
                PasswordSalt=hmac.Key
            };

            this.context.Users.Add(user);
            await this.context.SaveChangesAsync();

            return user;
        }

        [HttpPost("login")]
        public async Task<ActionResult<AppUser>> Login(LoginDTo loginDTo)
        {
            AppUser user = await this.context.Users.SingleOrDefaultAsync(x => x.UserName == loginDTo.Username);

            if(user == null) return Unauthorized("Invalid Username");
            using HMACSHA512 hmac = new HMACSHA512(user.PasswordSalt);

            var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(loginDTo.Password));

            for ( int i = 0; i < computedHash.Length; i++)
            {
                if(computedHash[i]!= user.PasswordHash[i]) return Unauthorized("Invalid Password!");
            }

            return user;

        }
        private async Task<bool> UserExists(string username)
        {
            return await this.context.Users.AnyAsync(x => x.UserName == username.ToLower());
        }
    }
}