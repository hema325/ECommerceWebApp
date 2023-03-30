using Dapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.DataAccessRepository.IRepository
{
    public interface ISqlRepository
    {
        Task<bool> ExecuteAsync(string sql, object param = null);
        Task<IEnumerable<T>> QueryAsync<T>(string sql, object param = null);
        Task<IEnumerable<TResult>> QueryAsync<TFirst, TSecond, TResult>(string sql, Func<TFirst, TSecond, TResult> map, object param = null, string splitOn = "Id");
        Task<IEnumerable<TResult>> QueryAsync<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh, TResult>(string sql, Func<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh, TResult> map, object param = null, string splitOn = "Id");
        Task<IEnumerable<TResult>> QueryAsync<TFirst, TSecond, TThird, TFourth, TResult>(string sql, Func<TFirst, TSecond, TThird, TFourth, TResult> map, object param = null, string splitOn = "Id");
        Task<IEnumerable<TResult>> QueryAsync<TFirst, TSecond, TThird, TFourth, TFifth, TResult>(string sql, Func<TFirst, TSecond, TThird, TFourth, TFifth, TResult> map, object param = null, string splitOn = "Id");
        Task<IEnumerable<TResult>> QueryAsync<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TResult>(string sql, Func<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TResult> map, object param = null, string splitOn = "Id");
        Task<T> QueryFirstOrDefaultAsync<T>(string sql, object param = null);
        Task<SqlMapper.GridReader> QueryMultipleAsync(string sql, object param = null);
    }
}
