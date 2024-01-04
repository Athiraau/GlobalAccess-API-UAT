using DataAccess.Dto.Request;
using DataAccess.Dto.Response;
using DataAccess.Entities;
using System;
using System.Threading.Tasks;

namespace Business.Contracts
{
    public interface IHelperService
    {
        public Task<CheckCredentialResDto> CheckEmployeeCredentials(int empCode, string password);
        public Task<CheckCredentialResDto> HostnameCheck(string hostName);
        public Task<CheckCredentialResDto> ResetPassword(PwdResetReqDto pwdReq);
        public Task LoginTracker(int userId, int status, string sip, int appId);
        public Task<ChangePasswordResDto> ChangePassword(PwdChangeReqDto pwdReq);
        public Task<GetRolesResDto> GetRoles(int userName, string password);
        public Task<EmployeeStatusResDto> GetEmployeeStatus(long mobile);
        public Task<SMSSendResDto> SendSMS(SMSSendReqDto smsReq);
        public Task<SMSSendResDto> SendMail(MailSendReqDto mailReq);
        public string DecryptText(string text);
        public string EncryptText(string text);
        public string FromHexString(string hexString);

    }
}
