namespace Auth.UI.src.Common
{
    public interface IResponse<T>
    {
        bool Succeeded { get; set; }
        string? Message { get; set; }
        T? Data { get; set; }
    }

    public class Response<T> : IResponse<T>
    {
        public bool Succeeded { get; set; }
        public string? Message { get; set; }
        public T? Data { get; set; }

        public static Response<T> Success(T data, string? message = null)
            => new() { Succeeded = true, Data = data, Message = message };

        public static Response<T> Failure(string message)
            => new() { Succeeded = false, Message = message };
    }

    public class MessageResponse
    {
        public bool Succeeded { get; set; }
        public string Message { get; set; } = string.Empty;

        public static MessageResponse Success(string message) => new() { Succeeded = true, Message = message };
        public static MessageResponse Failure(string message) => new() { Succeeded = false, Message = message };
    }
}
