using Auth.Model.Models.Account;
using Auth.Model.Models.Entities;
using Shared.Data.Repository.Interface;
using Shared.Data.Wrapper;
using Auth.API.Config;
using Auth.API.Service.Interface.Auth;
using Auth.API.Service.Interface.Security;

namespace Auth.API.Service.Implementation.Auth
{
    public class UserProfileService : IUserProfileService
    {
        private readonly IGenericRepository<User> _userRepository;
        private readonly IAuditLogService _auditLogService;
        private const string ProcedureName = DbConstants.Procedures.Users;

        public UserProfileService(IGenericRepository<User> userRepository, IAuditLogService auditLogService)
        {
            _userRepository = userRepository;
            _auditLogService = auditLogService;
        }

        public async Task<IResponse<UserProfileResponse?>> GetProfileAsync(int userId)
        {
            var userResult = await _userRepository.QuerySingleAsync<User>(
                ProcedureName,
                new { AuthType = DbConstants.AuthTypes.Login, UserId = userId });

            if (userResult.Succeeded && userResult.Data != null)
            {
                var profileResult = await _userRepository.QuerySingleAsync<UserProfileResponse>(
                    ProcedureName,
                    new { AuthType = DbConstants.AuthTypes.GetProfile, UserId = userId });

                if (profileResult.Succeeded && profileResult.Data != null)
                    return Response<UserProfileResponse?>.Success(profileResult.Data);

                return Response<UserProfileResponse?>.Success(new UserProfileResponse
                {
                    UserId = userResult.Data.Id,
                    Username = userResult.Data.Username ?? string.Empty,
                    Email = userResult.Data.Email ?? string.Empty,
                    DateJoined = userResult.Data.CreatedAt,
                    AccountStatus = "active"
                });
            }

            return Response<UserProfileResponse?>.Fail("User not found");
        }

        public async Task<IResponse<UserProfileResponse?>> UpdateProfileAsync(int userId, UpdateProfileRequest request)
        {
            var result = await _userRepository.QuerySingleAsync<UserProfileResponse>(
                ProcedureName,
                new
                {
                    AuthType = DbConstants.AuthTypes.UpdateProfile,
                    UserId = userId,
                    request.Username,
                    request.Email,
                    request.Phone,
                    request.Bio,
                    Now = DateTime.UtcNow
                });

            if (result.Succeeded && result.Data != null)
            {
                await _auditLogService.LogAsync(userId, "ProfileUpdated", "User", userId.ToString(), null, "Profile updated");
                return Response<UserProfileResponse?>.Success(result.Data);
            }

            return Response<UserProfileResponse?>.Fail("Failed to update profile");
        }
    }
}
