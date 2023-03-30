using DataAccess.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static DataAccess.Data.Order;

namespace DataAccess.DataAccessRepository.IRepository
{
    public interface IOrderRepository:IEntityRepository<Order>
    {
        Task<IEnumerable<Order>> GetByUserIdAsync(int userId, IEnumerable<string> attrs = null);
        Task<Order> GetOrderDetailsByIdAsync(int id);
        Task<IEnumerable<Order>> GetByStatusAsync(Statuses status, IEnumerable<string> attrs = null, IEnumerable<string> orderByAttrs = null, int? skip = null, int? take = null);
    }
}
