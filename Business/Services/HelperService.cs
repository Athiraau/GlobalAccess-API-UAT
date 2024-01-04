using Business.Contracts;
using DataAccess.Contracts;
using DataAccess.Dto;
using DataAccess.Dto.Request;
using DataAccess.Dto.Response;
using DataAccess.Enums;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Text.Unicode;


namespace Business.Services
{
    public class HelperService: IHelperService
    {
        public readonly IConfiguration _config;
        private readonly IRepositoryWrapper _repository;
        private readonly IWebHostEnvironment _env;

        private readonly DtoWrapper _dto;
        private readonly ServiceHelper _helper;
        private string _Npassword = "";
        private string _Dpassword = "";
        private readonly string _securityKey;

        public HelperService(IRepositoryWrapper repository, ServiceHelper helper, IWebHostEnvironment env, 
            DtoWrapper dto, IConfiguration config)
        {
            _repository = repository;
            _helper = helper;
            _env = env;
            _dto = dto;
            _config = config;
        }

        public async Task<CheckCredentialResDto> CheckEmployeeCredentials(int empCode, string password)
        {
            //7771762f4e4733392b5a3670417a71477770736a6c773d3d

            _Dpassword = FromHexString(password);
            _Npassword = DecryptText(_Dpassword);
            if (!_env.IsDevelopment())
            {
                password = _helper.PHelper.RemoveSpecialCharacters(_helper.PHelper.Encrypt(Convert.ToString(empCode), _Npassword));
            }
            CheckCredentialResDto checkCredRes = await _repository.Helper.EmpPasswordCheck(empCode, _Npassword);
            return checkCredRes;
        }

        public async Task<CheckCredentialResDto> HostnameCheck(string hostName)
        {
            CheckCredentialResDto checkCredRes = await _repository.Helper.HostnameCheck(hostName);
            return checkCredRes;
        }

        public async Task<CheckCredentialResDto> ResetPassword(PwdResetReqDto pwdReq)
        {
            CheckCredentialResDto resetPwdRes = null;
            if (!_env.IsDevelopment())
            {
                byte[] data = _helper.PHelper.getEdata(Convert.ToString(pwdReq.empCode), "software");
                resetPwdRes = await _repository.Helper.ResetPasswordProd(pwdReq.empCode, pwdReq.ifStaff, data);
            }
            else
            {
                resetPwdRes = await _repository.Helper.ResetPassword(pwdReq.empCode, pwdReq.ifStaff, "software");
            }
            return resetPwdRes;
        }

        public async Task LoginTracker(int userId, int status, string sip, int appId)
        {
            await _repository.Helper.LoginTracker(userId, status, sip, appId);
        }

        public async Task<ChangePasswordResDto> ChangePassword(PwdChangeReqDto pwdReq)
        {
            ChangePasswordResDto checkRes = null;
            if (!_env.IsDevelopment())
            {
                byte[] data = _helper.PHelper.getEdata(Convert.ToString(pwdReq.empCode), pwdReq.password);
                byte[] oldData = _helper.PHelper.getEdata(Convert.ToString(pwdReq.empCode), pwdReq.oldPassword);
                checkRes = await _repository.Helper.ChangePasswordProd(pwdReq.empCode, data, oldData);
            }
            else
            {
                checkRes = await _repository.Helper.ChangePassword(pwdReq);
            }
            return checkRes;
        }

        public async Task<GetRolesResDto> GetRoles(int userName, string password)
        {           
            GetRolesResDto getRoles = null;

            if (!_env.IsDevelopment())   //prod
            {
                
                _Dpassword = FromHexString(password);
                _Npassword = DecryptText(_Dpassword);

                byte[] data = _helper.PHelper.getEdata(Convert.ToString(userName), _Npassword);
                getRoles = await _repository.Helper.GetRolesProd(userName, data);
            }
            else   //uat
            {
                 _Dpassword = FromHexString(password);
                 _Npassword =  DecryptText(_Dpassword);
                 getRoles = await _repository.Helper.GetRoles(userName, _Npassword);
            }   
          
            getRoles = _helper.PHelper.SetAlertMessage(getRoles);           

            return getRoles;
        }
        public string FromHexString(string hexString)
        {
           // hexString = "7771762f4e4733392b5a3670417a71477770736a6c773d3d";
            if (hexString == null || (hexString.Length & 1) == 1)
            {
                throw new ArgumentException();
            }
            var sb = new StringBuilder();
            for (var i = 0; i < hexString.Length; i += 2)
            {
                var hexChar = hexString.Substring(i, 2);
                sb.Append((char)Convert.ToByte(hexChar, 16));
            }
            return sb.ToString();
        }

