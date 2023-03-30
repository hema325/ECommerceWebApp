using DataAccess.Data;
using DataAccess.DataAccessRepository.IRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.DataAccessRepository.Repository
{
    public class RoleRepository:EntityRepository<Role>,IRoleRepository
    {
        public RoleRepository(string connectionString) : base(connectionString) { }

        public async Task<Role> GetRoleByNameAsync(string name, IEnumerable<string> attrs = null)
        {
            if (attrs == null)
                attrs = EntityProps;

            var sql = $"Select {string.Join(",", attrs)} from Roles Where Name=@name";
            return await QueryFirstOrDefaultAsync<Role>(sql, new { name = name });
        }
    }
}
