using DataAccess.Dto.Request;
using DataAccess.Entities;
using System.Threading.Tasks;

namespace Business.Contracts
{
    public interface IUserService
    {
        public Task<UserInfo> GetUserInfo(UserCredReqDto userCred);
        public TokenResponse AuthenticateUser(UserInfo user);
        public string FromHexString(string hexString);
        public string DecryptText(string text);
    }
}
