using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.DataAccessRepository.IRepository
{
    public interface IEntityRepository<TEntity> : ISqlRepository
    {
        Task<TOutput> AddAsync<TOutput>(TEntity entity, string output = "id");
        Task<bool> AddAsync(TEntity entity);
        Task<bool> UpdateAsync(TEntity entity, IEnumerable<string> attrs = null);
        Task<bool> DeleteByIdAsync(int id);
        Task<TEntity> FindByIdAsync(int id, IEnumerable<string> attrs = null);
        Task<IEnumerable<TEntity>> GetAllAsync(IEnumerable<string> attrs = null, IEnumerable<string> orderByAttrs = null, int? skip = null, int? take=null);
        Task<int> CountAsync();
    }
}
