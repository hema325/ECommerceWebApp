using DataAccess.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.DataAccessRepository.IRepository
{
    public interface IItemOptionRepository:IEntityRepository<ItemOption>
    {
        Task<bool> Add(IEnumerable<ItemOption> itemOptions);
        Task<bool> DeleteAsync(int itemId, int optionId);
    }
}
