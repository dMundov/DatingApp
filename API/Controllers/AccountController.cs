namespace API.Controllers
{
    using System.Security.Cryptography;
    using System.Text;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.EntityFrameworkCore;
    
    using API.Data;
    using API.Data.Entities;
    using API.DTos;
    using API.Interfaces;
    using System.Linq;

    public class AccountController : BaseApiController
    {
        private readonly ApplicationDbContext context;
        private readonly ITokenService tokenService;

        public AccountController(ApplicationDbContext context, ITokenService tokenService)
        {
            this.tokenService = tokenService;
            this.context = context;
        }

        [HttpPost("register")]
        public async Task<ActionResult<UserDto>> Register(RegisterDto registerDto)
        {

            if (await UserExists(registerDto.UserName))
            {
                return BadRequest("Username is taken");
            }
            using HMACSHA512 hmac = new HMACSHA512();

            var user = new AppUser
            {
                UserName = registerDto.UserName.ToLower(),
                PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(registerDto.Password)),
                PasswordSalt = hmac.Key
            };

            this.context.Users.Add(user);
            await this.context.SaveChangesAsync();

            return new UserDto
            {
                UserName = user.UserName,
                Token = this.tokenService.CreateToken(user)
            };
        }

        [HttpPost("login")]
        public async Task<ActionResult<UserDto>> Login(LoginDTo loginDTo)
        {
            AppUser user = await this.context.Users
            .Include(p=>p.Photos)
            .SingleOrDefaultAsync(x => x.UserName == loginDTo.Username);

            if (user == null) return Unauthorized("Invalid Username");
            using HMACSHA512 hmac = new HMACSHA512(user.PasswordSalt);

            var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(loginDTo.Password));

            for (int i = 0; i < computedHash.Length; i++)
            {
                if (computedHash[i] != user.PasswordHash[i]) return Unauthorized("Invalid Password!");
            }

            return new UserDto
            {
                UserName = user.UserName,
                Token = this.tokenService.CreateToken(user),
                PhotoUrl = user.Photos.FirstOrDefault(x => x.IsMain)?.Url
            };

        }
        private async Task<bool> UserExists(string username)
        {
            return await this.context.Users.AnyAsync(x => x.UserName == username.ToLower());
        }
    }
}