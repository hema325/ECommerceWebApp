using DataAccess.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.DataAccessRepository.IRepository
{
    public interface IAddressRepository:IEntityRepository<Address>
    {
        Task<IEnumerable<Address>> GetByUserId(int userId, IEnumerable<string> attrs = null);
    }
}
