using DataAccess.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.DataAccessRepository.IRepository
{
    public interface IReviewRepository:IEntityRepository<Review>
    {
        Task<IEnumerable<Review>> GetByItemIdAsync(int itemId, int? skip = null, int? take = null);
    }
}
