using DataAccess.Data;
using DataAccess.DataAccessRepository.IRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.DataAccessRepository.Repository
{
    public class ItemRepository : EntityRepository<Item>, IItemRepository
    {
        public ItemRepository(string connectionString) : base(connectionString)
        {
            IgnoredProps.Add(nameof(Item.Product));
            IgnoredProps.Add(nameof(Item.Options));
            IgnoredProps.Add(nameof(Product.Category));
        }

        public async Task<IEnumerable<Item>> GetAllIncludeProductAsync(IEnumerable<string> attrs = null, IEnumerable<string> productAttrs = null, int? skip = null, int? take = null, string splitOn = "id")
        {
            if (attrs == null)
                attrs = EntityProps;

            if (productAttrs == null)
                productAttrs = GetProps<Product>();

            var sql = new StringBuilder($@"select {string.Join(",", attrs.Select(attr => $"ITs.{attr}"))},{string.Join(",", productAttrs.Select(attr => $"Ps.{attr}"))} from Items as ITs
                                           join Products as Ps on Ps.Id = ITs.ProductId");

            if (skip != null)
            {
                sql.Append(" ")
                    .Append("order by ITs.Id offset @skip rows ");

                if (take != null)
                    sql.Append(" ")
                        .Append("fetch next @take rows only");
            }


            return await QueryAsync<Item, Product, Item>(sql.ToString(), (item, product) =>
            {
                item.Product = product;
                return item;
            }, new { skip = skip, take = take }, splitOn: splitOn);
        }

        public async Task<IEnumerable<Item>> GetByNameIncludeProductAsync(string productName, IEnumerable<string> attrs = null, IEnumerable<string> productAttrs = null, int? skip = null, int? take = null, string splitOn = "id")
        {
            if (attrs == null)
                attrs = EntityProps;

            if (productAttrs == null)
                productAttrs = GetProps<Product>();

            var sql = new StringBuilder($@"select {string.Join(",", attrs.Select(attr => $"ITs.{attr}"))},{string.Join(",", productAttrs.Select(attr => $"Ps.{attr}"))} from Items as ITs
                                           join Products as Ps on Ps.Id = ITs.ProductId
                                           where Ps.Name like @productName+'%'");

            if (skip != null)
            {
                sql.Append(" ")
                    .Append("order by ITs.Id offset @skip rows ");

                if (take != null)
                    sql.Append(" ")
                        .Append("fetch next @take rows only");
            }

            return await QueryAsync<Item, Product, Item>(sql.ToString(), (item, product) =>
            {
                item.Product = product;
                return item;
            }, new { productName = productName, skip = skip, take = take }, splitOn: splitOn);

        }



        public async Task<IEnumerable<int>> GetCategoriesIdsAsync(int id)
        {
            var sql = $@"select l1.Id,l2.Id,l3.Id,l4.Id from Items as ITs
                         left join Products as Ps on Ps.Id = ITs.ProductId
                         left join Categories as l4 on l4.Id = Ps.CategoryId
                         left join Categories as l3 on l3.Id = l4.ParentId
                         left join Categories as l2 on l2.Id = l3.ParentId
                         left Join Categories as l1 on l1.Id = l1.ParentId
                         where ITs.Id = @id";

            var list = new List<int>();

            await QueryAsync<Category, Category, Category, Category, Category>(sql, (l1,l2,l3,l4) =>
            {
                if (l1 !=null && l1.Id !=0)
                    list.Add(l1.Id);

                if (l2 != null && l2.Id !=0)
                    list.Add(l2.Id);

                if (l3 != null && l3.Id != 0)
                    list.Add(l3.Id);

                if (l4 != null &&l4.Id!=0)
                    list.Add(l4.Id);

                return l1;
            }, new { id = id });

            return list;
        }

        public async Task<Item> GetItemDetailsAsync(int id)
        {
            var sql = $@"select Ps.Id,Ps.Name,Ps.Description,ITs.Id,ITs.Quantity,ITs.Price,ITs.Views,ITs.ImgUrl,Os.Id,Os.Value,Vs.Id,Vs.Name ,Ds.Id,Ds.Value from Items as ITs
                         left Join Products as Ps on Ps.Id = ITs.ProductId
                         left join ItemOptions as IOs on IOs.ItemId = ITs.Id
                         left join Options as Os on Os.Id = IOs.OptionId
                         left join Variations as Vs on Vs.Id = Os.VariationId
                         left Join Discounts as Ds on Ds.Id = Ps.DiscountId and Ds.Start <= getDate() and Ds.[End] >= getDate()
                         where ITs.Id = @id";

            Item output = null;

            await QueryAsync<Product,Item, Option, Variation,Discount, Item>(sql, (product,item, option, variation,Discount) =>
            {
                if (output == null)
                {
                    output = item;
                    output.Product = product;
                    output.Product.Discount = Discount;
                    output.Options = new List<Option>();
                }

                output.Options.Add(option);
                if(option != null)
                    option.Variation = variation;

                return item;
            }, new { id = id });

            return output;
        }


        public async Task<IEnumerable<Item>> GetITemsCardDetailsAsync(int? categoryId = null, string sortBy = null, decimal? minPrice = null, decimal? maxPrice = null, int? skip = null, int? take = null)
        {
            var sql = new StringBuilder()
                .Append(@$"select ITs.Id,ITs.Price,ITs.ImgUrl,Ps.Id,Ps.Name,Ds.Id,Ds.Value,Ds.[END] from items as ITs
                        join Products as Ps on Ps.Id = ITs.ProductId
                        left join Discounts as Ds on Ds.Id = Ps.DiscountId and Ds.Start <= getDate() and Ds.[END] >= getDate()");

            if (categoryId != null || minPrice != null || maxPrice != null)
                sql.Append(" ")
                    .Append("where");

            var flage = false;

            if (categoryId != null)
            {
                flage = true;
                sql.Append(" ")
                    .Append("Ps.CategoryId = @categoryId");
            }

            if (minPrice != null)
            {
                sql.Append(" ")
                    .Append($"{(flage ? "and" : "")} ITs.price >= @minPrice");
                flage = true;
            }

            if (maxPrice != null)
            {
                sql.Append(" ")
                    .Append($"{(flage ? "and" : "")} ITs.price <= @maxPrice");
                flage = true;
            }

            if (string.IsNullOrEmpty(sortBy))
                sql.Append(" ")
                    .Append("order by ITs.Id");
            else
                sql.Append(" ")
                    .Append($"order by {sortBy}");

            if (skip != null)
            {
                sql.Append(" ")
                    .Append("offset @skip rows ");

                if(take!= null)
                {
                    sql.Append(" ")
                        .Append("fetch next @take rows only");
                }
            }

            return await QueryAsync<Item, Product, Discount, Item>(sql.ToString(), (item, product, discount) =>
            {
                item.Product = product;
                item.Product.Discount = discount;
                return item;
            }, new { skip = skip, take = take,minPrice = minPrice,maxPrice = maxPrice,categoryId = categoryId });
        }

        public async Task<IEnumerable<Item>> GetITemsCardDetailsByNameAsync(string name,int? skip = null, int? take = null)
        {
            var sql = new StringBuilder()
                .Append(@$"select ITs.Id,ITs.Price,ITs.ImgUrl,Ps.Id,Ps.Name,Ds.Id,Ds.Value,Ds.[END] from items as ITs
                        join Products as Ps on Ps.Id = ITs.ProductId
                        left join Discounts as Ds on Ds.Id = Ps.DiscountId and Ds.Start <= getDate() and Ds.[END] >= getDate()
                        where Ps.Name like @name+'%'");

            if (skip != null)
            {
                sql.Append(" ")
                    .Append("order by ITs.Id")
                    .Append(" ")
                    .Append("offset @skip rows ");

                if (take != null)
                {
                    sql.Append(" ")
                        .Append("fetch next @take rows only");
                }
            }

            return await QueryAsync<Item, Product, Discount, Item>(sql.ToString(), (item, product, discount) =>
            {
                item.Product = product;
                item.Product.Discount = discount;
                return item;
            }, new { name=name,skip = skip, take = take });
        }

    }
}
