using System;

namespace API.Data.Entities
{
    public class Photo
    {
        public Photo()
        {
            this.Id = Guid.NewGuid().ToString();
        }

        public string Id { get; set; }
        public string Url {get;set;}
        public bool IsMain { get; set; }
        public string  PublicId { get; set; }

        public int AppUserId {get;set;}
        public virtual AppUser AppUser {get;set;}
    }
}