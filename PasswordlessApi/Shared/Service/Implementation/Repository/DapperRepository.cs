using API.Shared.Configuration;
using API.Shared.Models.Common;
using API.Shared.Service.Interface.Repository;
using Dapper;
using System.Data;

namespace API.Shared.Service.Implementation.Repository
{
    public class DapperRepository : IDapperRepository
    {
        private readonly DapperContext _context;

        public DapperRepository(DapperContext context)
        {
            _context = context;
        }

        public async Task<Response<int>> ExecuteAsync(string procedureName, object dataType, CommandType queryType = CommandType.StoredProcedure)
        {
            using var connection = _context.CreateConnection();

            try
            {
                var rowsAffected = await connection.ExecuteAsync(procedureName, dataType, commandType: queryType);

                if (rowsAffected > 0)
                {
                    return Response<int>.Success(rowsAffected);
                }
                else
                {
                    return Response<int>.Failure("No rows affected");
                }
            }
            catch (Exception ex)
            {
                return Response<int>.Failure($"Execution failed: {ex.Message}");
            }
        }

        public async Task<Response<IEnumerable<T>>> QueryAsync<T>(string procedureName, object dataType, CommandType queryType = CommandType.StoredProcedure)
        {
            using var connection = _context.CreateConnection();

            try
            {
                var result = await connection.QueryAsync<T>(procedureName, dataType, commandType: queryType);
                return Response<IEnumerable<T>>.Success(result);
            }
            catch (Exception ex)
            {
                return Response<IEnumerable<T>>.Failure($"Query failed: {ex.Message}");
            }
        }

        public async Task<Response<T?>> QuerySingleAsync<T>(string procedureName, object dataType, CommandType queryType = CommandType.StoredProcedure)
        {
            using var connection = _context.CreateConnection();

            try
            {
                var result = await connection.QuerySingleOrDefaultAsync<T>(procedureName, dataType, commandType: queryType);

                if (result != null)
                {
                    return Response<T?>.Success(result);
                }
                else
                {
                    return Response<T?>.Failure("No data found");
                }
            }
            catch (Exception ex)
            {
                return Response<T?>.Failure($"Query failed: {ex.Message}");
            }
        }

        public async Task<Response<T?>> QueryFirstAsync<T>(string procedureName, object dataType, CommandType queryType = CommandType.StoredProcedure)
        {
            using var connection = _context.CreateConnection();

            try
            {
                var result = await connection.QueryFirstOrDefaultAsync<T>(procedureName, dataType, commandType: queryType);

                if (result != null)
                {
                    return Response<T?>.Success(result);
                }
                else
                {
                    return Response<T?>.Failure("No data found");
                }
            }
            catch (Exception ex)
            {
                return Response<T?>.Failure($"Query failed: {ex.Message}");
            }
        }

        public async Task<Response<SqlMapper.GridReader>> QueryMultipleAsync(string procedureName, object dataType, CommandType queryType = CommandType.StoredProcedure)
        {
            var connection = _context.CreateConnection();

            try
            {
                var result = await connection.QueryMultipleAsync(procedureName, dataType, commandType: queryType);
                return Response<SqlMapper.GridReader>.Success(result);
            }
            catch (Exception ex)
            {
                connection.Dispose();
                return Response<SqlMapper.GridReader>.Failure($"Query multiple failed: {ex.Message}");
            }
        }

        public async Task<Response<List<TResult>>> GetAllAsync<TResult>(string spName, object obj, CommandType queryType = CommandType.StoredProcedure)
        {
            using var connection = _context.CreateConnection();

            try
            {
                var result = (await connection.QueryAsync<TResult>(spName, obj, commandType: queryType)).ToList();
                return Response<List<TResult>>.Success(result);
            }
            catch (Exception ex)
            {
                return Response<List<TResult>>.Failure($"GetAll failed: {ex.Message}");
            }
        }

        public async Task<Response<TResult>> GetAsync<TResult>(string spName, object obj, CommandType queryType = CommandType.StoredProcedure)
        {
            using var connection = _context.CreateConnection();

            try
            {
                var result = await connection.QuerySingleOrDefaultAsync<TResult>(spName, obj, commandType: queryType);

                if (result != null)
                {
                    return Response<TResult>.Success(result);
                }
                else
                {
                    return Response<TResult>.Failure("No data found");
                }
            }
            catch (Exception ex)
            {
                return Response<TResult>.Failure($"Get failed: {ex.Message}");
            }
        }

