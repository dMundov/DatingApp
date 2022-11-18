namespace API.Controllers
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;

    using API.DTos;
    using API.Interfaces;
    using AutoMapper;
    using System.Security.Claims;
    using Microsoft.AspNetCore.Http;
    using API.Extensions;
    using API.Data.Entities;
    using System.Linq;
    using API.Helpers;

    [Authorize]
    public class UsersController : BaseApiController
    {
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;
        private readonly IPhotoService _photoService;
        public UsersController(IUserRepository userRepository, IMapper mapper, IPhotoService photoService)
        {
            _photoService = photoService;
            _mapper = mapper;
            _userRepository = userRepository;

        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<MemberDTo>>> GetUsers([FromQuery]UserParams userParams)
        {
            var users = await _userRepository.GetMembersAsync(userParams);

            Response.AddPaginationHeader(users.CurrentPage,users.PageSize, users.TotalCount,users.TotalPages);
            return Ok(users);
        }

        [HttpGet("{username}", Name = "GetUser")]
        public async Task<ActionResult<MemberDTo>> GetUser(string username)
        {
            return await _userRepository.GetMemberAsync(username);
        }

        [HttpPut]
        public async Task<ActionResult> UpdateUser(MemberUpdateDto memberUpdateDto)
        {
            var userName = User.GetUserName();
            var user = await _userRepository.GetUserByUsernameAsync(userName);

            _mapper.Map(memberUpdateDto, user);

            _userRepository.Update(user);
            if (await _userRepository.SaveAllAsync()) return NoContent();

            return BadRequest("Failed to update user");
        }

        [HttpPost("add-photo")]
        public async Task<ActionResult<PhotoDto>> AddPhoto(IFormFile file)
        {
            var user = await _userRepository.GetUserByUsernameAsync(User.GetUserName());

            var result = await _photoService.AddPhotoAsync(file);

            if (result.Error != null) return BadRequest(result.Error.Message);


            var photo = new Photo
            {
                Url = result.SecureUrl.AbsoluteUri,
                PublicId = result.PublicId
            };

            if (user.Photos.Count == 0)
            {
                photo.IsMain = true;
            }

            user.Photos.Add(photo);

            if (await _userRepository.SaveAllAsync())
            {
                return CreatedAtRoute("GetUser", new {Username=user.UserName} ,  _mapper.Map<PhotoDto>(photo));
                
            }

            return BadRequest("Problem Adding Photo!");
        }

        [HttpPut("set-main-photo/{photoId}")]
        public async Task<ActionResult> SetMainPhoto(string photoId)
        {
            var user = await _userRepository.GetUserByUsernameAsync(User.GetUserName());

            var photo = user.Photos.FirstOrDefault(x=>x.Id == photoId);

            if(photo.IsMain) return BadRequest("This is already your main photo");

            var currentMain = user.Photos.FirstOrDefault(x=>x.IsMain);

            if(currentMain != null) currentMain.IsMain=false;
            photo.IsMain = true;

            if(await _userRepository.SaveAllAsync()) return NoContent();

            return BadRequest("Failed to set main photo");

        }
        [HttpDelete("delete-photo/{photoId}")]
        public async Task<ActionResult> DeletePhoto( string photoId)
        {
            var user = await _userRepository.GetUserByUsernameAsync(User.GetUserName());

            var photo = user.Photos.FirstOrDefault(x => x.Id == photoId);

            if(photo == null) return NotFound();

            if(photo.IsMain) return BadRequest("You cannot delete your main photo!");

            if ( photo.PublicId != null)
            {
                var results = await _photoService.DeletePhotoAsync(photo.PublicId);
                if( results.Error != null) return BadRequest(results.Error.Message);
            }

            user.Photos.Remove(photo);
            if(await _userRepository.SaveAllAsync()) return Ok();
            
            return BadRequest("Failed to delete photo!");
        
        }

    }
}