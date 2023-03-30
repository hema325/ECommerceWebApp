using DataAccess.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.DataAccessRepository.IRepository
{
    public interface ICategoryRepository:IEntityRepository<Category>
    {
        Task<Category> FindByIdIncludeParent(int id, IEnumerable<string> attrs = null, IEnumerable<string> parentAttrs = null, string splitOn = "id");
        Task<IEnumerable<Category>> GetByNameAsync(string name, IEnumerable<string> attrs = null, IEnumerable<string> orderByAttrs = null, int? skip = null, int? take = null);

        Task<IEnumerable<Category>> GetCategoriesHierarchyAsync();
    }
}
