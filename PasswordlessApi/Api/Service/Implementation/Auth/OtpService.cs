using PasswordlessApi.Api.Models.Entities;
using PasswordlessApi.Api.Models.RequestModel.Auth;
using PasswordlessApi.Api.Models.ResponseModel.Auth;
using PasswordlessApi.Api.Service.Interface.Auth;
using PasswordlessApi.Api.Service.Interface.Repository;
using PasswordlessApi.Api.Utility.Jwt;

namespace PasswordlessApi.Api.Service.Implementation.Auth
{
    public class OtpService : IOtpService
    {
        private readonly IGenericRepository<User> _userRepository;
        private readonly IDapperRepository _dapperRepository;
        private readonly IJwtHelper _jwtHelper;
        private const string ProcedureName = "sp_Users";

        public OtpService(
            IGenericRepository<User> userRepository,
            IDapperRepository dapperRepository,
            IJwtHelper jwtHelper)
        {
            _userRepository = userRepository;
            _dapperRepository = dapperRepository;
            _jwtHelper = jwtHelper;
        }

        public async Task<OtpResponse> RequestOtpAsync(OtpRequest request)
        {
            var userResult = await _userRepository.QuerySingleAsync(
                ProcedureName,
                new { AuthType = "GetById", UserId = request.UserId });

            if (userResult == null || !userResult.Succeeded || userResult.Data == null)
            {
                return new OtpResponse { Success = false, Message = "User not found" };
            }

            var user = userResult.Data;

            if (string.IsNullOrEmpty(user.Email))
            {
                return new OtpResponse { Success = false, Message = "User does not have an email configured" };
            }

            var otp = GenerateOtp();
            var expiresAt = DateTime.UtcNow.AddMinutes(5);

            await _dapperRepository.ExecuteAsync(
                ProcedureName,
                new
                {
                    AuthType = "EmailOtp",
                    FIDOOperation = "CreateOtp",
                    UserId = user.Id,
                    Otp = otp,
                    ExpiresAt = expiresAt
                });

            return new OtpResponse
            {
                Success = true,
                Message = $"OTP sent to {user.Email} (Demo OTP: {otp})",
                Otp = otp
            };
        }

        public async Task<AuthResponse> VerifyOtpAsync(OtpVerifyRequest request)
        {
            var userResult = await _userRepository.QuerySingleAsync(
                ProcedureName,
                new { AuthType = "GetById", UserId = request.UserId });

            if (userResult == null || !userResult.Succeeded || userResult.Data == null)
            {
                return new AuthResponse { Message = "User not found" };
            }

            var user = userResult.Data;

            var isConsumed = await _dapperRepository.QuerySingleAsync<bool>(
                ProcedureName,
                new
                {
                    AuthType = "EmailOtp",
                    FIDOOperation = "ConsumeOtp",
                    UserId = request.UserId,
                    Otp = request.Otp,
                    Now = DateTime.UtcNow
                });

            if (!isConsumed)
            {
                return new AuthResponse { Message = "Invalid or expired OTP" };
            }

            var token = _jwtHelper.GenerateToken(user.Id, user.Username);

            return new AuthResponse
            {
                UserId = user.Id,
                Username = user.Username,
                Email = user.Email,
                Token = token,
                Message = "Login successful",
                RequiresFido2 = false
            };
        }
    }
}