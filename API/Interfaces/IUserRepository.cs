namespace API.Interfaces
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    
    using API.Data.Entities;
    using API.DTos;

    public interface IUserRepository
    {

        void Update(AppUser user);
        Task<bool> SaveAllAsync();
        Task<IEnumerable<AppUser>> GetUsersAsync();
        Task<AppUser> GetUserByIdAsync(string id);
        Task<AppUser> GetUserByUsernameAsync(string username);
        Task<IEnumerable<MemberDTo>> GetMembersAsync();
        Task<MemberDTo> GetMemberAsync(string username);

    }
}