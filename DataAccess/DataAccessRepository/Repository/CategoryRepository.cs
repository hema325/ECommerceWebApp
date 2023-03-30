using DataAccess.Data;
using DataAccess.DataAccessRepository.IRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.DataAccessRepository.Repository
{
    public class CategoryRepository:EntityRepository<Category>,ICategoryRepository
    {
        public CategoryRepository(string connectionString) : base(connectionString) 
        {
            IgnoredProps.Add(nameof(Category.Parent));
            IgnoredProps.Add(nameof(Category.Children));
        }

        public async Task<Category> FindByIdIncludeParent(int id,IEnumerable<string> attrs = null,IEnumerable<string> parentAttrs = null,string splitOn="id")
        {
            if (attrs == null)
                attrs = EntityProps;

            if (parentAttrs == null)
                parentAttrs = EntityProps;

            var sql = @$"select {string.Join(",",attrs.Select(attr=>$"Cs.{attr}"))},{string.Join(",",parentAttrs.Select(attr=>$"Slf.{attr}"))} from Categories as Cs 
                        left join Categories as Slf on Slf.Id = Cs.ParentId
                        where Cs.Id = @childId";

            Category category = null;

            await QueryAsync<Category, Category, Category>(sql, (child, parent) =>
            {
                category = child;
                category.Parent = parent;

                return category;
            }, new { childId = id }, splitOn: splitOn);

            return category;
        }

        public async Task<IEnumerable<Category>> GetByNameAsync(string name, IEnumerable<string> attrs = null, IEnumerable<string> orderByAttrs = null, int? skip = null, int? take = null)
        {
            if (attrs == null)
                attrs = EntityProps;


            var sql = new StringBuilder()
                .Append($@"Select {string.Join(",", attrs)} From Categories
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

            return await QueryAsync<Category>(sql.ToString(), new { name = name, skip = skip, take = take });
        }

        public async Task<IEnumerable<Category>> GetCategoriesHierarchyAsync()
        {

            var sql = @"select l1.*,l2.*,l3.*,l4.* from Categories as l1
                        left join Categories as l2 on l2.ParentId = l1.Id
                        left join Categories as l3 on l3.ParentId = l2.Id
                        left join Categories as l4 on l4.ParentId = l3.Id
                        where l1.ParentId is null";

            var lookUp = new Dictionary<int, Category>();

             await QueryAsync<Category, Category, Category, Category, Category>(sql.ToString(), (l1, l2, l3, l4) =>
            {
                if (!lookUp.ContainsKey(l1.Id))
                    lookUp.Add(l1.Id, l1);

                if(l2 != null)
                {
                    if (!lookUp.ContainsKey(l2.Id))
                    {
                        lookUp.Add(l2.Id, l2);

                        if (lookUp[l1.Id].Children == null)
                            lookUp[l1.Id].Children = new List<Category>();

                        lookUp[l1.Id].Children.Add(l2);
                    }

                    if (l3 != null)
                    {
                        if (!lookUp.ContainsKey(l3.Id))
                        {
                            lookUp.Add(l3.Id, l3);

                            if (lookUp[l2.Id].Children == null)
                                lookUp[l2.Id].Children = new List<Category>();

                            lookUp[l2.Id].Children.Add(l3);
                        }

                        if (l4 != null)
                        {
                            if (lookUp[l3.Id].Children == null)
                                lookUp[l3.Id].Children = new List<Category>();

                            lookUp[l3.Id].Children.Add(l4);
                        }
                    }
                }
                return lookUp[l1.Id];
            });

            return lookUp.Values.Where(category => category.ParentId == null).ToList();
        }

    }
}
