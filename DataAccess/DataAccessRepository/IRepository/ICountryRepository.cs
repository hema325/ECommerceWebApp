using DataAccess.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.DataAccessRepository.IRepository
{
    public interface ICountryRepository : IEntityRepository<Country>
    {
        Task<IEnumerable<Country>> GetByNameAsync(string name, IEnumerable<string> attrs = null, IEnumerable<string> orderByAttrs = null, int? skip = null, int? take = null);
    }
}
