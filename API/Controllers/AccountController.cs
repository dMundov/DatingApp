namespace API.Controllers
{

    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.EntityFrameworkCore;

    using API.Data;
    using API.DTos;
    using API.Interfaces;
    using System.Linq;
    using AutoMapper;
    using API.Data.Entities;
    using Microsoft.AspNetCore.Identity;

    public class AccountController : BaseApiController
    {

        private readonly ITokenService _tokenService;
        private readonly IMapper _mapper;
        private readonly UserManager<AppUser> _userManager;

        public AccountController(UserManager<AppUser> userManager, ITokenService tokenService, IMapper mapper)
        {
            _userManager = userManager;
            _mapper = mapper;
            _tokenService = tokenService;


        }

        [HttpPost("register")]
        public async Task<ActionResult<UserDto>> Register(RegisterDto registerDto)
        {

            if (await UserExists(registerDto.UserName))
            {
                return BadRequest("Username is taken");
            }

            var user = _mapper.Map<AppUser>(registerDto);

            user.UserName = registerDto.UserName.ToLower();

            var result = await _userManager.CreateAsync(user, registerDto.Password);

            if (!result.Succeeded) return BadRequest(result.Errors);

            var roleResults = await _userManager.AddToRoleAsync(user, "Member");
            
            if(!roleResults.Succeeded) return BadRequest(result.Errors);

            return new UserDto
            {
                Username = user.UserName,
                Token = await _tokenService.CreateToken(user),
                KnownAs = user.KnownAs,
                Gender = user.Gender
            };
        }

        [HttpPost("login")]
        public async Task<ActionResult<UserDto>> Login(LoginDTo loginDTo)
        {
            AppUser user = await _userManager.Users
            .Include(p => p.Photos)
            .SingleOrDefaultAsync(x => x.UserName == loginDTo.Username);

            if (user == null) return Unauthorized("Invalid Username");

            var result = await _userManager.CheckPasswordAsync(user, loginDTo.Password);

            if (!result) return Unauthorized("Invalid password!");

            return new UserDto
            {
                Username = user.UserName,
                Token = await _tokenService.CreateToken(user),
                PhotoUrl = user.Photos.FirstOrDefault(x => x.IsMain)?.Url,
                KnownAs = user.KnownAs,
                Gender = user.Gender
            };

        }
        private async Task<bool> UserExists(string username)
        {
            return await _userManager.Users.AnyAsync(x => x.UserName == username.ToLower());
        }
    }
}