        public string EncryptText(string text)
        {
            string keyString = "8080808080808080";
            string ivString = "8080808080808080";

            byte[] key = Encoding.UTF8.GetBytes(keyString);
            byte[] iv = Encoding.UTF8.GetBytes(ivString);

            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.Key = key;
                aesAlg.IV = iv;
                aesAlg.Mode = CipherMode.CBC;

                ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

                byte[] encryptedBytes;
                using (var msEncrypt = new System.IO.MemoryStream())
                {
                    using (var csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                    {
                        using (var swEncrypt = new System.IO.StreamWriter(csEncrypt))
                        {
                            swEncrypt.Write(text);
                        }
                        encryptedBytes = msEncrypt.ToArray();
                    }
                }

                string base64String = Convert.ToBase64String(encryptedBytes);
                Console.WriteLine(base64String);
                return base64String;
            }
        }
        public string DecryptText(string text)
        {
            // For AES Encryption/Decryption
            string DecryptKey = _config.GetValue<string>("Decrypt:DKey");

            string keyString = DecryptKey;
            string ivString = DecryptKey;
            byte[] key = Encoding.UTF8.GetBytes(keyString);
            byte[] iv = Encoding.UTF8.GetBytes(ivString);

            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.Key = key;
                aesAlg.IV = iv;
                aesAlg.Mode = CipherMode.CBC;

                ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);

                byte[] encryptedBytes = Convert.FromBase64String(text);
                byte[] decryptedBytes = decryptor.TransformFinalBlock(encryptedBytes, 0, encryptedBytes.Length);

                string decryptedText = Encoding.UTF8.GetString(decryptedBytes);
                Console.WriteLine(decryptedText);
                Console.WriteLine("working");
                return decryptedText;
            }
        }
        public async Task<EmployeeStatusResDto> GetEmployeeStatus(long mobile)
        {
            EmployeeStatusResDto empStatus = await _repository.Helper.GetEmployeeStatus(mobile);
            return empStatus;
        }

        public async Task<SMSSendResDto> SendSMS(SMSSendReqDto smsReq)
        {
            string result = _helper.PHelper.SolutionInfiniSend(smsReq.accId, smsReq.pNumber, smsReq.message);
            if(string.IsNullOrEmpty(result)) 
            {
                _dto.SmsSendRes.status = (int)SMSSendStatus.Failed;
                _dto.SmsSendRes.message = "SMS Send - Failed";
            }
            else
            {
                _dto.SmsSendRes.status = (int)SMSSendStatus.Success;
                _dto.SmsSendRes.message = "SMS Send - Success";
            }

            return _dto.SmsSendRes;
        }

        public async Task<SMSSendResDto> SendMail(MailSendReqDto mailReq)
        {
            string server = _config.GetValue<string>("Mail:Server");
            Int32 port = _config.GetValue<int>("Mail:Port");
            Int32 timeout = _config.GetValue<int>("Mail:Timeout");
            string target = _config.GetValue<string>("Mail:Target");
            string sender = _config.GetValue<string>("Mail:Sender");
            string password = _config.GetValue<string>("Mail:Password");


            string result = _helper.PHelper.sendMail(mailReq.mailId, mailReq.subject, mailReq.message, server, port, timeout, target, sender, password);
            if (string.IsNullOrEmpty(result))
            {
                _dto.SmsSendRes.status = (int)SMSSendStatus.Failed;
                _dto.SmsSendRes.message = "Mail Sending Failed";
            }
            else
            {
                _dto.SmsSendRes.status = (int)SMSSendStatus.Success;
                _dto.SmsSendRes.message = "Mail sent successfully";
            }

            return _dto.SmsSendRes;
        }
    }
}
