using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;

namespace API.Data.Entities
{
    public class AppRole : IdentityRole
    {
        public ICollection<AppUserRole> UserRoles { get; set; }
    }
}