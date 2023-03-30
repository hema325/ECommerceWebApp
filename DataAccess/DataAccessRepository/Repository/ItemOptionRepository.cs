using DataAccess.Data;
using DataAccess.DataAccessRepository.IRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.DataAccessRepository.Repository
{
    public class ItemOptionRepository:EntityRepository<ItemOption>,IItemOptionRepository
    {
        public ItemOptionRepository(string connectionString) : base(connectionString) { }

        //not safe for strings 
        public async Task<bool> Add(IEnumerable<ItemOption> itemOptions)
        {
            var sql = new StringBuilder()
                .Append($"insert into ItemOptions ({string.Join(",", EntityProps)}) values");

            foreach (var itemOption in itemOptions)
                sql.Append(" ")
                    .Append($"({itemOption.ItemId},{itemOption.OptionId})")
                    .Append(",");

            //remove last ,
            --sql.Length;

            return await ExecuteAsync(sql.ToString());
        }

        public async Task<bool> DeleteAsync(int itemId,int optionId)
        {
            var sql = "Delete from ItemOptions where itemId = @itemId and optionId = @optionId";

            return await ExecuteAsync(sql, new { itemId = itemId, optionId = optionId });
        }

    }
}
