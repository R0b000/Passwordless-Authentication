using System.Security.Cryptography;

namespace PasswordlessApi.Api.Utility.OtpGenerator
{
    public class GenerateSecureOtp
    {
        public static string GenerateSecureOtpCode()
        {
            var value = RandomNumberGenerator.GetInt32(0, 1000000);
            return value.ToString("D6");
        }
    }
}
