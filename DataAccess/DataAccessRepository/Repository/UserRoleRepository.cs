using DataAccess.Data;
using DataAccess.DataAccessRepository.IRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.DataAccessRepository.Repository
{
    public class UserRoleRepository:EntityRepository<UserRole>,IUserRoleRepository
    {
        public UserRoleRepository(string connectionString) : base(connectionString) { }
    }
}
