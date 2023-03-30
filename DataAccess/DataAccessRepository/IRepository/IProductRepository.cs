using DataAccess.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.DataAccessRepository.IRepository
{
    public interface IProductRepository:IEntityRepository<Product>
    {
        Task<IEnumerable<Product>> GetAllIncludeCategoryAsync(IEnumerable<string> attrs = null, IEnumerable<string> categoryAttrs = null, int? skip = null, int? take = null, string splitOn = "id");
        Task<IEnumerable<Product>> GetByNameIncludeCategoryAsync(string name, IEnumerable<string> attrs = null, IEnumerable<string> categoryAttrs = null, int? skip = null, int? take = null, string splitOn = "id");
        Task<Product> FindByIdIncludeAllAsync(int id, IEnumerable<string> attrs = null, IEnumerable<string> categoryAttrs = null, IEnumerable<string> discountAttrs = null, string splitOn = "id");
    }
}
