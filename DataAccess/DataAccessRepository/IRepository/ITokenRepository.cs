using DataAccess.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.DataAccessRepository.IRepository
{
    public interface ITokenRepository:IEntityRepository<Token>
    {
        Task<Token> FindByValueAsync(string value, IEnumerable<string> attrs = null);
        Task<bool> DeleteByValueAsync(string value);
        Task<bool> DeleteByUserIdAndPurposeAsync(int userId,Token.Purposes purpose);
        Task CleanUpAsync();
    }
}
