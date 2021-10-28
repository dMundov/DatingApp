namespace API.Data
{
    using API.Data.Entities;
    using Microsoft.EntityFrameworkCore;

    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext()
        {
        }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }


        public DbSet<AppUser> Users { get; set; }

        public DbSet<Photo> Photos { get; set; }
    }
}