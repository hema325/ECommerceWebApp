using DataAccess.Data;
using DataAccess.DataAccessRepository.IRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static DataAccess.Data.Order;

namespace DataAccess.DataAccessRepository.Repository
{
    public class OrderRepository:EntityRepository<Order>,IOrderRepository
    {
        public OrderRepository(string connectionString) : base(connectionString) 
        {
            IgnoredProps.Add(nameof(Order.OrderItems));
            IgnoredProps.Add(nameof(Order.Address));
        }

        public async Task<IEnumerable<Order>> GetByUserIdAsync(int userId,IEnumerable<string> attrs = null)
        {
            if (attrs == null)
                attrs = EntityProps;

            var sql = $"select {string.Join(",", attrs)} from Orders where UserId = @userId";

            return await QueryAsync<Order>(sql, new { userId = userId });
        }

        public async Task<IEnumerable<Order>> GetByStatusAsync(Statuses status, IEnumerable<string> attrs = null, IEnumerable<string> orderByAttrs = null, int? skip = null, int? take = null)
        {
            if (attrs == null)
                attrs = EntityProps;

            var sql = new StringBuilder()
                .Append($"select {string.Join(",", attrs)} from Orders where status = @status");

            if (orderByAttrs != null)
            {
                sql.Append(" ")
                    .Append($"order by {string.Join(",", orderByAttrs)}");

                if (skip != null)
                {
                    sql.Append(" ")
                        .Append($"Offset @skip Rows");

                    if (take != null)
                    {
                        sql.Append(" ")
                            .Append($"Fetch next @take Rows Only");
                    }
                }
            }

            return await QueryAsync<Order>(sql.ToString(), new { status = status, skip = skip, take = take });
        }

        public async Task<Order> GetOrderDetailsByIdAsync(int id)
        {
            var sql = @$"select ODs.Id,ODs.TimeStamp,ODs.Status,ODs.Total,OITs.Id,OITs.Price,OITs.Quantity,ITs.Id,ITs.ImgUrl,Ps.Id,Ps.Name,ADs.Id,ADs.City,ADs.State,ADs.StreetAddress,ADs.PostalCode,Cs.Id,Cs.Name
                         from  Orders as ODs
                         join OrderItems as OITs on OITs.OrderId = ODs.Id
                         join Items as ITs on ITs.Id = OITs.ItemId
                         join Products as Ps on Ps.Id = ITs.ProductId
                         join Addresses as ADs on ADs.Id = ODs.AddressId
                         join Countries as Cs on Cs.Id = ADs.CountryId
                         where ODs.Id = @id";

            Order output = null;

            await QueryAsync<Order,OrderItem,Item,Product,Address,Country,Order>(sql,(order,orderItem,item,product,address,country)=>
            {
                if (output==null)
                    output = order;

                if (output.OrderItems == null)
                    output.OrderItems = new List<OrderItem>();

                output.Address = address;
                output.OrderItems.Add(orderItem);
                orderItem.Item = item;
                item.Product = product;
                address.Country = country;

                return order;
            },new { id = id });

            return output;
        }
    }
}
