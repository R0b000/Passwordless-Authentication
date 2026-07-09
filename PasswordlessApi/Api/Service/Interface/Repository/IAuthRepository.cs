using PasswordlessApi.Api.Models;
using PasswordlessApi.Api.Models.Common;
using System.Data;
using Dapper;

namespace PasswordlessApi.Api.Service.Interface.Repository
{
    public interface IAuthRepository
    {
        Task<Response<int>> ExecuteAsync(string procedureName, object dataType);
        Task<Response<T>> QuerySingleAsync<T>(string procedureName, object dataType);
        Task<IEnumerable<T>> QueryAsync<T>(string procedureName, object dataType);
        Task<T?> QueryFirstAsync<T>(string procedureName, object dataType);
        Task<SqlMapper.GridReader> QueryMultipleAsync(string procedureName, object dataType);
        Task<MessageResponse> UpdateAsync(string spName, object input, CommandType queryType = CommandType.StoredProcedure);
        Task<MessageResponse> InsertAsync(string spName, object input, CommandType queryType = CommandType.StoredProcedure);
        Task<MessageResponse> DeleteAsync(string spName, object input, CommandType queryType = CommandType.StoredProcedure);
    }
}
