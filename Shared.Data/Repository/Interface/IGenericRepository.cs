using System.Data;
using Dapper;
using Auth.Model.Models.Common;
using Auth.Model.Wrapper;

namespace Shared.Data.Repository.Interface
{
    public interface IGenericRepository<T> where T : class
    {
        Task<Response<int>> ExecuteAsync(string procedureName, object dataType);
        Task<Response<T>> QuerySingleAsync(string procedureName, object dataType);
        Task<Response<TResult>> QuerySingleAsync<TResult>(string procedureName, object dataType);
        Task<Response<List<T>>> QueryAsync(string procedureName, object dataType);
        Task<Response<List<TResult>>> QueryAsync<TResult>(string procedureName, object dataType);
        Task<T?> QueryFirstAsync(string procedureName, object dataType);
        Task<TResult?> QueryFirstAsync<TResult>(string procedureName, object dataType);
        Task<SqlMapper.GridReader> QueryMultipleAsync(string procedureName, object dataType);
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

