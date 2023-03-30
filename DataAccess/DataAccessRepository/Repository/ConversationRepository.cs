using DataAccess.Data;
using DataAccess.DataAccessRepository.IRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.DataAccessRepository.Repository
{
    public class ConversationRepository:EntityRepository<Conversation>,IConversationRepository
    {
        public ConversationRepository(string connectionString) : base(connectionString) 
        {
            IgnoredProps.Add(nameof(Conversation.UnReadMessagesCount));
            IgnoredProps.Add(nameof(Conversation.LastMessage));
            IgnoredProps.Add(nameof(Conversation.Users));
        }

        public async Task<IEnumerable<User>> GetAllGroupMembers(int convId, IEnumerable<string> userAttrs = null)
        {
            var sql = @$"select {string.Join(",",userAttrs)} from UserConversations as UCs
                        join Users as Us on Us.Id = UCs.UserId
                        where UCs.ConversationId = @convId and UCs.LeftDateTime is null";

            return await QueryAsync<User>(sql, new { convId = convId });
        }

        public async Task<IEnumerable<User>> GetGroupOnlineMembers(int convId, IEnumerable<string> userAttrs = null)
        {
            var sql = @$"select {string.Join(",", userAttrs)} from UserConversations as UCs
                        join Users as Us on Us.Id = UCs.UserId
                        where ConversationId = @convId and Us.IsOnline = 1 and UCs.LeftDateTime is null";

            return await QueryAsync<User>(sql, new { convId = convId });
        }

        public async Task<int> GetConvIdBetween(int userId1,int userId2)
        {
            var sql = @"select Cs.Id from UserConversations as UCs
                        join UserConversations as SLF on SLF.ConversationId = UCs.ConversationId
                        join Conversations as Cs on Cs.Id = UCs.ConversationId
                        where UCs.UserId = @userId1 and SLF.UserId = @userId2 and name is null";

            return await QueryFirstOrDefaultAsync<int>(sql, new { userId1 = userId1, userId2 = userId2 });
        }

        public async Task<IEnumerable<User>> GetConversationsAsync(int userId, int? skip = null, int? take = null)
        {
            var sql = new StringBuilder()
                .Append(@"select Us.Id,Us.FirstName,Us.LastName,Us.IsOnline,Us.ImgUrl,Cs.Id,(select count(*) from Messages where ConversationId = Cs.Id and IsRead = 0 and senderId != @id) as UnReadMessagesCount,Ms.Id,Ms.Value,Ms.TimeStamp
                          from (select Id,LastMessageId from Conversations where Id in (select ConversationId from UserConversations where userId = @id) and name is null) as Cs
                          left join Messages as Ms on Ms.Id = Cs.LastMessageId
                          join UserConversations as UCs on UCs.ConversationId = Cs.Id and UCs.UserId != @id
                          join Users as Us on Us.Id = UCs.UserId
                          order by Ms.TimeStamp desc");

            if (skip != null)
            {
                sql.Append(" ")
                    .Append("offset @skip rows");

                if (take != null)
                    sql.Append(" ")
                        .Append("fetch next @take rows only");
            }

            return await QueryAsync<User, Conversation, Message, User>(sql.ToString(), (user, conversation, message) =>
            {
                user.Conversation = conversation;
                conversation.LastMessage = message;
                return user;
            }, new { id = userId, skip = skip, take = take });
        }

        public async Task<IEnumerable<Conversation>> GetGroupsAsync(int userId, int? skip = null, int? take = null)
        {
            var sql = new StringBuilder()
                .Append(@"select Cs.Id,CS.Name,(select count(*) from Messages where SenderId != @Id and IsRead = 0 and conversationId = Cs.Id and (leftDateTime is null or leftDateTime > Ms.TimeStamp)) as UnReadMessagesCount,Ms.Id,
                          case when UCs.LeftDateTime is null then Ms.TimeStamp else UCs.LeftDateTime end as TimeStamp,
                          case when UCs.LeftDateTime is null then Ms.Value else 'You have left this group' end as Value
                          from (select ConversationId,leftDateTime from UserConversations where UserId = @Id) as UCs
                          join Conversations as Cs on Cs.Id = UCs.ConversationId
                          left join Messages as Ms on Ms.Id = Cs.LastMessageId 
                          where Cs.Name is not null
                          order by Ms.TimeStamp desc");

            if (skip != null)
            {
                sql.Append(" ")
                    .Append("offset @skip rows");

                if (take != null)
                    sql.Append(" ")
                        .Append("fetch next @take rows only");
            }

            return await QueryAsync<Conversation, Message, Conversation>(sql.ToString(), (conversation, message) =>
            {
                conversation.LastMessage = message;
                return conversation;
            }, new { id = userId, skip = skip, take = take });
        }

        public async Task<IEnumerable<User>> GetConversationsByNameAsync(int userId, string convName, int? skip = null, int? take = null)
        {
            var sql = new StringBuilder()
                .Append(@"select Us.Id,Us.FirstName,Us.LastName,Us.IsOnline,Us.ImgUrl,Cs.Id,(select count(*) from Messages where ConversationId = Cs.Id and IsRead = 0 and senderId != @id) as UnReadMessagesCount,Ms.Id,Ms.Value,Ms.TimeStamp
                          from (select Id,LastMessageId from Conversations where Id in (select ConversationId from UserConversations where userId = @id) and Name is null) as Cs
                          join Messages as Ms on Ms.Id = Cs.LastMessageId
                          join UserConversations as UCs on UCs.ConversationId = Cs.Id and UCs.UserId != @id
                          join Users as Us on Us.Id = UCs.UserId
                          where Us.firstName + ' ' + Us.lastName like @filter + '%' ");

            if (skip != null)
            {
                sql.Append(" ")
                .Append($"order by Us.Id")
                .Append(" ")
                .Append("offset @skip rows");

                if (take != null)
                    sql.Append(" ")
                        .Append("fetch next @take rows only");
            }

            return await QueryAsync<User, Conversation, Message, User>(sql.ToString(), (user, conversation, message) =>
            {
                user.Conversation = conversation;
                conversation.LastMessage = message;
                return user;
            }, new { id = userId, filter = convName, skip = skip, take = take });
        }

        public async Task<IEnumerable<Conversation>> GetGroupsByNameAsync(int userId, string filter, int? skip = null, int? take = null)
        {
            var sql = new StringBuilder()
                .Append(@"select Cs.Id,CS.Name,(select count(*) from Messages where SenderId != @Id and IsRead = 0 and conversationId = Cs.Id and (leftDateTime is null or leftDateTime > Ms.TimeStamp)) as UnReadMessagesCount,Ms.Id,
                          case when UCs.LeftDateTime is null then Ms.TimeStamp else UCs.LeftDateTime end as TimeStamp,
                          case when UCs.LeftDateTime is null then Ms.Value else 'You have left this group' end as Value
                          from (select ConversationId,leftDateTime from UserConversations where UserId = @Id) as UCs
                          join Conversations as Cs on Cs.Id = UCs.ConversationId
                          join Messages as Ms on Ms.Id = Cs.LastMessageId 
                          where Cs.Name like @filter + '%'");

            if (skip != null)
            {
                sql.Append(" ")
                   .Append("order by Cs.Id")
                   .Append(" ")
                   .Append("offset @skip rows");

                if (take != null)
                    sql.Append(" ")
                        .Append("fetch next @take rows only");
            }

            return await QueryAsync<Conversation, Message, Conversation>(sql.ToString(), (conversation, message) =>
            {
                conversation.LastMessage = message;
                return conversation;
            }, new { filter = filter, Id = userId, skip = skip, take = take });
        }

        public async Task<int> GetConvsHasUnReadMsgsCountAsync(int userId)
        {
            var sql = @"select count(*) from UserConversations as UCs
                        join Conversations as Cs on Cs.Id = UCs.ConversationId
                        join Messages as Ms on Ms.Id = Cs.LastMessageId
                        where UCs.UserId = @userId and Ms.IsRead = 0 and Ms.SenderId != @userId and (UCs.LeftDateTime is null or UCs.LeftDateTime > Ms.TimeStamp) and joinDateTime < Ms.TimeStamp";

            return await QueryFirstOrDefaultAsync<int>(sql, new { userId = userId });
        }

        public async Task<int> GetAllConvMembersCount(int convId)
        {
            var sql = @$"select Count(*) from UserConversations as UCs
                        join Users as Us on Us.Id = UCs.UserId
                        where UCs.ConversationId = @convId and UCs.LeftDateTime is null";

            return await QueryFirstOrDefaultAsync<int>(sql, new { convId = convId });
        }
    }
}
