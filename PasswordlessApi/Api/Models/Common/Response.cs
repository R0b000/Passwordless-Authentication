// Models/Response.cs
namespace PasswordlessApi.Api.Models.Common
{
    public class Response
    {
        public bool Succeeded { get; set; }
        public string? Message { get; set; }
        
        public static Response Success(string? message = null)
        {
            return new Response 
            { 
                Succeeded = true, 
                Message = message ?? "Operation successful" 
            };
        }
        
        public static Response Failure(string message)
        {
            return new Response 
            { 
                Succeeded = false, 
                Message = message 
            };
        }
    }
}