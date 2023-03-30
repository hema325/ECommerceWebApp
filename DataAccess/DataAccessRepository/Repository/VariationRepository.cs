using DataAccess.Data;
using DataAccess.DataAccessRepository.IRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.DataAccessRepository.Repository
{
    public class VariationRepository:EntityRepository<Variation>,IVariationRepository
    {
        public VariationRepository(string connectionString) : base(connectionString) 
        {
            IgnoredProps.Add(nameof(Variation.Category));
            IgnoredProps.Add(nameof(Variation.Options));
            IgnoredProps.Add(nameof(Category.Parent));
        }

        public async Task<Variation> FindByIdIncludeCategoryAsync(int id, IEnumerable<string> attrs = null, IEnumerable<string> categoryAttrs = null, string splitOn = "id")
        {
            if (attrs == null)
                attrs = EntityProps;

            if (categoryAttrs == null)
                categoryAttrs = GetProps<Category>();

            var sql = $@"select {string.Join(",", attrs.Select(attr => $"Vs.{attr}"))},{string.Join(",", categoryAttrs.Select(attr => $"Cs.{attr}"))} from Variations as Vs
                           join Categories as Cs on Cs.Id = Vs.CategoryId
                           where Vs.Id = @Id ";

            Variation output = null;

            await QueryAsync<Variation, Category, Variation>(sql, (variation, category) =>
            {
                output = variation;
                output.Category = category;
                return output;
            }, new { id = id }, splitOn: splitOn);

            return output;
        }

        public async Task<IEnumerable<Variation>> GetAllIncludeCategoryAsync(IEnumerable<string> attrs = null, IEnumerable<string> categoryAttrs = null, int? skip = null, int? take = null, string splitOn = "id")
        {
            if (attrs == null)
                attrs = EntityProps;

            if (categoryAttrs == null)
                categoryAttrs = GetProps<Category>();

            var sql = new StringBuilder()
                .Append($@"select {string.Join(",", attrs.Select(attr => $"Vs.{attr}"))},{string.Join(",", categoryAttrs.Select(attr => $"Cs.{attr}"))} from Variations as Vs
                           join Categories as Cs on Cs.Id = Vs.CategoryId");

            if (skip != null)
            {
                sql.Append(" ")
                    .Append("order by Vs.Id offset @skip rows ");

                if (take != null)
                    sql.Append(" ")
                        .Append("fetch next @take rows only");
            }


            return await QueryAsync<Variation, Category, Variation>(sql.ToString(), (variation, category) =>
            {
                variation.Category = category;
                return variation;
            }, new { skip = skip, take = take }, splitOn: splitOn);
        }

        public async Task<IEnumerable<Variation>> GetByNameIncludeCategoryAsync(string name, IEnumerable<string> attrs = null, IEnumerable<string> categoryAttrs = null, int? skip = null, int? take = null, string splitOn = "id")
        {
            if (attrs == null)
                attrs = EntityProps;

            if (categoryAttrs == null)
                categoryAttrs = GetProps<Category>();

            var sql = new StringBuilder()
                .Append($@"select {string.Join(",", attrs.Select(attr => $"Vs.{attr}"))},{string.Join(",", categoryAttrs.Select(attr => $"Cs.{attr}"))} from Variations as Vs
                           join Categories as Cs on Cs.Id = Vs.CategoryId
                           where Vs.Name like @name+'%' ");

            if (skip != null)
            {
                sql.Append(" ")
                    .Append("order by Vs.Id offset @skip rows ");

                if (take != null)
                    sql.Append(" ")
                        .Append("fetch next @take rows only");
            }


            return await QueryAsync<Variation, Category, Variation>(sql.ToString(), (variation, category) =>
            {
                variation.Category = category;
                return variation;
            }, new { name=name,skip = skip, take = take }, splitOn: splitOn);
        }

        public async Task<Variation> GetByIdIncludeCategoryThenOptionAsync(int id,IEnumerable<string> attrs = null, IEnumerable<string> categoryAttrs = null, IEnumerable<string> optionAttrs = null, string splitOn = "id")
        {
            if (attrs == null)
                attrs = EntityProps;

            if (categoryAttrs == null)
                categoryAttrs = GetProps<Category>();

            if (optionAttrs == null)
                optionAttrs = GetProps<Option>();

            var sql = $@"select {string.Join(",", attrs.Select(attr => $"Vs.{attr}"))},{string.Join(",", categoryAttrs.Select(attr => $"Cs.{attr}"))},{string.Join(",", optionAttrs.Select(attr => $"Os.{attr}"))} 
                         from Variations as Vs
                         left join Categories as Cs on Cs.Id = Vs.CategoryId
                         left Join Options as Os on Os.VariationId = Vs.Id
                         where Vs.Id = @id";

            Variation output = null;

            await QueryAsync<Variation, Category, Option, Variation>(sql.ToString(), (variation, category, Option) =>
            {
                if (output == null)
                {
                    output = variation;
                    output.Category = category;
                    output.Options = new List<Option>();
                }

                output.Options.Add(Option);

                return variation;
            }, new { id = id }, splitOn: splitOn);

            return output;
        }

        public async Task<IEnumerable<Variation>> GetVariationsIncludeOptionsByCategoriesIds(IEnumerable<int> ids, IEnumerable<string> attrs = null, IEnumerable<string> optionAttrs = null,string splitOn="id")
        {
            if (attrs == null)
                attrs = EntityProps;

            if (optionAttrs == null)
                optionAttrs = GetProps<Option>();

            var sql = $@"select {string.Join(",", attrs.Select(attr => $"Vs.{attr}"))},{string.Join(",", optionAttrs.Select(attr => $"Os.{attr}"))} from Variations as Vs
                        Join Options as Os on Os.VariationId = Vs.Id
                        where CategoryId in({string.Join(",", ids.Select(id => id.ToString()))})";

            var lookUp = new Dictionary<int, Variation>();

            await QueryAsync<Variation, Option, Variation>(sql, (variation, option) =>
            {
                if (!lookUp.ContainsKey(variation.Id))
                    lookUp.Add(variation.Id, variation);

                if (lookUp[variation.Id].Options == null)
                    variation.Options = new List<Option>();

                lookUp[variation.Id].Options.Add(option);

                return variation;
            }, splitOn: splitOn);

            return lookUp.Values.ToList();
        }

    }
}
