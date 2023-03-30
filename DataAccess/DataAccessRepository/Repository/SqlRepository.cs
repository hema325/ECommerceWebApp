using Dapper;
using DataAccess.DataAccessRepository.IRepository;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.DataAccessRepository.Repository
{
    public abstract class SqlRepository :ISqlRepository
    {
        private readonly string ConnectionString;
        public SqlRepository(string connectionString)
        {
            ConnectionString = connectionString;
        }

        private SqlConnection Connection =>
            new SqlConnection(ConnectionString);

        public async Task<bool> ExecuteAsync(string sql, object param = null) =>
            await Connection.ExecuteAsync(sql, param) > 0;

        public async Task<IEnumerable<T>> QueryAsync<T>(string sql,object param = null) =>
            await Connection.QueryAsync<T>(sql,param);

        public async Task<IEnumerable<TResult>> QueryAsync<TFirst, TSecond, TResult>(string sql, Func<TFirst, TSecond, TResult> map, object param = null,string splitOn = "Id") =>
            await Connection.QueryAsync<TFirst, TSecond, TResult>(sql, map, param,splitOn:splitOn);

        public async Task<IEnumerable<TResult>> QueryAsync<TFirst, TSecond,TThird, TResult>(string sql, Func<TFirst, TSecond,TThird, TResult> map, object param = null, string splitOn = "Id") =>
            await Connection.QueryAsync<TFirst, TSecond,TThird, TResult>(sql, map, param, splitOn: splitOn);

        public async Task<IEnumerable<TResult>> QueryAsync<TFirst, TSecond, TThird,TFourth, TFifth, TResult>(string sql, Func<TFirst, TSecond, TThird, TFourth, TFifth, TResult> map, object param = null, string splitOn = "Id") =>
            await Connection.QueryAsync<TFirst, TSecond, TThird, TFourth,TFifth, TResult>(sql, map, param, splitOn: splitOn);

        public async Task<IEnumerable<TResult>> QueryAsync<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TResult>(string sql, Func<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TResult> map, object param = null, string splitOn = "Id") =>
            await Connection.QueryAsync<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TResult>(sql, map, param, splitOn: splitOn);


        public async Task<IEnumerable<TResult>> QueryAsync<TFirst, TSecond, TThird,TFourth,TFifth, TSixth, TSeventh, TResult>(string sql, Func<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh, TResult> map, object param = null, string splitOn = "Id") =>
            await Connection.QueryAsync<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh, TResult>(sql, map, param, splitOn: splitOn);

        public async Task<IEnumerable<TResult>> QueryAsync<TFirst, TSecond, TThird, TFourth, TResult>(string sql, Func<TFirst, TSecond, TThird, TFourth, TResult> map, object param = null, string splitOn = "Id") =>
           await Connection.QueryAsync<TFirst, TSecond, TThird, TFourth, TResult>(sql, map, param, splitOn: splitOn);


        public async Task<T> QueryFirstOrDefaultAsync<T>(string sql,object param = null) =>
            await Connection.QueryFirstOrDefaultAsync<T>(sql, param);

        public async Task<SqlMapper.GridReader> QueryMultipleAsync(string sql,object param = null) =>
            await Connection.QueryMultipleAsync(sql,param);

    }
}
