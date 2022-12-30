namespace API.Data.Entities
{
    using System;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("Photos")]
    public class Photo
    {
        public Photo()
        {
            this.Id = Guid.NewGuid().ToString();
        }

        public string Id { get; set; }
        public string Url { get; set; }
        public bool IsMain { get; set; }
        public bool isApproved { get; set; }
        public string PublicId { get; set; }

        public int AppUserId { get; set; }
        public virtual AppUser AppUser { get; set; }
    }
}