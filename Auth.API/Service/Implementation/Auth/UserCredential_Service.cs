using Auth.Model.Models.Entities;
using Shared.Data.Repository.Interface;
using Shared.Data.Wrapper;
using Auth.API.Service.Interface.Auth;

namespace Auth.API.Service.Implementation.Auth
{
    public class UserCredentialService : IUserCredentialService
    {
        private readonly IDapperRepository _dapperRepository;
        private static string ProcedureName = "sp_Users";

        public UserCredentialService(IDapperRepository dapperRepository)
        {
            _dapperRepository = dapperRepository;
        }

        public async Task<IResponse<List<UserCredential>>> GetUserCredentialsAsync(int userId)
        {
            var credentials = (await _dapperRepository.QueryAsync<UserCredential>(
                ProcedureName,
                new { AuthType = "FIDO", FIDOOperation = "GetCredentialsByUserId", UserId = userId }));

            return Response<List<UserCredential>>.Success(credentials?.ToList() ?? new List<UserCredential>());
        }

        public async Task<IResponse<bool>> HasCredentialsAsync(int userId)
        {
            var credentials = (await GetUserCredentialsAsync(userId)).Data ?? new List<UserCredential>();
            return Response<bool>.Success(credentials.Any());
        }
    }
}


