using PasswordlessApi.Api.Models.Entities;
using PasswordlessApi.Api.Service.Interface.Auth;
using PasswordlessApi.Api.Service.Interface.Repository;

namespace PasswordlessApi.Api.Service.Implementation.Auth
{
    public class UserCredentialService : IUserCredentialService
    {
        private readonly IDapperRepository _dapperRepository;
        private static string ProcedureName = "sp_Users";

        public UserCredentialService(IDapperRepository dapperRepository)
        {
            _dapperRepository = dapperRepository;
        }

        public async Task<List<UserCredential>> GetUserCredentialsAsync(int userId)
        {
            var credentials = await _dapperRepository.QueryAsync<UserCredential>(
                ProcedureName,
                new { AuthType = "FIDO", FIDOOperation = "GetCredentialsByUserId", UserId = userId });

            return credentials.ToList();
        }

        public async Task<bool> HasCredentialsAsync(int userId)
        {
            var credentials = await GetUserCredentialsAsync(userId);
            return credentials.Any();
        }
    }
}
