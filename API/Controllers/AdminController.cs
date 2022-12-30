using System.Linq;
using System.Threading.Tasks;
using API.Data;
using API.Data.Entities;
using API.Interfaces;
using API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
    public class AdminController : BaseApiController
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IPhotoService _photoService;
        public AdminController(UserManager<AppUser> userManager, IUnitOfWork unitOfWork,IPhotoService photoService)
        {
            _photoService = photoService;
            _unitOfWork = unitOfWork;
            _userManager = userManager;

        }

        [Authorize(Policy = "RequiredAdminRole")]
        [HttpGet("users-with-roles")]
        public async Task<ActionResult> GetUsersWithRoles()
        {
            var users = await _userManager.Users
                .OrderBy(u => u.UserName)
                .Select(u => new
                {
                    u.Id,
                    username = u.UserName,
                    Roles = u.UserRoles.Select(r => r.Role.Name).ToList()
                })
                .ToListAsync();

            return Ok(users);
        }

        [Authorize(Policy = "RequiredAdminRole")]
        [HttpPost("edit-roles/{username}")]
        public async Task<ActionResult> EditRoles(string username, [FromQuery] string roles)
        {
            if (string.IsNullOrEmpty(roles)) return BadRequest("You must select at least one role!");

            var selectedRoles = roles.Split(",").ToArray();

            var user = await _userManager.FindByNameAsync(username);

            if (user == null) return NotFound();

            var userRoles = await _userManager.GetRolesAsync(user);

            var result = await _userManager.AddToRolesAsync(user, selectedRoles.Except(userRoles));

            if (!result.Succeeded) return BadRequest("Failed to add to roles");

            result = await _userManager.RemoveFromRolesAsync(user, userRoles.Except(selectedRoles));

            if (!result.Succeeded) return BadRequest("Failed to remove from roles.");

            return Ok(await _userManager.GetRolesAsync(user));
        }

        [Authorize(Policy = "ModeratePhotoRole")]
        [HttpGet("photos-to-moderate")]
        public async Task<ActionResult> GetPhotosForModeration()
        {
            var photos = await _unitOfWork.PhotoRepository
                .GetUnapprovedPhotos();
            
            return Ok(photos);
        }
        
        [Authorize(Policy ="ModeratePhotoRole")]
        [HttpPost("approve-photo/{photoId}")]
        public async Task<ActionResult> ApprovePhoto(string photoId)
        {
            var photo = await _unitOfWork.PhotoRepository.GetPhotoById(photoId);
            

            if(photo == null)  return NotFound("Could not find photo!");
            
            photo.isApproved = true;

            var user = await _unitOfWork.UserRepository.GetUserByPhotoId(photoId);

            if(!user.Photos.Any(x=>x.IsMain)) photo.IsMain = true;

            await _unitOfWork.Complete();

            return Ok();
            
        }
        
        [Authorize("ModeratePhotoRole")]
        [HttpPost("reject-photo/{photoId}")]
        public async Task<ActionResult> RejectPhoto(string photoId)
        {
            var photo = await _unitOfWork.PhotoRepository.GetPhotoById(photoId);
            if(photo.PublicId != null)
            {
                var result = await _photoService.DeletePhotoAsync(photo.PublicId);
                
                if(result.Result == "Ok")
                {
                    _unitOfWork.PhotoRepository.RemovePhoto(photo);
                }
            }
            else
            {
                _unitOfWork.PhotoRepository.RemovePhoto(photo);
            }

            await _unitOfWork.Complete();

            return Ok();
        }
        
    }
}