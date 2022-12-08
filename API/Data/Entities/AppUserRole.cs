using Microsoft.AspNetCore.Identity;

namespace API.Data.Entities
{
    public class AppUserRole: IdentityUserRole<string>
    {
        public AppUser User { get; set; }
        public AppRole Role { get; set; }
        
    }
}