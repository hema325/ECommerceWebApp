using DataAccess.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.DataAccessRepository.IRepository
{
    public interface ICartItemRepository :IEntityRepository<CartItem>
    {
        Task<bool> DeleteByUserIdAsync(int userId);
        Task<IEnumerable<CartItem>> GetCartDetailsAsync(int userId, int? skip = null, int? take = null);
        Task<int> CountByUserIdAsync(int userId);
        Task<bool> IsExist(int userId, int itemId);
    }
}
