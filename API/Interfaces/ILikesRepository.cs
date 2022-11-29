namespace API.Interfaces
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using API.Data.Entities;
    using API.DTos;

    public interface ILikesRepository
    {
        Task<UserLike> GetUserLike(string SourceUserId, string TargetUserId);
        Task<AppUser> GetUserWithLikes(string userId);
        Task<IEnumerable<LikeDto>> GetUserLikes(string predicate, string userId);

    }
}