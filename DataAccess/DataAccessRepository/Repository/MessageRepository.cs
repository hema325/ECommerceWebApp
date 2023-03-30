using DataAccess.Data;
using DataAccess.DataAccessRepository.IRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.DataAccessRepository.Repository
{
    public class MessageRepository:EntityRepository<Message>,IMessageRepository
    {
        public MessageRepository(string connectionString) : base(connectionString) 
        {
            IgnoredProps.Add(nameof(Message.Sender));
        }

        public async Task<IEnumerable<Message>> GetByConvId(int convId, int? skip = null, int? take = null)
        {
            var sql = new StringBuilder()
                .Append(@$"select Ms.Id,Ms.TimeStamp,Ms.Value,Ms.IsRead,Us.Id,Us.ImgUrl,Us.IsOnline from Messages as Ms
                           join Users as Us on Us.Id = Ms.SenderId
                           where Ms.ConversationId = @convId
                           order by Ms.TimeStamp desc");
                       
            if(skip != null)
            {
                sql.Append(" ")
                    .Append("offset @skip rows");
                if (take != null)
                    sql.Append(" ")
                        .Append("fetch next @take rows only");
            }

            return await QueryAsync<Message, User, Message>(sql.ToString(), (msg, user) =>
            {
                msg.Sender = user;
                return msg;
            }, new { convId = convId, skip = skip, take = take });
        }

        public async Task<IEnumerable<Message>> GetByConvIdUserId(int convId,int userId, int? skip = null, int? take = null)
        {
            var sql = new StringBuilder()
                .Append(@$"select Ms.Id,Ms.TimeStamp,Ms.Value,Ms.IsRead,Us.Id,Us.ImgUrl,Us.IsOnline from Messages as Ms
                           join UserConversations as UCs on UCs.ConversationId = Ms.ConversationId
                           join Users as Us on Us.Id = Ms.SenderId
                           where Ms.ConversationId = @convId and (Ms.TimeStamp < UCs.LeftDateTime or UCs.LeftDateTime is null) and Ms.TimeStamp > UCs.JoinDateTime and UCs.UserId = @userId
                           order by Ms.TimeStamp desc");

            if (skip != null)
            {
                sql.Append(" ")
                    .Append("offset @skip rows");
                if (take != null)
                    sql.Append(" ")
                        .Append("fetch next @take rows only");
            }

            return await QueryAsync<Message, User, Message>(sql.ToString(), (msg, user) =>
            {
                msg.Sender = user;
                return msg;
            }, new { convId = convId, skip = skip, take = take, userId = userId });
        }

        public async Task<int> GetConvHasUnReadMsgsCount(int convId, int userId)
        {
            var sql = @"select count(*) as convHasUnReadMsgsCount from  Messages
                        where ConversationId = @convId and IsRead = 0 and SenderId != @userId";

            return await QueryFirstOrDefaultAsync<int>(sql, new { convId = convId, userId = userId });
        }

        public async Task<bool> MarkAsReadAsync(int convId,int ownerId)
        {
            var sql = "Update Messages Set IsRead = 1 where conversationId = @convId and senderId != @ownerId";

            return await ExecuteAsync(sql, new { convId = convId, ownerId = ownerId });
        }

        public async Task<bool> MarkAsReadAsync(long msgId)
        {
            var sql = "update messages set isRead = 1 where id = @msgId";
            return await ExecuteAsync(sql, new { msgId = msgId });
        }

    }
}
