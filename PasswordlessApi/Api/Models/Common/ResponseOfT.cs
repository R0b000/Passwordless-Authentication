// Models/ResponseOfT.cs
namespace PasswordlessApi.Api.Models.Common
{
    public class Response<T> : Response
    {
        public T? Data { get; set; }
        
        public static Response<T> Success(T data, string? message = null)
        {
            return new Response<T> 
            { 
                Succeeded = true, 
                Data = data,
                Message = message ?? "Operation successful" 
            };
        }
        
        public new static Response<T> Failure(string message)
        {
            return new Response<T> 
            { 
                Succeeded = false, 
                Message = message 
            };
        }
    }
}