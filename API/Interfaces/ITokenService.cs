namespace API.Interfaces
{
    using System.Threading.Tasks;
    using API.Data.Entities;
    
    public interface ITokenService
    {
        Task<string> CreateToken(AppUser user);
    }
}