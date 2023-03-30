using DataAccess.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.DataAccessRepository.IRepository
{
    public interface IVariationRepository:IEntityRepository<Variation>
    {
        Task<IEnumerable<Variation>> GetAllIncludeCategoryAsync(IEnumerable<string> attrs = null, IEnumerable<string> categoryAttrs = null, int? skip = null, int? take = null, string splitOn = "id");
        Task<IEnumerable<Variation>> GetByNameIncludeCategoryAsync(string name, IEnumerable<string> attrs = null, IEnumerable<string> categoryAttrs = null, int? skip = null, int? take = null, string splitOn = "id");
        Task<Variation> FindByIdIncludeCategoryAsync(int id, IEnumerable<string> attrs = null, IEnumerable<string> categoryAttrs = null, string splitOn = "id");
        Task<Variation> GetByIdIncludeCategoryThenOptionAsync(int id, IEnumerable<string> attrs = null, IEnumerable<string> categoryAttrs = null, IEnumerable<string> optionAttrs = null, string splitOn = "id");
        Task<IEnumerable<Variation>> GetVariationsIncludeOptionsByCategoriesIds(IEnumerable<int> ids, IEnumerable<string> attrs = null, IEnumerable<string> optionAttrs = null, string splitOn = "id");
    }
}
