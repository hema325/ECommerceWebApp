using DataAccess.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.DataAccessRepository.IRepository
{
    public interface IMessageRepository:IEntityRepository<Message>
    {
        Task<IEnumerable<Message>> GetByConvId(int convId, int? skip = null, int? take = null);
        Task<IEnumerable<Message>> GetByConvIdUserId(int convId,int UserId, int? skip = null, int? take = null);
        Task<bool> MarkAsReadAsync(int convId, int ownerId);
        Task<bool> MarkAsReadAsync(long msgId);
        Task<int> GetConvHasUnReadMsgsCount(int convId, int userId);
    }
}
