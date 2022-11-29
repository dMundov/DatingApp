namespace API.Interfaces
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using API.Data.Entities;
    using API.DTos;
    using API.Helpers;

    public interface ILikesRepository
    {
        Task<UserLike> GetUserLike(string SourceUserId, string TargetUserId);
        Task<AppUser> GetUserWithLikes(string userId);
        Task<PagedList<LikeDto>> GetUserLikes(LikesParams likesParams);

    }
}