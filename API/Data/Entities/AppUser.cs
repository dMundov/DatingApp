namespace API.Data.Entities
{
    using System;
    using System.Collections.Generic;

    public class AppUser
    {
        public AppUser()
        {
            this.Id = Guid.NewGuid().ToString();
        }

        public string Id { get; set; }
        public string UserName { get; set; }
        public byte[] PasswordHash { get; set; }
        public byte[] PasswordSalt { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string KnownAs { get; set; }
        public DateTime CreatedOn { get; set; } = DateTime.Now;
        public DateTime LastActive { get; set; } = DateTime.Now;
        public string Gender { get; set; }
        public string Introduction { get; set; }
        public string LookingFor { get; set; }
        public string Interests { get; set; }
        public string City { get; set; }
        public string Country { get; set; }
        public virtual ICollection<Photo> Photos { get; set; } = new HashSet<Photo>();

        public virtual List<UserLike> LikedByUser { get; set; }
        public virtual List<UserLike> LikedUsers { get; set; }
    }
}