using Dapper;
using PasswordlessApi.Api.Configuration;
using PasswordlessApi.Api.Models.Common;
using PasswordlessApi.Api.Models.Entities;
using PasswordlessApi.Api.Models.ResponseModel.Auth;
using PasswordlessApi.Api.Service.Interface.Auth;
using PasswordlessApi.Api.Service.Interface.Repository;
using PasswordlessApi.Api.Utility.Jwt;
using PasswordlessApi.Api.Utility.PasswordHash;
using System.Data;

namespace PasswordlessApi.Api.Service.Implementation.Repository
{
    public class DapperRepository : IDapperRepository
    {
        private readonly DapperContext _context;

        public DapperRepository(DapperContext context)
        {
            _context = context;
        }

        public async Task<int> ExecuteAsync(string procedureName, object dataType, CommandType queryType = CommandType.StoredProcedure)
        {
            using var connection = _context.CreateConnection();
            return await connection.ExecuteAsync(procedureName, dataType, commandType: queryType);
        }

        public async Task<IEnumerable<T>> QueryAsync<T>(string procedureName, object dataType, CommandType queryType = CommandType.StoredProcedure)
        {
            using var connection = _context.CreateConnection();
            return await connection.QueryAsync<T>(procedureName, dataType, commandType: queryType);
        }

        public async Task<T?> QuerySingleAsync<T>(string procedureName, object dataType, CommandType queryType = CommandType.StoredProcedure)
        {
            using var connection = _context.CreateConnection();
            return await connection.QuerySingleOrDefaultAsync<T>(procedureName, dataType, commandType: queryType);
        }

        public async Task<T?> QueryFirstAsync<T>(string procedureName, object dataType, CommandType queryType = CommandType.StoredProcedure)
        {
            using var connection = _context.CreateConnection();
            return await connection.QueryFirstOrDefaultAsync<T>(procedureName, dataType, commandType: queryType);
        }

        public async Task<SqlMapper.GridReader> QueryMultipleAsync(string procedureName, object dataType, CommandType queryType = CommandType.StoredProcedure)
        {
            var connection = _context.CreateConnection();
            try
            {
                return await connection.QueryMultipleAsync(procedureName, dataType, commandType: queryType);
            }
            catch
            {
                connection.Dispose();
                throw;
            }
        }

        public async Task<List<TResult>> GetAllAsync<TResult>(string spName, object obj, CommandType queryType = CommandType.StoredProcedure)
        {
            using var connection = _context.CreateConnection();
            return (await connection.QueryAsync<TResult>(spName, obj, commandType: queryType)).ToList();
        }

        public async Task<TResult> GetAsync<TResult>(string spName, object obj, CommandType queryType = CommandType.StoredProcedure)
        {
            using var connection = _context.CreateConnection();
            return (await connection.QuerySingleOrDefaultAsync<TResult>(spName, obj, commandType: queryType))!;
        }

        public async Task<SqlMapper.GridReader> GetMultipleListAsync(string spName, object obj, CommandType queryType = CommandType.StoredProcedure)
        {
            var connection = _context.CreateConnection();
            try
            {
                return await connection.QueryMultipleAsync(spName, obj, commandType: queryType);
            }
            catch
            {
                connection.Dispose();
                throw;
            }
        }

        public async Task<MessageResponse> UpdateAsync(string spName, object input, CommandType queryType = CommandType.StoredProcedure)
        {
            using var connection = _context.CreateConnection();
            var result = await connection.ExecuteAsync(spName, input, commandType: queryType);
            if (result > 0)
                return MessageResponse.Success("Update successful");
            return MessageResponse.Failure("Update failed");
        }

        public async Task<MessageResponse> InsertAsync(string spName, object input, CommandType queryType = CommandType.StoredProcedure)
        {
            using var connection = _context.CreateConnection();
            var result = await connection.ExecuteAsync(spName, input, commandType: queryType);
            if (result > 0)
                return MessageResponse.Success("Insert successful");
            return MessageResponse.Failure("Insert failed");
        }

        public async Task<MessageResponse> DeleteAsync(string spName, object input, CommandType queryType = CommandType.StoredProcedure)
        {
            using var connection = _context.CreateConnection();
            var result = await connection.ExecuteAsync(spName, input, commandType: queryType);
            if (result > 0)
                return MessageResponse.Success("Delete successful");
            return MessageResponse.Failure("Delete failed");
        }

        public async Task<List<object>> GetFromMultipleQuery<T0, T1>(string sqlQuery, object sqlParam, CommandType queryType = CommandType.StoredProcedure)
        {
            using var connection = _context.CreateConnection();
            using var gridReader = await connection.QueryMultipleAsync(sqlQuery, sqlParam, commandType: queryType);
            var result = new List<object>();
            var set0 = await gridReader.ReadAsync<T0>();
            if (set0 != null) result.AddRange(set0.Cast<object>());
            var set1 = await gridReader.ReadAsync<T1>();
            if (set1 != null) result.AddRange(set1.Cast<object>());
            return result;
        }

        public async Task<List<object>> GetFromMultipleQuery<T0, T1, T2>(string sqlQuery, object sqlParam, CommandType queryType = CommandType.StoredProcedure)
        {
            using var connection = _context.CreateConnection();
            using var gridReader = await connection.QueryMultipleAsync(sqlQuery, sqlParam, commandType: queryType);
            var result = new List<object>();
            var set0 = await gridReader.ReadAsync<T0>();
            if (set0 != null) result.AddRange(set0.Cast<object>());
            var set1 = await gridReader.ReadAsync<T1>();
            if (set1 != null) result.AddRange(set1.Cast<object>());
            var set2 = await gridReader.ReadAsync<T2>();
            if (set2 != null) result.AddRange(set2.Cast<object>());
            return result;
        }

        public async Task<List<object>> GetFromMultipleQuery<T0, T1, T2, T3>(string sqlQuery, object sqlParam, CommandType queryType = CommandType.StoredProcedure)
        {
            using var connection = _context.CreateConnection();
            using var gridReader = await connection.QueryMultipleAsync(sqlQuery, sqlParam, commandType: queryType);
            var result = new List<object>();
            var set0 = await gridReader.ReadAsync<T0>();
            if (set0 != null) result.AddRange(set0.Cast<object>());
            var set1 = await gridReader.ReadAsync<T1>();
            if (set1 != null) result.AddRange(set1.Cast<object>());
            var set2 = await gridReader.ReadAsync<T2>();
            if (set2 != null) result.AddRange(set2.Cast<object>());
            var set3 = await gridReader.ReadAsync<T3>();
            if (set3 != null) result.AddRange(set3.Cast<object>());
            return result;
        }


    }
}
