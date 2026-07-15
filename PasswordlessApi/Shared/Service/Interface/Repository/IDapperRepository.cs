using System.Data;
using Dapper;
using API.Shared.Models.Common;

namespace API.Shared.Service.Interface.Repository
{
    public interface IDapperRepository
    {
        Task<Response<int>> ExecuteAsync(string procedureName, object dataType, CommandType queryType = CommandType.StoredProcedure);
        Task<Response<IEnumerable<T>>> QueryAsync<T>(string procedureName, object dataType, CommandType queryType = CommandType.StoredProcedure);
        Task<Response<T?>> QuerySingleAsync<T>(string procedureName, object dataType, CommandType queryType = CommandType.StoredProcedure);
        Task<Response<T?>> QueryFirstAsync<T>(string procedureName, object dataType, CommandType queryType = CommandType.StoredProcedure);
        Task<Response<SqlMapper.GridReader>> QueryMultipleAsync(string procedureName, object dataType, CommandType queryType = CommandType.StoredProcedure);
        Task<Response<List<TResult>>> GetAllAsync<TResult>(string spName, object obj, CommandType queryType = CommandType.StoredProcedure);
        Task<Response<TResult>> GetAsync<TResult>(string spName, object obj, CommandType queryType = CommandType.StoredProcedure);
        Task<Response<SqlMapper.GridReader>> GetMultipleListAsync(string spName, object obj, CommandType queryType = CommandType.StoredProcedure);
        Task<Response<MessageResponse>> UpdateAsync(string spName, object input, CommandType queryType = CommandType.StoredProcedure);
        Task<Response<MessageResponse>> InsertAsync(string spName, object input, CommandType queryType = CommandType.StoredProcedure);
        Task<Response<MessageResponse>> DeleteAsync(string spName, object input, CommandType queryType = CommandType.StoredProcedure);
        Task<Response<List<object>>> GetFromMultipleQuery<T0, T1>(string sqlQuery, object sqlParam, CommandType queryType = CommandType.StoredProcedure);
        Task<Response<List<object>>> GetFromMultipleQuery<T0, T1, T2>(string sqlQuery, object sqlParam, CommandType queryType = CommandType.StoredProcedure);
        Task<Response<List<object>>> GetFromMultipleQuery<T0, T1, T2, T3>(string sqlQuery, object sqlParam, CommandType queryType = CommandType.StoredProcedure);
    }
}