        public async Task<Response<SqlMapper.GridReader>> GetMultipleListAsync(string spName, object obj, CommandType queryType = CommandType.StoredProcedure)
        {
            var connection = _context.CreateConnection();

            try
            {
                var result = await connection.QueryMultipleAsync(spName, obj, commandType: queryType);
                return Response<SqlMapper.GridReader>.Success(result);
            }
            catch (Exception ex)
            {
                connection.Dispose();
                return Response<SqlMapper.GridReader>.Failure($"Get multiple failed: {ex.Message}");
            }
        }

        public async Task<Response<MessageResponse>> UpdateAsync(string spName, object input, CommandType queryType = CommandType.StoredProcedure)
        {
            using var connection = _context.CreateConnection();

            try
            {
                var result = await connection.ExecuteAsync(spName, input, commandType: queryType);

                if (result > 0)
                {
                    return Response<MessageResponse>.Success(MessageResponse.Success("Update successful"));
                }
                else
                {
                    return Response<MessageResponse>.Failure("Update failed");
                }
            }
            catch (Exception ex)
            {
                return Response<MessageResponse>.Failure($"Update failed: {ex.Message}");
            }
        }

        public async Task<Response<MessageResponse>> InsertAsync(string spName, object input, CommandType queryType = CommandType.StoredProcedure)
        {
            using var connection = _context.CreateConnection();

            try
            {
                var result = await connection.ExecuteAsync(spName, input, commandType: queryType);

                if (result > 0)
                {
                    return Response<MessageResponse>.Success(MessageResponse.Success("Insert successful"));
                }
                else
                {
                    return Response<MessageResponse>.Failure("Insert failed");
                }
            }
            catch (Exception ex)
            {
                return Response<MessageResponse>.Failure($"Insert failed: {ex.Message}");
            }
        }

        public async Task<Response<MessageResponse>> DeleteAsync(string spName, object input, CommandType queryType = CommandType.StoredProcedure)
        {
            using var connection = _context.CreateConnection();

            try
            {
                var result = await connection.ExecuteAsync(spName, input, commandType: queryType);

                if (result > 0)
                {
                    return Response<MessageResponse>.Success(MessageResponse.Success("Delete successful"));
                }
                else
                {
                    return Response<MessageResponse>.Failure("Delete failed");
                }
            }
            catch (Exception ex)
            {
                return Response<MessageResponse>.Failure($"Delete failed: {ex.Message}");
            }
        }

        public async Task<Response<List<object>>> GetFromMultipleQuery<T0, T1>(string sqlQuery, object sqlParam, CommandType queryType = CommandType.StoredProcedure)
        {
            using var connection = _context.CreateConnection();

            try
            {
                using var gridReader = await connection.QueryMultipleAsync(sqlQuery, sqlParam, commandType: queryType);
                var result = new List<object>();
                var set0 = await gridReader.ReadAsync<T0>();
                if (set0 != null) result.AddRange(set0.Cast<object>());
                var set1 = await gridReader.ReadAsync<T1>();
                if (set1 != null) result.AddRange(set1.Cast<object>());

                return Response<List<object>>.Success(result);
            }
            catch (Exception ex)
            {
                return Response<List<object>>.Failure($"Multiple query failed: {ex.Message}");
            }
        }

        public async Task<Response<List<object>>> GetFromMultipleQuery<T0, T1, T2>(string sqlQuery, object sqlParam, CommandType queryType = CommandType.StoredProcedure)
        {
            using var connection = _context.CreateConnection();

            try
            {
                using var gridReader = await connection.QueryMultipleAsync(sqlQuery, sqlParam, commandType: queryType);
                var result = new List<object>();
                var set0 = await gridReader.ReadAsync<T0>();
                if (set0 != null) result.AddRange(set0.Cast<object>());
                var set1 = await gridReader.ReadAsync<T1>();
                if (set1 != null) result.AddRange(set1.Cast<object>());
                var set2 = await gridReader.ReadAsync<T2>();
                if (set2 != null) result.AddRange(set2.Cast<object>());

                return Response<List<object>>.Success(result);
            }
            catch (Exception ex)
            {
                return Response<List<object>>.Failure($"Multiple query failed: {ex.Message}");
            }
        }

        public async Task<Response<List<object>>> GetFromMultipleQuery<T0, T1, T2, T3>(string sqlQuery, object sqlParam, CommandType queryType = CommandType.StoredProcedure)
        {
            using var connection = _context.CreateConnection();

            try
            {
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

                return Response<List<object>>.Success(result);
            }
            catch (Exception ex)
            {
                return Response<List<object>>.Failure($"Multiple query failed: {ex.Message}");
            }
        }
    }
}