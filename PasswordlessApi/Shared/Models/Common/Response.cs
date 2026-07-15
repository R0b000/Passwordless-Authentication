namespace API.Shared.Models.Common
{
    public class Response<T>
    {
        public T? Data { get; set; }

        public bool Succeeded { get; set; } = false;
        public string? Messages { get; set; }

        public static Response<T> Success(T data, string? message = null)
        {
            return new Response<T>
            {
                Data = data,
                Succeeded = true,
                Messages = message ?? "Operation successful"
            };
        }

        public static Response<T> Failure(string message, T? data = default)
        {
            return new Response<T>
            {
                Data = data,
                Succeeded = false,
                Messages = message
            };
        }
    }
}