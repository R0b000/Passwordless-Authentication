using PasswordlessApi.Models;

namespace PasswordlessApi.Data
{
    public interface IGenericRepository<T> where T : class
    {
        Task<Response<int>> CreateAsync(string tableName, T entity);
        Task<Response<T>> GetByIdAsync(string tableName, int id);
        Task<Response<List<T>>> GetAllAsync(string tableName);
        Task<Response<int>> UpdateAsync(string tableName, T entity);
        Task<Response<int>> DeleteAsync(string tableName, int id);
    }
}