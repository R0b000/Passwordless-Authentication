using PasswordlessApi.Api.Models.Entities;
using PasswordlessApi.Api.Models.RequestModel.Auth;
using PasswordlessApi.Api.Models.ResponseModel.Auth;
using PasswordlessApi.Api.Service.Interface.Auth;
using PasswordlessApi.Api.Service.Interface.Repository;
using PasswordlessApi.Api.Service.Interface.Rbac;
using PasswordlessApi.Api.Utility.Jwt;
using PasswordlessApi.Api.Utility.OtpGenerator;
using PasswordlessApi.Api.Utility.PasswordHash;

namespace PasswordlessApi.Api.Service.Implementation.Auth
{
    public class OtpService : IOtpService
    {
        private readonly IGenericRepository<User> _userRepository;
        private readonly IDapperRepository _dapperRepository;
        private readonly IJwtHelper _jwtHelper;
        private readonly IPasswordHash _passwordHash;
        private readonly IUserRoleService _userRoleService;
        private readonly IRoleService _roleService;
        private const string ProcedureName = "sp_Users";

        public OtpService(
            IGenericRepository<User> userRepository,
            IDapperRepository dapperRepository,
            IJwtHelper jwtHelper,
            IPasswordHash passwordHash,
            IUserRoleService userRoleService,
            IRoleService roleService)
        {
            _userRepository = userRepository;
            _dapperRepository = dapperRepository;
            _jwtHelper = jwtHelper;
            _passwordHash = passwordHash;
            _userRoleService = userRoleService;
            _roleService = roleService;
        }

        public async Task<OtpResponse> RequestOtpAsync(OtpRequest request)
        {
            var userResult = await _userRepository.QuerySingleAsync(
                ProcedureName,
                new { AuthType = "Login", UserId = request.UserId });

            if (userResult == null || !userResult.Succeeded || userResult.Data == null)
            {
                return new OtpResponse { Success = false, Message = "User not found" };
            }

            var user = userResult.Data;

            if (string.IsNullOrEmpty(user.Email))
            {
                return new OtpResponse { Success = false, Message = "User does not have an email configured" };
            }

            var otp = GenerateSecureOtp.GenerateSecureOtpCode();
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
                new { AuthType = "Login", UserId = request.UserId });

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
            var refreshToken = await CreateRefreshTokenAsync(user.Id);

            await AssignDefaultRoleIfMissingAsync(user.Id);

            var userWithRoles = await _userRoleService.GetUserWithRolesAndPermissionsAsync(user.Id);

            return new AuthResponse
            {
                UserId = user.Id,
                Username = user.Username,
                Email = user.Email,
                Token = token,
                RefreshToken = refreshToken,
                Message = "Login successful",
                RequiresFido2 = false,
                Role = userWithRoles?.Role,
                Permissions = userWithRoles?.Permissions ?? new List<string>()
            };
        }

        private async Task<string> CreateRefreshTokenAsync(int userId)
        {
            var now = DateTime.UtcNow;
            var refreshToken = _jwtHelper.GenerateRefreshToken();
            var refreshTokenHash = _passwordHash.HashPassword(refreshToken);
            var refreshExpiryDays = _jwtHelper.GetRefreshTokenExpiryDays();

            await _dapperRepository.ExecuteAsync(
                ProcedureName,
                new
                {
                    AuthType = "RefreshToken",
                    FIDOOperation = "CreateRefreshToken",
                    UserId = userId,
                    Token = refreshTokenHash,
                    ExpiresAt = now.AddDays(refreshExpiryDays),
                    Now = now
                });

            return refreshToken;
        }

        private async Task AssignDefaultRoleIfMissingAsync(int userId)
        {
            var userRoles = await _userRoleService.GetUserRoleNamesAsync(userId);
            if (!userRoles.Any())
            {
                var user = await _userRepository.QuerySingleAsync(
                    ProcedureName,
                    new { AuthType = "Login", UserId = userId });

                if (user.Succeeded && user.Data != null && !string.IsNullOrEmpty(user.Data.Username))
                {
                    var roleName = user.Data.Username.Equals("admin", StringComparison.OrdinalIgnoreCase) ? "Admin" : "User";

                    var role = await _roleService.GetRoleByNameAsync(roleName);
                    if (role == null)
                    {
                        role = await _roleService.CreateRoleAsync(roleName, $"Default {roleName} role");
                    }

                    if (role != null)
                    {
                        await _userRoleService.AssignRoleToUserAsync(userId, role.Id);
                    }
                }
            }
        }
    }
}
