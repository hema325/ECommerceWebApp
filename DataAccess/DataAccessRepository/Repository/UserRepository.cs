using DataAccess.Data;
using DataAccess.DataAccessRepository.IRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.DataAccessRepository.Repository
{
    public class UserRepository :EntityRepository<User>, IUserRepository
    {
        public UserRepository(string connectionString) : base(connectionString) 
        {
            IgnoredProps.Add(nameof(User.ConvsHasUnReadMsgsCount));
            IgnoredProps.Add(nameof(User.Conversation));
            IgnoredProps.Add(nameof(User.Roles));
            IgnoredProps.Add(nameof(User.Addresses));
            IgnoredProps.Add(nameof(Address.Country));
        }


        public async Task<User> FindByEmailAsync(string email,IEnumerable<string> attributes = null)
        {
            if (attributes == null)
                attributes = EntityProps;

            var sql = $"select {string.Join(",", attributes)} from Users where Email = @email";

            return await QueryFirstOrDefaultAsync<User>(sql, new { email = email });
        }

        public async Task<User> FindByEmailIncludeRolesAsync(string email, IEnumerable<string> userAttrs = null, IEnumerable<string> roleAttrs = null)
        {
            if (userAttrs == null)
                userAttrs = EntityProps;

            if (roleAttrs == null)
                roleAttrs = GetProps<Role>();

            var sql = @$"select {string.Join(",", userAttrs.Select(attr => $"Us.{attr}"))},{string.Join(",", roleAttrs.Select(attr => $"Rs.{attr}"))} from users as Us
                        join UserRoles as URs on Us.Id = URs.UserId
                        join Roles as Rs on Rs.Id = URs.RoleId
                        where Us.Email = @email";

            User output = null;

            await QueryAsync<User, Role, User>(sql, (user, role) =>
            {
                if (output == null)
                    output = user;

                if (role != null)
                {
                    if (output.Roles == null)
                        output.Roles = new List<Role>();

                    output.Roles.Add(role);
                }
                return user;

            }, new { email = email });

            return output;
        }


        public async Task<User> FindByIdIncludeRolesAsync(int id,IEnumerable<string> userAttrs = null,IEnumerable<string> roleAttrs = null)
        {
            if (userAttrs == null)
                userAttrs = EntityProps;

            if (roleAttrs == null)
                roleAttrs = GetProps<Role>();

            var sql = @$"select {string.Join(",", userAttrs.Select(attr => $"Us.{attr}"))},{string.Join(",", roleAttrs.Select(attr => $"Rs.{attr}"))} from users as Us
                        join UserRoles as URs on Us.Id = URs.UserId
                        join Roles as Rs on Rs.Id = URs.RoleId
                        where Us.Id = @Id";

            User output = null;

            await QueryAsync<User, Role, User>(sql, (user, role) =>
            {
                if (output == null)
                    output = user;

                if (role != null)
                {
                    if (output.Roles == null)
                        output.Roles = new List<Role>();

                    output.Roles.Add(role);
                }
                return user;

            }, new { id = id });

            return output;
        }

        public async Task<User> FindByIdIncludeAddreesesAsync(int id, IEnumerable<string> userAttrs = null, IEnumerable<string> addressAttrs = null)
        {
            if (userAttrs == null)
                userAttrs = EntityProps;

            if (addressAttrs == null)
                addressAttrs = GetProps<Address>();

            var sql = @$"select {string.Join(",", userAttrs.Select(attr => $"Us.{attr}"))},{string.Join(",", addressAttrs.Select(attr => $"ADs.{attr}"))},Cs.* from Users as Us
                         left join Addresses as ADs on ADs.UserId = Us.Id
                         left join Countries as Cs on Cs.Id = ADs.CountryId
                         where Us.Id = @Id";

            User output = null;

            await QueryAsync<User, Address,Country, User>(sql, (user, address,country) =>
            {
                if (output == null)
                    output = user;

                if (address != null)
                {
                    if (output.Addresses == null)
                        output.Addresses = new List<Address>();

                    address.Country = country;

                    output.Addresses.Add(address);
                }
                
                return user;

            }, new { id = id });

            return output;
        }

        public async Task<IEnumerable<User>> GetByNameAsync(string filter, IEnumerable<string> attrs = null,IEnumerable<string> orderByAttrs = null, int? skip = null, int? take = null)
        {
            if (attrs == null)
                attrs = EntityProps;

            var sql = new StringBuilder()
                .Append($"select {string.Join(",", attrs)} from Users where firstName + ' ' + lastName like @filter + '%'");
            
            if(orderByAttrs != null)
            {
                sql.Append($" order by {string.Join(",", orderByAttrs)}");
                if (skip != null)
                {
                    sql.Append(" offset @skip rows");
                    if (take != null)
                        sql.Append(" fetch next @take rows only");
                }
            }

            return await QueryAsync<User>(sql.ToString(), new { filter = filter, skip = skip, take = take });
        }

        
        public async Task<IEnumerable<User>> GetRelatedUsersAsync(int userId,IEnumerable<string> attrs = null)
        {
            if (attrs == null)
                attrs = EntityProps;

            var sql = $@"select distinct {string.Join(",",attrs)} from (select ConversationId from UserConversations where userId = userId) as Cs
                        join UserConversations as UCs on UCs.ConversationId = Cs.ConversationId
                        join Users as Us on Us.Id = UCs.UserId
                        where UCs.UserId != @userId";

            return await QueryAsync<User>(sql, new { userId = userId });
        }

        public async Task<IEnumerable<User>> GetRelatedUsersNotInConvIdAsync(int userId, int convId, IEnumerable<string> attrs = null)
        {
            if (attrs == null)
                attrs = EntityProps;

            var sql = $@"select distinct {string.Join(",",attrs)} from (select ConversationId from UserConversations where userId = @userId) as Cs
                         join UserConversations as UCs on UCs.ConversationId = Cs.ConversationId
                         join Users as Us on Us.Id = UCs.UserId
                         where UCs.UserId not in (select UserId from UserConversations where ConversationId = @convId and leftDateTime is null)";

            return await QueryAsync<User>(sql, new { userId = userId, convId = convId});
        }

        //not safe for strings but its not possible to use Strings here
        public async Task<IEnumerable<User>> GetById(IEnumerable<int> usersIDs, IEnumerable<string> attrs = null)
        {
            if (attrs == null)
                attrs = EntityProps;

            var sql = $"select {string.Join(",", attrs)} from Users where Id in({string.Join(",", usersIDs)})";

            return await QueryAsync<User>(sql);
        }

    }
}
