using DataAccess.Data;
using DataAccess.DataAccessRepository.IRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.DataAccessRepository.Repository
{
    public class TokenRepository:EntityRepository<Token>,ITokenRepository
    {
        public TokenRepository(string connectionString) : base(connectionString) { }

        public async Task<bool> DeleteByUserIdAndPurposeAsync(int userId,Token.Purposes purpose)
        {
            var sql = $"Delete from Tokens where UserId=@userId and Purpose=@purpose";
            return await ExecuteAsync(sql, new { userId = userId, purpose = purpose });
        }

        public Task<bool> DeleteByValueAsync(string value)
        {
            var sql = $"Delete from Tokens where Value = @value";
            return ExecuteAsync(sql, new { value = value });
        }

        public async Task<Token> FindByValueAsync(string value, IEnumerable<string> attrs = null)
        {
            if (attrs == null)
                attrs = EntityProps;

            var sql = $"Select {string.Join(",", attrs)} from Tokens Where Value=@value";

            return await QueryFirstOrDefaultAsync<Token>(sql, new { value = value });
        }
    }
}
