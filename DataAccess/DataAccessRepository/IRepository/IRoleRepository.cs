using DataAccess.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.DataAccessRepository.IRepository
{
    public interface IRoleRepository:IEntityRepository<Role>
    {
        Task<Role> GetRoleByNameAsync(string roleName, IEnumerable<string> attrs = null);
    }
}
