using PasswordlessApi.Api.Models;
using PasswordlessApi.Api.Models.Common;
using System.Data;
using Dapper;

namespace PasswordlessApi.Api.Service.Interface.Repository
{
    public interface IDapperRepository
    {
        Task<int> ExecuteAsync(string procedureName, object dataType, CommandType queryType = CommandType.StoredProcedure);
        Task<IEnumerable<T>> QueryAsync<T>(string procedureName, object dataType, CommandType queryType = CommandType.StoredProcedure);
        Task<T?> QuerySingleAsync<T>(string procedureName, object dataType, CommandType queryType = CommandType.StoredProcedure);
        Task<T?> QueryFirstAsync<T>(string procedureName, object dataType, CommandType queryType = CommandType.StoredProcedure);
        Task<SqlMapper.GridReader> QueryMultipleAsync(string procedureName, object dataType, CommandType queryType = CommandType.StoredProcedure);
        Task<List<TResult>> GetAllAsync<TResult>(string spName, object obj, CommandType queryType = CommandType.StoredProcedure);
        Task<TResult> GetAsync<TResult>(string spName, object obj, CommandType queryType = CommandType.StoredProcedure);
        Task<SqlMapper.GridReader> GetMultipleListAsync(string spName, object obj, CommandType queryType = CommandType.StoredProcedure);
        Task<MessageResponse> UpdateAsync(string spName, object input, CommandType queryType = CommandType.StoredProcedure);
        Task<MessageResponse> InsertAsync(string spName, object input, CommandType queryType = CommandType.StoredProcedure);
        Task<MessageResponse> DeleteAsync(string spName, object input, CommandType queryType = CommandType.StoredProcedure);
        Task<List<object>> GetFromMultipleQuery<T0, T1>(string sqlQuery, object sqlParam, CommandType queryType = CommandType.StoredProcedure);
        Task<List<object>> GetFromMultipleQuery<T0, T1, T2>(string sqlQuery, object sqlParam, CommandType queryType = CommandType.StoredProcedure);
        Task<List<object>> GetFromMultipleQuery<T0, T1, T2, T3>(string sqlQuery, object sqlParam, CommandType queryType = CommandType.StoredProcedure);
    }
}
