using DataAccess.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.DataAccessRepository.IRepository
{
    public interface IOrderItemRepository:IEntityRepository<OrderItem>
    {
        Task<bool> AddAsync(IEnumerable<OrderItem> orderItems);
        Task<IEnumerable<OrderItem>> GetByOrderIdIncludeProductAsync(int orderId);
    }
}
