using DataAccess.Data;
using DataAccess.DataAccessRepository.IRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.DataAccessRepository.Repository
{
    public class UserLoginRepository:EntityRepository<UserLogin>,IUserLoginRepository
    {
        public UserLoginRepository(string connectionString) : base(connectionString) { }

        public async Task<UserLogin> FindByLoginNameAndProviderKey(string loginName, string providerKey)
        {
            var sql = "select * from UserLogins where LoginName = @loginName and ProviderKey = @providerKey";

            return await QueryFirstOrDefaultAsync<UserLogin>(sql, new { loginName = loginName, providerKey = providerKey });
        }

    }
}
