namespace API.Interfaces
{
    using API.Data.Entities;
    public interface ITokenService
    {
        string CreateToken(AppUser user);
    }
}