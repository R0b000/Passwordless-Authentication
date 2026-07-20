namespace Auth.API.Service.Interface.Security
{
    public interface IEmailService
    {
        Task SendAsync(string to, string subject, string body);
    }
}
