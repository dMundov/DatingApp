namespace API.Data
{
    using System.Collections.Generic;
    using System.Security.Cryptography;
    using System.Text;
    using System.Text.Json;
    using System.Threading.Tasks;
    using Microsoft.EntityFrameworkCore;

    using API.Data.Entities;
    using Microsoft.AspNetCore.Identity;

    public class Seed
    {
        public static async Task SeedUsers(UserManager<AppUser> userManager)
        {
            if (await userManager.Users.AnyAsync()) return;

            var userData = await System.IO.File.ReadAllTextAsync("Data/UserSeedData.json");
            var users = JsonSerializer.Deserialize<List<AppUser>>(userData);

            foreach (var user in users)
            {         
                user.UserName = user.UserName.ToLower();

                await userManager.CreateAsync(user,"633Ds725");
            }
        }
    }
}