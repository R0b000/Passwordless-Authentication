using Shared.Data.Wrapper;
using Auth.Model.Models.Account;

namespace Account.API.Service.Interface
{
    public interface IUserProfileService
    {
        Task<IResponse<UserProfileResponse?>> GetProfileAsync(int userId);
        Task<IResponse<UserProfileResponse?>> UpdateProfileAsync(int userId, UpdateProfileRequest request);
    }
}
