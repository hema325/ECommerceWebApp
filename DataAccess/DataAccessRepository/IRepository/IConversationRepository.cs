using DataAccess.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.DataAccessRepository.IRepository
{
    public interface IConversationRepository:IEntityRepository<Conversation>
    {
        Task<IEnumerable<User>> GetAllGroupMembers(int convId, IEnumerable<string> userAttrs = null);
        Task<IEnumerable<User>> GetGroupOnlineMembers(int convId, IEnumerable<string> userAttrs = null);
        Task<int> GetConvIdBetween(int userId1, int userId2);

        Task<IEnumerable<User>> GetConversationsAsync(int userId, int? skip = null, int? take = null);
        Task<IEnumerable<User>> GetConversationsByNameAsync(int userId, string convName, int? skip = null, int? take = null);
        Task<IEnumerable<Conversation>> GetGroupsByNameAsync(int userId, string filter, int? skip = null, int? take = null);
        Task<IEnumerable<Conversation>> GetGroupsAsync(int userId, int? skip = null, int? take = null);
        Task<int> GetConvsHasUnReadMsgsCountAsync(int userId);
        Task<int> GetAllConvMembersCount(int convId);
    }
}
