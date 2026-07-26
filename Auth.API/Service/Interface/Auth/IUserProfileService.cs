using Shared.Data.Wrapper;
using Auth.Model.Models.Account;

namespace Auth.API.Service.Interface.Auth
{
    public interface IUserProfileService
    {
        Task<IResponse<UserProfileResponse?>> GetProfileAsync(int userId);
        Task<IResponse<UserProfileResponse?>> UpdateProfileAsync(int userId, UpdateProfileRequest request);
    }
}
