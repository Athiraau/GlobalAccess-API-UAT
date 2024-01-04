using DataAccess.Dto.Request;
using DataAccess.Dto.Response;
using DataAccess.Entities;
using System;
using System.Threading.Tasks;

namespace DataAccess.Contracts
{
    public interface IHelperRepository
    {
        public Task<CheckCredentialResDto> EmpPasswordCheck(int empCode, string password);
        public Task<CheckCredentialResDto> HostnameCheck(string hostName);
        public Task<CheckCredentialResDto> ResetPasswordProd(int userName, string ifStaff, byte[] data);
        public Task<CheckCredentialResDto> ResetPassword(int userName, string ifStaff, string data);
        public Task LoginTracker(int userId, int status, string sip, int appId);
        public Task<ChangePasswordResDto> ChangePasswordProd(int userName, byte[] password, byte[] oldPassword);
        public Task<ChangePasswordResDto> ChangePassword(PwdChangeReqDto pwdReq);
        public Task<GetRolesResDto> GetRolesProd(int userName, byte[] password);
        public Task<GetRolesResDto> GetRoles(int userName, string password);
        public Task<EmployeeStatusResDto> GetEmployeeStatus(long mobile);        
    }
}
