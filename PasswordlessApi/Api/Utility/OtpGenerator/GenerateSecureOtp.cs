namespace PasswordlessApi.Api.Utility.OtpGenerator
{
    public class GenerateSecureOtp
    {
        public static string GenerateSecureOtpCode() {
            using var rng = RandomNumberGenerator.Create();
            var bytes = new byte[4];
            rng.GetBytes(bytes);
            var value = BitConverter.ToUInt32(bytes, 0) % 1000000;
            return value.ToString("D6");
        }
    }
}