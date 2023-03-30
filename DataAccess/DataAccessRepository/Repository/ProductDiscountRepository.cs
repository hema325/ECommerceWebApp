using DataAccess.Data;
using DataAccess.DataAccessRepository.IRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.DataAccessRepository.Repository
{
    public class ProductDiscountRepository:EntityRepository<ProductDiscount>,IProductDiscountRepository
    {
        public ProductDiscountRepository(string connectionString) : base(connectionString) { }

        public async Task<bool> AddAsync(IEnumerable<ProductDiscount> productDiscounts)
        {
            var sql = new StringBuilder()
                .Append("insert into ProductDiscounts (ProductId,DiscountId) Values");

            foreach (var item in productDiscounts)
                sql.Append(" ")
                    .Append($"({item.ProductId},{item.DiscountId})")
                    .Append(",");

            --sql.Length;

            return await ExecuteAsync(sql.ToString());
        }

        public async Task<bool> DeleteAsync(int productId,int discountId)
        {
            var sql = "delete from productDiscounts where productId = @productId and DiscountId = @discountId";

            return await ExecuteAsync(sql.ToString(), new { productId = productId, discountId = discountId });
        }

    }
}
