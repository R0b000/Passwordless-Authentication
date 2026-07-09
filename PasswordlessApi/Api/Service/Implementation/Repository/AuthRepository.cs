using PasswordlessApi.Api.Configuration;
using PasswordlessApi.Api.Models.Common;
using PasswordlessApi.Api.Service.Interface.Repository;
using System.Data;
using Dapper;

namespace PasswordlessApi.Api.Service.Implementation.Repository
{
    public class AuthRepository : IAuthRepository
    {
        private readonly DapperContext _context;

        public AuthRepository(DapperContext context)
        {
            _context = context;
        }

        public async Task<Response<int>> ExecuteAsync(string procedureName, object dataType)
        {
            using var connection = _context.CreateConnection();
            var result = await connection.ExecuteAsync(procedureName, dataType, commandType: CommandType.StoredProcedure);
            return Response<int>.Success(result);
        }

        public async Task<Response<T>> QuerySingleAsync<T>(string procedureName, object dataType)
        {
            using var connection = _context.CreateConnection();
            var result = await connection.QuerySingleOrDefaultAsync<T>(procedureName, dataType, commandType: CommandType.StoredProcedure);
            return Response<T>.Success(result!);
        }

        public async Task<IEnumerable<T>> QueryAsync<T>(string procedureName, object dataType)
        {
            using var connection = _context.CreateConnection();
            return await connection.QueryAsync<T>(procedureName, dataType, commandType: CommandType.StoredProcedure);
        }

        public async Task<T?> QueryFirstAsync<T>(string procedureName, object dataType)
        {
            using var connection = _context.CreateConnection();
            return await connection.QueryFirstOrDefaultAsync<T>(procedureName, dataType, commandType: CommandType.StoredProcedure);
        }

        public async Task<SqlMapper.GridReader> QueryMultipleAsync(string procedureName, object dataType)
        {
            using var connection = _context.CreateConnection();
            return await connection.QueryMultipleAsync(procedureName, dataType, commandType: CommandType.StoredProcedure);
        }

        public async Task<MessageResponse> UpdateAsync(string spName, object input, CommandType queryType = CommandType.StoredProcedure)
        {
            using var connection = _context.CreateConnection();
            var result = await connection.ExecuteAsync(spName, input, commandType: queryType);
            return result > 0 ? MessageResponse.Success("Update successful") : MessageResponse.Failure("Update failed");
        }

        public async Task<MessageResponse> InsertAsync(string spName, object input, CommandType queryType = CommandType.StoredProcedure)
        {
            using var connection = _context.CreateConnection();
            var result = await connection.ExecuteAsync(spName, input, commandType: queryType);
            return result > 0 ? MessageResponse.Success("Insert successful") : MessageResponse.Failure("Insert failed");
        }

        public async Task<MessageResponse> DeleteAsync(string spName, object input, CommandType queryType = CommandType.StoredProcedure)
        {
            using var connection = _context.CreateConnection();
            var result = await connection.ExecuteAsync(spName, input, commandType: queryType);
            return result > 0 ? MessageResponse.Success("Delete successful") : MessageResponse.Failure("Delete failed");
        }
    }
}
