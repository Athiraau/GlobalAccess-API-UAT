using DataAccess.Entities;
using DataAccess.Dto.Request;

namespace DataAccess.Contracts
{
    public interface IUserRepository 
    {
        public Task<UserInfo> GetUserInfo(UserCredReqDto userCred);
        public TokenResponse AuthenticateUser(UserInfo user);
    }
}
