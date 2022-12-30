namespace API.Interfaces
{
    using System.Threading.Tasks;
    public interface IUnitOfWork
    {
        IUserRepository UserRepository{get;}
        IMessageRepository MessageRepository{get;}
        ILikesRepository LikesRepository{get;}
        IPhotoRepository PhotoRepository {get;}

        Task<bool> Complete();
        bool HasChanges();
    }
}