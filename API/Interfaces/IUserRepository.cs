namespace API.Interfaces
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    
    using API.Data.Entities;
    using API.DTos;
    using API.Helpers;

    public interface IUserRepository
    {

        void Update(AppUser user);
        Task<IEnumerable<AppUser>> GetUsersAsync();
        Task<AppUser> GetUserByIdAsync(string id);
        Task<AppUser> GetUserByUsernameAsync(string username);
        Task<PagedList<MemberDTo>> GetMembersAsync(UserParams userParams);
        Task<MemberDTo> GetMemberAsync(string username);

        Task<string> GetUserGender(string username);

    }
}