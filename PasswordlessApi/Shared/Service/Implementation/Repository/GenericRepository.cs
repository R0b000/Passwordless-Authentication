using System.Data;
using Dapper;
using API.Shared.Models.Common;
using API.Shared.Service.Interface.Repository;

namespace API.Shared.Service.Implementation.Repository
{
    public class GenericRepository<T> : IGenericRepository<T> where T : class
    {
        private readonly IDapperRepository _dapperRepository;

        public GenericRepository(IDapperRepository dapperRepository)
        {
            _dapperRepository = dapperRepository;
        }

        public async Task<Response<int>> ExecuteAsync(string procedureName, object dataType)
        {
            return await _dapperRepository.ExecuteAsync(procedureName, dataType);
        }

        public async Task<Response<T>> QuerySingleAsync(string procedureName, object dataType)
        {
            var result = await _dapperRepository.QuerySingleAsync<T>(procedureName, dataType);
            return result.Succeeded
                ? Response<T>.Success(result.Data!)
                : Response<T>.Failure(result.Messages!);
        }

        public async Task<Response<List<T>>> QueryAsync(string procedureName, object dataType)
        {
            var result = await _dapperRepository.QueryAsync<T>(procedureName, dataType);
            return result.Succeeded
                ? Response<List<T>>.Success(result.Data!.ToList())
                : Response<List<T>>.Failure(result.Messages!);
        }

        public async Task<Response<TResult>> QuerySingleAsync<TResult>(string procedureName, object dataType)
        {
            var result = await _dapperRepository.QuerySingleAsync<TResult>(procedureName, dataType);
            return result.Succeeded
                ? Response<TResult>.Success(result.Data!)
                : Response<TResult>.Failure(result.Messages!);
        }

        public async Task<Response<List<TResult>>> QueryAsync<TResult>(string procedureName, object dataType)
        {
            var result = await _dapperRepository.QueryAsync<TResult>(procedureName, dataType);
            return result.Succeeded
                ? Response<List<TResult>>.Success(result.Data!.ToList())
                : Response<List<TResult>>.Failure(result.Messages!);
        }

        public async Task<T?> QueryFirstAsync(string procedureName, object dataType)
        {
            return (await _dapperRepository.QueryFirstAsync<T>(procedureName, dataType)).Data;
        }

        public async Task<TResult?> QueryFirstAsync<TResult>(string procedureName, object dataType)
        {
            return (await _dapperRepository.QueryFirstAsync<TResult>(procedureName, dataType)).Data;
        }

        public async Task<SqlMapper.GridReader> QueryMultipleAsync(string procedureName, object dataType)
        {
            return (await _dapperRepository.QueryMultipleAsync(procedureName, dataType)).Data!;
        }

        public async Task<List<TResult>> GetAllAsync<TResult>(string spName, object obj, CommandType queryType = CommandType.StoredProcedure)
        {
            var result = await _dapperRepository.GetAllAsync<TResult>(spName, obj, queryType);
            return result.Data ?? new List<TResult>();
        }

        public async Task<TResult> GetAsync<TResult>(string spName, object obj, CommandType queryType = CommandType.StoredProcedure)
        {
            return (await _dapperRepository.GetAsync<TResult>(spName, obj, queryType)).Data!;
        }

        public async Task<SqlMapper.GridReader> GetMultipleListAsync(string spName, object obj, CommandType queryType = CommandType.StoredProcedure)
        {
            return (await _dapperRepository.GetMultipleListAsync(spName, obj, queryType)).Data!;
        }

        public async Task<MessageResponse> UpdateAsync(string spName, object input, CommandType queryType = CommandType.StoredProcedure)
        {
            return (await _dapperRepository.UpdateAsync(spName, input, queryType)).Data!;
        }

        public async Task<MessageResponse> InsertAsync(string spName, object input, CommandType queryType = CommandType.StoredProcedure)
        {
            return (await _dapperRepository.InsertAsync(spName, input, queryType)).Data!;
        }

        public async Task<MessageResponse> DeleteAsync(string spName, object input, CommandType queryType = CommandType.StoredProcedure)
        {
            return (await _dapperRepository.DeleteAsync(spName, input, queryType)).Data!;
        }

        public async Task<List<object>> GetFromMultipleQuery<T0, T1>(string sqlQuery, object sqlParam, CommandType queryType = CommandType.StoredProcedure)
        {
            return (await _dapperRepository.GetFromMultipleQuery<T0, T1>(sqlQuery, sqlParam, queryType)).Data!;
        }

        public async Task<List<object>> GetFromMultipleQuery<T0, T1, T2>(string sqlQuery, object sqlParam, CommandType queryType = CommandType.StoredProcedure)
        {
            return (await _dapperRepository.GetFromMultipleQuery<T0, T1, T2>(sqlQuery, sqlParam, queryType)).Data!;
        }

        public async Task<List<object>> GetFromMultipleQuery<T0, T1, T2, T3>(string sqlQuery, object sqlParam, CommandType queryType = CommandType.StoredProcedure)
        {
            return (await _dapperRepository.GetFromMultipleQuery<T0, T1, T2, T3>(sqlQuery, sqlParam, queryType)).Data!;
        }
    }
}
