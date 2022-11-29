namespace API.Data
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using API.Data.Entities;
    using API.DTos;
    using API.Extensions;
    using API.Interfaces;
    using Microsoft.EntityFrameworkCore;

    public class LikesRepository : ILikesRepository
    {
        private readonly ApplicationDbContext _context;

        public LikesRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<UserLike> GetUserLike(string sourceUserId, string targetUseriD)
        {
            return await _context.Likes.FindAsync(sourceUserId, targetUseriD);
        }

        public async Task<IEnumerable<LikeDto>> GetUserLikes(string predicate, string userId)
        {
            var users = _context.Users.OrderBy(u => u.UserName).AsQueryable();
            var likes = _context.Likes.AsQueryable();

            if (predicate == "liked")
            {
                likes = likes.Where(like => like.SourceUserId == userId);
                users = likes.Select(like => like.TargetUser);
            }

            if (predicate == "likedBy")
            {
                likes = likes.Where(like => like.TargetUserId == userId);
                users = likes.Select(like => like.SourceUser);
            }

            return await users.Select(user => new LikeDto
            {
                UserName = user.UserName,
                KnownAs = user.KnownAs,
                Age = user.DateOfBirth.CalculateAge(),
                PhotoUrl = user.Photos.FirstOrDefault(x => x.IsMain).Url,
                City = user.City,
                Id = user.Id

            }).ToListAsync();
        }

        public async Task<AppUser> GetUserWithLikes(string userId)
        {
            return await _context.Users
            .Include(x => x.LikedUsers)
            .FirstOrDefaultAsync(x => x.Id == userId);
        }
    }
}