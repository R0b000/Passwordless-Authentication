using Auth.API.Service.Interface.Security;
using Microsoft.Extensions.Logging;

namespace Auth.API.Service.Implementation.Security
{
    public class LoggingEmailService : IEmailService
    {
        private readonly ILogger<LoggingEmailService> _logger;

        public LoggingEmailService(ILogger<LoggingEmailService> logger)
        {
            _logger = logger;
        }

        public Task SendAsync(string to, string subject, string body)
        {
            _logger.LogInformation("Email to {To}: {Subject}\n{Body}", to, subject, body);
            return Task.CompletedTask;
        }
    }
}
