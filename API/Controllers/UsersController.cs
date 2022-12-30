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
        private readonly IMapper _mapper;
        private readonly IPhotoService _photoService;
        private readonly IUnitOfWork _uow;
        public UsersController(IUnitOfWork uow, IMapper mapper, IPhotoService photoService)
        {
            _uow = uow;
            _photoService = photoService;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<MemberDTo>>> GetUsers([FromQuery] UserParams userParams)
        {
            var gender = await _uow.UserRepository.GetUserGender(User.GetUserName());
            userParams.CurrentUserName = User.GetUserName();

            if (string.IsNullOrEmpty(userParams.Gender))
                userParams.Gender = gender == "male" ? "female" : "male";

            var users = await _uow.UserRepository.GetMembersAsync(userParams);

            Response.AddPaginationHeader(users.CurrentPage, users.PageSize, users.TotalCount, users.TotalPages);
            return Ok(users);
        }

        [HttpGet("{username}", Name = "GetUser")]
        public async Task<ActionResult<MemberDTo>> GetUser(string username)
        {

            var currentUsername = User.GetUserName();
            return await _uow.UserRepository.GetMemberAsync(username, 
                    isCurrentUser:currentUsername == username);
        }

        [HttpPut]
        public async Task<ActionResult> UpdateUser(MemberUpdateDto memberUpdateDto)
        {
            var userName = User.GetUserName();
            var user = await _uow.UserRepository.GetUserByUsernameAsync(userName);

            _mapper.Map(memberUpdateDto, user);

            _uow.UserRepository.Update(user);
            if (await _uow.Complete()) return NoContent();

            return BadRequest("Failed to update user");
        }

        [HttpPost("add-photo")]
        public async Task<ActionResult<PhotoDto>> AddPhoto(IFormFile file)
        {
            var user = await _uow.UserRepository.GetUserByUsernameAsync(User.GetUserName());

            var result = await _photoService.AddPhotoAsync(file);

            if (result.Error != null) return BadRequest(result.Error.Message);


            var photo = new Photo
            {
                Url = result.SecureUrl.AbsoluteUri,
                PublicId = result.PublicId
            };

            user.Photos.Add(photo);

            if (await _uow.Complete())
            {
                return CreatedAtRoute("GetUser", new { Username = user.UserName }, _mapper.Map<PhotoDto>(photo));

            }

            return BadRequest("Problem Adding Photo!");
        }

        [HttpPut("set-main-photo/{photoId}")]
        public async Task<ActionResult> SetMainPhoto(string photoId)
        {
            var user = await _uow.UserRepository.GetUserByUsernameAsync(User.GetUserName());

            var photo = user.Photos.FirstOrDefault(x => x.Id == photoId);

            if (photo.IsMain) return BadRequest("This is already your main photo");

            var currentMain = user.Photos.FirstOrDefault(x => x.IsMain);

            if (currentMain != null) currentMain.IsMain = false;
            photo.IsMain = true;

            if (await _uow.Complete()) return NoContent();

            return BadRequest("Failed to set main photo");

        }
        [HttpDelete("delete-photo/{photoId}")]
        public async Task<ActionResult> DeletePhoto(string photoId)
        {
            var user = await _uow.UserRepository.GetUserByUsernameAsync(User.GetUserName());

            var photo = user.Photos.FirstOrDefault(x => x.Id == photoId);

            if (photo == null) return NotFound();

            if (photo.IsMain) return BadRequest("You cannot delete your main photo!");

            if (photo.PublicId != null)
            {
                var results = await _photoService.DeletePhotoAsync(photo.PublicId);
                if (results.Error != null) return BadRequest(results.Error.Message);
            }

            user.Photos.Remove(photo);
            if (await _uow.Complete()) return Ok();

            return BadRequest("Failed to delete photo!");

        }

    }
}