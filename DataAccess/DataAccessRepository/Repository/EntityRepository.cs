using DataAccess.DataAccessRepository.IRepository;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.DataAccessRepository.Repository
{
    public class EntityRepository<TEntity> : SqlRepository, IEntityRepository<TEntity>
    {
        protected readonly HashSet<string> IgnoredProps;

        protected IEnumerable<string> EntityProps =>
            typeof(TEntity).GetProperties().Select(prop => prop.Name).Where(prop => !IgnoredProps.Contains(prop));

        protected IEnumerable<string> GetProps<TType>() =>
            typeof(TType).GetProperties().Select(prop => prop.Name).Where(prop => !IgnoredProps.Contains(prop));

        private string TableName 
        {
            get
            {
                var tableName = new StringBuilder(typeof(TEntity).Name);

                if (char.ToLower(tableName[tableName.Length - 1]) == 'y')
                    tableName.Remove(tableName.Length - 1, 1).Append("ies");
                else if(char.ToLower(tableName[tableName.Length - 1]) == 's')
                    tableName.Append("es");
                else
                    tableName.Append("s");

                return tableName.ToString();
            }
        }

        public EntityRepository(string connectionString) : base(connectionString)
        {
            IgnoredProps = new HashSet<string>();
        }

        public async Task<TOutput> AddAsync<TOutput>(TEntity entity, string output = "id")
        {
            var attrs = EntityProps.Where(prop => prop.ToLower() != "id");

            var sql = @$"Insert into {TableName} ({string.Join(",", attrs.Select(attr => $"[{attr}]"))})
                         output inserted.{output}
                         values ({string.Join(",", attrs.Select(name => $"@{name}"))})";

            return await QueryFirstOrDefaultAsync<TOutput>(sql, entity);
        }

        public async Task<bool> AddAsync(TEntity entity)
        {
            var attrs = EntityProps.Where(prop => prop.ToLower() != "id");

            var sql = @$"Insert into {TableName} ({string.Join(",", attrs.Select(attr=>$"[{attr}]"))})
                        values ({string.Join(",", attrs.Select(name => $"@{name}"))})";

            return await ExecuteAsync(sql, entity);
        }

        public async Task<bool> UpdateAsync(TEntity entity, IEnumerable<string> attrs = null)
        {
            if (attrs == null)
                attrs = EntityProps.Where(prop => prop.ToLower() != "id"); 

            var sql = $"Update {TableName} Set {string.Join(",", attrs.Select(attr => $"[{attr}]=@{attr}"))} Where id=@id";
            return await ExecuteAsync(sql, entity);
        }

        public async Task<bool> DeleteByIdAsync(int id)
        {
            var sql = $"Delete From {TableName} Where id = @id";

            return await ExecuteAsync(sql, new { Id = id });
        }

        public async Task<TEntity> FindByIdAsync(int id, IEnumerable<string> attrs = null)
        {
            if (attrs == null)
                attrs = EntityProps;

            var sql = $"Select {string.Join(",", attrs.Select(attr => $"[{attr}]"))} From {TableName} Where id=@id";

            return await QueryFirstOrDefaultAsync<TEntity>(sql.ToString(), new { id = id });
        }

        public async Task<IEnumerable<TEntity>> GetAllAsync(IEnumerable<string> attrs = null, IEnumerable<string> orderByAttrs = null, int? skip = null, int? take = null)
        {
            if (attrs == null)
                attrs = EntityProps;

            var sql = new StringBuilder()
                .Append($"Select {string.Join(",", attrs.Select(attr => $"[{attr}]"))} From {TableName}");

            if (orderByAttrs != null)
            {
                sql.Append(" ")
                    .Append($"order by {string.Join(",", orderByAttrs)}");

                if (skip != null)
                {
                    sql.Append(" ")
                        .Append($"Offset @skip Rows");

                    if (take != null)
                    {
                        sql.Append(" ")
                            .Append($"Fetch next @take Rows Only");
                    }
                }
            }

            return await QueryAsync<TEntity>(sql.ToString(), new { skip = skip, take = take });
        }

        public async Task<int> CountAsync()
        {
            var sql = $"Select Count(*) From {TableName}";

            return await QueryFirstOrDefaultAsync<int>(sql);
        }


    }
}
