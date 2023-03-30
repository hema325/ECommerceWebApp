using DataAccess.Data;
using DataAccess.DataAccessRepository.IRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.DataAccessRepository.Repository
{
    public class CountryRepository:EntityRepository<Country>,ICountryRepository
    {
        public CountryRepository(string connectionString) : base(connectionString) { }

        public async Task<IEnumerable<Country>> GetByNameAsync(string name, IEnumerable<string> attrs = null, IEnumerable<string> orderByAttrs = null, int? skip = null, int? take = null)
        {
            if (attrs == null)
                attrs = EntityProps;


            var sql = new StringBuilder()
                .Append($@"Select {string.Join(",", attrs)} From Countries
                           where Name like @name+'%'");


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

            return await QueryAsync<Country>(sql.ToString(), new { name = name, skip = skip, take = take });
        }
    }
}
