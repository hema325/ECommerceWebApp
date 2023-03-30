using DataAccess.Data;
using DataAccess.DataAccessRepository.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.DataAccessRepository.IRepository
{
    public interface IUserRepository:IEntityRepository<User>
    {
        Task<User> FindByEmailAsync(string email, IEnumerable<string> attributes = null);
        Task<IEnumerable<User>> GetRelatedUsersAsync(int userId, IEnumerable<string> attrs = null);
        Task<IEnumerable<User>> GetRelatedUsersNotInConvIdAsync(int userId,int convId, IEnumerable<string> attrs = null);
        Task<IEnumerable<User>> GetByNameAsync(string filter, IEnumerable<string> attrs = null, IEnumerable<string> orderByAttrs = null, int? skip = null, int? take = null);
        Task<IEnumerable<User>> GetById(IEnumerable<int> usersIDs, IEnumerable<string> attrs = null);
        Task<User> FindByIdIncludeRolesAsync(int id, IEnumerable<string> userAttrs = null, IEnumerable<string> roleAttrs = null);
        Task<User> FindByEmailIncludeRolesAsync(string email, IEnumerable<string> userAttrs = null, IEnumerable<string> roleAttrs = null);
        Task<User> FindByIdIncludeAddreesesAsync(int id, IEnumerable<string> userAttrs = null, IEnumerable<string> addressAttrs = null);
    }

}
