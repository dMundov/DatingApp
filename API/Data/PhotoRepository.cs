namespace API.Data
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using API.Data.Entities;
    using API.DTos;
    using API.Interfaces;
    using Microsoft.EntityFrameworkCore;

    public class PhotoRepository : IPhotoRepository
    {
        private readonly ApplicationDbContext _context;

        public PhotoRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Photo> GetPhotoById(string id)
        {
            return await _context.Photos
                .IgnoreQueryFilters()
                .SingleOrDefaultAsync(x=>x.Id==id);
        }

        public async Task<IEnumerable<PhotoForApprovalDto>> GetUnapprovedPhotos()
        {
            return await _context.Photos
                .IgnoreQueryFilters()
                .Where(p=>p.isApproved == false)
                .Select(u => new PhotoForApprovalDto
                {
                    Id = u.Id,
                    Username = u.AppUser.UserName,
                    Url = u.Url,
                    IsApproved = u.isApproved
                }).ToListAsync();
        }

        public void RemovePhoto(Photo photo)
        {
            _context.Photos.Remove(photo);
        }
    }
}