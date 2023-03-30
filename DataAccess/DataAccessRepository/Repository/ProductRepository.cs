using DataAccess.Data;
using DataAccess.DataAccessRepository.IRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.DataAccessRepository.Repository
{
    public class ProductRepository:EntityRepository<Product>,IProductRepository
    {
        public ProductRepository(string connectionString) : base(connectionString) 
        {
            IgnoredProps.Add(nameof(Product.Category));
            IgnoredProps.Add(nameof(Category.Parent));
            IgnoredProps.Add(nameof(Product.Discount));
            IgnoredProps.Add(nameof(Category.Children));
        }

        public async Task<IEnumerable<Product>> GetAllIncludeCategoryAsync(IEnumerable<string> attrs = null, IEnumerable<string> categoryAttrs = null, int? skip = null, int? take = null,string splitOn="id")
        {
            if (attrs == null)
                attrs = EntityProps;

            if (categoryAttrs == null)
                categoryAttrs = GetProps<Category>();

            var sql = new StringBuilder()
                .Append($@"select {string.Join(",",attrs.Select(attr=>$"Ps.{attr}"))},{string.Join(",",categoryAttrs.Select(attr=>$"Cs.{attr}"))} from Products as Ps 
                        left join Categories as Cs on Cs.Id = Ps.CategoryId");

            if (skip != null)
            {
                sql.Append(" ")
                    .Append("order by Ps.Id offset @skip rows ");

                if (take != null)
                    sql.Append(" ")
                        .Append("fetch next @take rows only");
            }


            return await QueryAsync<Product, Category, Product>(sql.ToString(), (product, category) =>
            {
                product.Category = category;
                return product;
            }, new { skip = skip, take = take }, splitOn: splitOn);
        }

        public async Task<IEnumerable<Product>> GetByNameIncludeCategoryAsync(string name,IEnumerable<string> attrs = null, IEnumerable<string> categoryAttrs = null, int? skip = null, int? take = null, string splitOn = "id")
        {
            if (attrs == null)
                attrs = EntityProps;

            if (categoryAttrs == null)
                categoryAttrs = GetProps<Category>();

            var sql = new StringBuilder()
                .Append($@"select {string.Join(",", attrs.Select(attr => $"Ps.{attr}"))},{string.Join(",", categoryAttrs.Select(attr => $"Cs.{attr}"))} from Products as Ps 
                        left join Categories as Cs on Cs.Id = Ps.CategoryId 
                        where Ps.Name like @name + '%'");

            if (skip != null)
            {
                sql.Append(" ")
                    .Append("order by Ps.Id offset @skip rows ");

                if (take != null)
                    sql.Append(" ")
                        .Append("fetch next @take rows only");
            }


            return await QueryAsync<Product, Category, Product>(sql.ToString(), (product, category) =>
            {
                product.Category = category;
                return product;
            }, new { name = name, skip = skip, take = take }, splitOn: splitOn);
        }

        public async Task<Product> FindByIdIncludeAllAsync(int id, IEnumerable<string> attrs = null, IEnumerable<string> categoryAttrs = null, IEnumerable<string> discountAttrs = null, string splitOn = "id")
        {
            if (attrs == null)
                attrs = EntityProps;

            if (categoryAttrs == null)
                categoryAttrs = GetProps<Category>();

            if (discountAttrs == null)
                discountAttrs = GetProps<Discount>();

            var sql = @$"select {string.Join(",", attrs.Select(attr => $"Ps.[{attr}]"))},{string.Join(",", categoryAttrs.Select(attr => $"Cs.[{attr}]"))},{string.Join(",", discountAttrs.Select(attr => $"Ds.[{attr}]"))}
                        from products as Ps
                        join Categories as Cs on Cs.Id = Ps.CategoryId
                        left join Discounts as Ds on Ds.Id = Ps.DiscountId
                        where Ps.Id = @id";

            Product output = null;

            await QueryAsync<Product, Category, Discount, Product>(sql, (product, category, discount) =>
            {
                output = product;
                output.Category = category;
                output.Discount = discount;

                return product;
            }, new { id = id },splitOn);

            return output;
        }

    }
}
