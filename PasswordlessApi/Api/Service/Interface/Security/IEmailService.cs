using PasswordlessApi.Api.Models.Common;

namespace PasswordlessApi.Api.Service.Interface.Security
{
    public interface IEmailService
    {
        Task SendAsync(string to, string subject, string body);
    }
}
