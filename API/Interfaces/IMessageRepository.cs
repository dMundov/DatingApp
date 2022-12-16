namespace API.Interfaces
{
    using System.Collections.Generic;
    using System.Threading.Tasks;   
    using API.Data.Entities;
    using API.DTos;
    using API.Helpers;

    public interface IMessageRepository
    {
        void AddMessage(Message message);
        void DeleteMessage(Message message);
        Task<Message> GetMessage(int id);
        Task<PagedList<MessageDto>> GetMessagesForUser(MessageParams messageParams);
        Task<IEnumerable<MessageDto>> GetMessageThread(string currentUserName, string recipientUserName);
        Task<bool> SaveAllAsync();

        void AddGroup(Group group);
        void RemoveConnection(Connection connection);
         Task<Connection> GetConnection(string connectionId);
         Task<Group> GetMessageGroup(string groupName);



    }
}