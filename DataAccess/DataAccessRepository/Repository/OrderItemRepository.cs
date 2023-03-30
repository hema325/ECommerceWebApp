using DataAccess.Data;
using DataAccess.DataAccessRepository.IRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.DataAccessRepository.Repository
{
    public class OrderItemRepository:EntityRepository<OrderItem>, IOrderItemRepository
    {
        public OrderItemRepository(string connectionString) : base(connectionString) 
        {
            IgnoredProps.Add(nameof(OrderItem.Item));
        }

        public async Task<bool> AddAsync(IEnumerable<OrderItem> orderItems)
        {
            var sql = $@"insert into orderItems(OrderId,ItemId,Price,Quantity) values 
                         {string.Join(", ", orderItems.Select(orderItem => $"({orderItem.OrderId},{orderItem.ItemId},{orderItem.Price},{orderItem.Quantity})"))}";
            return await ExecuteAsync(sql);
        }

        public async Task<IEnumerable<OrderItem>> GetByOrderIdIncludeProductAsync(int orderId)
        {

            var sql = new StringBuilder($@"select OITs.Price,OITs.Quantity,Ps.Name from items as ITs
                                           join Products as Ps on Ps.Id = ITs.ProductId
                                           join OrderItems as OITs on OITs.ItemId = ITs.Id
                                           where OITs.OrderId = @orderId");


            return await QueryAsync<OrderItem, Product, OrderItem>(sql.ToString(), (orderItem, product) =>
            {
                orderItem.Item = new Item();
                orderItem.Item.Product = product; 
                return orderItem;
            }, new { orderId = orderId }, splitOn: "Name");

        }

    }
}
