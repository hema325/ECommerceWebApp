using DataAccess.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.DataAccessRepository.IRepository
{
    public interface IUserConversationRepository:IEntityRepository<UserConversation>
    {
        Task<bool> AddMultipleAsync(IEnumerable<UserConversation> userConvs);
        Task<UserConversation> FindByConvIdUserId(int convId, int userId, IEnumerable<string> attrs = null);
        Task<bool> UpdateLeftDateTime(int convId, int userId, DateTime? dateTime);
        Task<bool> IsUserExistsInConv(int userId, int convId);
    }
}
