using DataAccess.Data;
using DataAccess.DataAccessRepository.IRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.DataAccessRepository.Repository
{
    public class CartItemRepository : EntityRepository<CartItem>, ICartItemRepository
    {
        public CartItemRepository(string connectionString) : base(connectionString)
        {
            IgnoredProps.Add(nameof(CartItem.Item));
        }

        public async Task<bool> DeleteByUserIdAsync(int userId)
        {
            var sql = $@"delete from CartItems where userId = @userId";

            return await ExecuteAsync(sql, new { userId = userId });
        }

        public async Task<bool> IsExist(int userId,int itemId)
        {
            var sql = $@"select count(*) from CartItems where userId = @userId and itemId = @itemId";

            return await QueryFirstOrDefaultAsync<int>(sql, new { userId = userId, itemId = itemId }) > 0;
        }

        public async Task<IEnumerable<CartItem>> GetCartDetailsAsync(int userId, int? skip = null, int? take = null)
        {
            var sql = $@"select CITs.Id,CITs.Quantity,ITs.Id, ITs.Price, ITs.ImgUrl, Ps.Id, Ps.Name, Ds.Id, Ds.Value, Ds.[END] from CartItems as CITs
                         join Items as ITs on ITs.Id = CITs.ItemId
                         join Products as Ps on Ps.Id = ITs.ProductId
                         left join Discounts as Ds on Ds.Id = Ps.DiscountId and Ds.Start <= getDate() and Ds.[END] >= getDate()
                         where CITs.UserId = @userId";


            return await QueryAsync<CartItem, Item, Product, Discount, CartItem>(sql.ToString(), (cartItem, item, product, discount) =>
            {
                cartItem.Item = item;
                cartItem.Item.Product = product;
                cartItem.Item.Product.Discount = discount;
                return cartItem;
            }, new { userId = userId, skip = skip, take = take });
        }

        public async Task<int> CountByUserIdAsync(int userId)
        {
            var sql = "select count(*) from CartItems where UserId = @userId";

            return await QueryFirstOrDefaultAsync<int>(sql, new { userId = userId });
        }
    }
}
