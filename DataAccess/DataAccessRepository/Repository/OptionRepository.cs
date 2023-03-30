using DataAccess.Data;
using DataAccess.DataAccessRepository.IRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.DataAccessRepository.Repository
{
    public class OptionRepository:EntityRepository<Option>,IOptionRepository
    {
        public OptionRepository(string connectionString) : base(connectionString)
        {
            IgnoredProps.Add(nameof(Option.Variation));
        }

        public async Task<IEnumerable<Option>> GetByCategoryId(int categoryId,IEnumerable<string> attrs = null)
        {
            if (attrs == null)
                attrs = EntityProps;

            var sql = $@"select {string.Join(",",attrs.Select(attr=>$"Os.{attr}"))} from Options as Os
                         join Variations as Vs on Vs.Id = Os.VariationId
                         where Vs.CategoryId = @categoryId";

            return await QueryAsync<Option>(sql, new {categoryId = categoryId});
        }
    }
}
