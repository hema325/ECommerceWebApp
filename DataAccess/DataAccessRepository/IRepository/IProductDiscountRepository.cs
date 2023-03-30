using DataAccess.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.DataAccessRepository.IRepository
{
    public interface IProductDiscountRepository:IEntityRepository<ProductDiscount>
    {
        Task<bool> AddAsync(IEnumerable<ProductDiscount> productDiscounts);
        Task<bool> DeleteAsync(int productId, int discountId);
    }
}
