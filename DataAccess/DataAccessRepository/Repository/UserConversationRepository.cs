using DataAccess.Data;
using DataAccess.DataAccessRepository.IRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.DataAccessRepository.Repository
{
    public class UserConversationRepository:EntityRepository<UserConversation>,IUserConversationRepository
    {
        public UserConversationRepository(string connectionString) : base(connectionString) { }


        //this method is not safe for strings you'll have to check it first
        public async Task<bool> AddMultipleAsync(IEnumerable<UserConversation> userConvs)
        {
            var sql = new StringBuilder()
                .Append($"insert into UserConversations (UserId,ConversationId,JoinDateTime) values");

            //dapper doesn't support bulk insert and i'll alway use this method to insert integeres so ->

            foreach (var userConv in userConvs) 
            {
                sql.Append(" ")
                   .Append($"({userConv.UserId},{userConv.ConversationId},'{userConv.JoinDateTime.ToString("yyyy/MM/dd hh:mm:ss")}')")
                   .Append(",");
            }

            //remove last ,
            --sql.Length;

            return await ExecuteAsync(sql.ToString());

        }

        public async Task<bool> UpdateLeftDateTime(int convId,int userId,DateTime? dateTime)
        {
            var sql = $@"update userConversations set LeftDateTime = @dateTime where ConversationId = @convId and userId = @userId";

            return await ExecuteAsync(sql, new { convId = convId, userId = userId, dateTime = dateTime });
        }

        public async Task<UserConversation> FindByConvIdUserId(int convId, int userId, IEnumerable<string> attrs = null)
        {
            if (attrs == null)
                attrs = EntityProps;

            var sql = @$"select {string.Join(",",attrs)} from UserConversations where conversationId = @convId and UserId = @userId";

            return await QueryFirstOrDefaultAsync<UserConversation>(sql, new { convId = convId, userId = userId });
        }

        public async Task<bool> IsUserExistsInConv(int userId, int convId)
        {
            var sql = "select count(*) from UserConversations where userId = @userId and conversationId = @convId";

            return await QueryFirstOrDefaultAsync<int>(sql, new { userId = userId, convId = convId }) > 0;
        }
    }
}
