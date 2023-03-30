using DataAccess.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.DataAccessRepository.IRepository
{
    public interface IItemRepository:IEntityRepository<Item>
    {
        Task<IEnumerable<Item>> GetAllIncludeProductAsync(IEnumerable<string> attrs = null, IEnumerable<string> productAttrs = null, int? skip = null, int? take = null, string splitOn = "id");
        Task<IEnumerable<Item>> GetByNameIncludeProductAsync(string productName, IEnumerable<string> attrs = null, IEnumerable<string> productAttrs = null, int? skip = null, int? take = null, string splitOn = "id");
        Task<IEnumerable<int>> GetCategoriesIdsAsync(int id);
        Task<Item> GetItemDetailsAsync(int id);
        Task<IEnumerable<Item>> GetITemsCardDetailsAsync(int? categoryId = null, string sortBy = null, decimal? minPrice = null, decimal? maxPrice = null, int? skip = null, int? take = null);
        Task<IEnumerable<Item>> GetITemsCardDetailsByNameAsync(string name,int? skip = null, int? take = null);
    }
}
