using PasswordlessApi.Models;
using System.Data;
using Dapper;

namespace PasswordlessApi.Data
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
            var result = await _dapperRepository.ExecuteAsync(procedureName, dataType);
            return Response<int>.Success(result);
        }

        public async Task<Response<T>> QuerySingleAsync(string procedureName, object dataType)
        {
            var result = await _dapperRepository.QuerySingleAsync<T>(procedureName, dataType);
            return Response<T>.Success(result);
        }

        public async Task<Response<List<T>>> QueryAsync(string procedureName, object dataType)
        {
            var result = (await _dapperRepository.QueryAsync<T>(procedureName, dataType)).ToList();
            return Response<List<T>>.Success(result);
        }

        public async Task<List<TResult>> GetAllAsync<TResult>(string spName, object obj, CommandType queryType = CommandType.StoredProcedure)
        {
            return await _dapperRepository.GetAllAsync<TResult>(spName, obj, queryType);
        }

        public async Task<TResult> GetAsync<TResult>(string spName, object obj, CommandType queryType = CommandType.StoredProcedure)
        {
            return await _dapperRepository.GetAsync<TResult>(spName, obj, queryType);
        }

        public async Task<SqlMapper.GridReader> GetMultipleListAsync(string spName, object obj, CommandType queryType = CommandType.StoredProcedure)
        {
            return await _dapperRepository.GetMultipleListAsync(spName, obj, queryType);
        }

        public async Task<MessageResponse> UpdateAsync(string spName, object input, CommandType queryType = CommandType.StoredProcedure)
        {
            return await _dapperRepository.UpdateAsync(spName, input, queryType);
        }

        public async Task<MessageResponse> InsertAsync(string spName, object input, CommandType queryType = CommandType.StoredProcedure)
        {
            return await _dapperRepository.InsertAsync(spName, input, queryType);
        }

        public async Task<MessageResponse> DeleteAsync(string spName, object input, CommandType queryType = CommandType.StoredProcedure)
        {
            return await _dapperRepository.DeleteAsync(spName, input, queryType);
        }

        public async Task<List<object>> GetFromMultipleQuery<T0, T1>(string sqlQuery, object sqlParam, CommandType queryType = CommandType.StoredProcedure)
        {
            return await _dapperRepository.GetFromMultipleQuery<T0, T1>(sqlQuery, sqlParam, queryType);
        }

        public async Task<List<object>> GetFromMultipleQuery<T0, T1, T2>(string sqlQuery, object sqlParam, CommandType queryType = CommandType.StoredProcedure)
        {
            return await _dapperRepository.GetFromMultipleQuery<T0, T1, T2>(sqlQuery, sqlParam, queryType);
        }

        public async Task<List<object>> GetFromMultipleQuery<T0, T1, T2, T3>(string sqlQuery, object sqlParam, CommandType queryType = CommandType.StoredProcedure)
        {
            return await _dapperRepository.GetFromMultipleQuery<T0, T1, T2, T3>(sqlQuery, sqlParam, queryType);
        }
    }
}
