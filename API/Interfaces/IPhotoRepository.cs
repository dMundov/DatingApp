namespace API.Interfaces
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using API.Data.Entities;
    using API.DTos;

    public interface IPhotoRepository
    {
        Task<IEnumerable<PhotoForApprovalDto>> GetUnapprovedPhotos();
        Task<Photo> GetPhotoById (string id);
        void RemovePhoto(Photo photo);
        
    }
}