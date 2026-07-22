namespace Auth.Model.Models.Common
{
    public class MessageResponse
    {
        public bool Succeeded { get; set; }
        public string? Message { get; set; }

        public static MessageResponse Success(string? message = null)
        {
            return new MessageResponse
            {
                Succeeded = true,
                Message = message ?? "Operation successful"
            };
        }

        public static MessageResponse Failure(string message)
        {
            return new MessageResponse
            {
                Succeeded = false,
                Message = message
            };
        }
    }
}

