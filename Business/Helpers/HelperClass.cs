using DataAccess.Contracts;
using DataAccess.Dto.Response;
using Microsoft.Extensions.Configuration;
using System;
using System.IO;
using System.Net;
using System.Net.Mail;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;

namespace Business.Helpers
{
    public class HelperClass
    {
		private readonly IRepositoryWrapper _repository;
		private readonly string _securityKey;
        private  string _Npassword="";

		public HelperClass(IRepositoryWrapper repository)
		{
			_repository = repository;
			_securityKey = "raju";
		}

		public string Encrypt(string eCode, string Pwd)
		{
			MD5CryptoServiceProvider md5Hasher = new MD5CryptoServiceProvider();
			byte[] hashedDataBytes;
			UTF8Encoding encoder = new UTF8Encoding();
			hashedDataBytes = md5Hasher.ComputeHash(encoder.GetBytes(eCode + _securityKey + Pwd));
			
			//Convert and return the encrypted data/byte into string format.
			return BitConverter.ToString(hashedDataBytes, 0, hashedDataBytes.Length);
		}

		public string RemoveSpecialCharacters(string str)
		{
			return Regex.Replace(str, "[^a-zA-Z0-9_.]+", "", RegexOptions.Compiled);
		}

		public byte[] getEdata(string userName, string password)
		{
            //_Npassword = Decrypt(password);
          

            bool flag = String.Compare(password.ToLower(), "software", false) == 0;
			if (flag)
			{
				userName = "ITJOINING";
			}
			MD5CryptoServiceProvider mD5CryptoServiceProvider = new MD5CryptoServiceProvider();
			UTF8Encoding uTF8Encoding = new UTF8Encoding();
			return mD5CryptoServiceProvider.ComputeHash(uTF8Encoding.GetBytes(userName + _securityKey + password));
		}

		public string EncryptText(string password)
		{
			RC2CryptoServiceProvider rc2CSP = new RC2CryptoServiceProvider();

			byte[] key = rc2CSP.Key;
			byte[] IV = rc2CSP.IV;

			ICryptoTransform encryptor = rc2CSP.CreateEncryptor(key, IV);

			MemoryStream msEncrypt = new MemoryStream();
			CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write);

			byte[] toEncrypt = Encoding.ASCII.GetBytes(password);

			csEncrypt.Write(toEncrypt, 0, toEncrypt.Length);
			csEncrypt.FlushFinalBlock();
			byte[] encrypted = msEncrypt.ToArray();

			return Convert.ToBase64String(encrypted);
		}


        public string Decrypt(string password)
        {
            var toEncryptArray = Convert.FromBase64String(password);

            return UTF8Encoding.UTF8.GetString(toEncryptArray);
        }

        public GetRolesResDto SetAlertMessage(GetRolesResDto input)
		{
			if (input.flag == 1)
			{
				input.isSuccess = "True";
				input.message = "Success";
			}
			else if (input.flag > 1 && input.flag <= 6)
			{
				input.isSuccess = "Warning";
				input.message = $"Your password will expire in {input.flag - 1} days";
			}
			else if (input.flag == 7 || input.flag == 8)
			{
				input.isSuccess = "False";
				input.message = "E006 - Password Expired. Please change your password and login again";
			}
			else if (input.flag == 92)
			{
				input.isSuccess = "False";
				input.message = "You are temporarily blocked due to continous invaid attempts..!! Please try after 30 minutes.";
			}
			else if (input.accessId == 97)
			{
				input.isSuccess = "False";
				input.message = "Being a resigned employee,access denied...!";
			}
			else
			{
				input.isSuccess = "False";
				input.message = "E011 - Check your user id / password !! You will be blocked for five invalid attempts.";
			}
			return input;
		}

        public string SolutionInfiniSend(int accId, string pNumber, string msg)
        {
            string requestUriString = string.Empty;
            if (accId == 1)
            {
                requestUriString = string.Concat(new string[]
                {
                    "http://bankalerts.sinfini.com/api/web2sms.php?workingkey=Aabf003763d4a95672832f21e2e0725ed&sender=MAFILR&to= ",
                    Convert.ToString(pNumber),
                    " &message=",
                    HttpUtility.UrlEncode(msg),
                    "&type=xml"
                });
            }
            else if (accId == 2)
            {
                requestUriString = string.Concat(new string[]
                {
                    "http://bankalerts.sinfini.com/api/web2sms.php?workingkey=Ab495a7783699c0b49b130a5924fcebef&sender=MAFILT&to= ",
                    Convert.ToString(pNumber),
                    " &message=",
                    HttpUtility.UrlEncode(msg),
                    "&type=xml"
                });
            }
            else if (accId == 3)
            {
                requestUriString = string.Concat(new string[]
                {
                    "http://bankalerts.sinfini.com/api/web2sms.php?workingkey=Ad54e7d5d3d20a7fb840f358a2e959a91&sender=MAFILD&to= ",
                    Convert.ToString(pNumber),
                    " &message=",
                    HttpUtility.UrlEncode(msg),
                    "&type=xml"
                });
            }
            else if (accId == 4)
            {
                requestUriString = string.Concat(new string[]
                {
                    "http://bankalerts.sinfini.com/api/web2sms.php?workingkey=A98121438b7df1bd359b083edcfedffaa&sender=MAFLHR&to= ",
                    Convert.ToString(pNumber),
                    " &message=",
                    HttpUtility.UrlEncode(msg),
                    "&type=xml"
                });
            }

            WebRequest webRequest = WebRequest.Create(requestUriString);
            Stream responseStream = webRequest.GetResponse().GetResponseStream();
            StreamReader streamReader = new StreamReader(responseStream);
            string text = streamReader.ReadLine();

            if (text == null || text.Length == 0)
            {
                return text = "No Message";
            }
            else
            {
                return text;
            }
        }

        public string sendMail(string mailId, string subject, string message, string serverurl, int port, int timeout, string target, string sender, string password)
        {
            SmtpClient server = new SmtpClient(serverurl);
            server.Port = port;
            server.EnableSsl = true;
            server.UseDefaultCredentials = false;
            server.Credentials = new System.Net.NetworkCredential(sender, password, serverurl);
            server.Timeout = timeout;
            server.TargetName = "   ";
            server.DeliveryMethod = SmtpDeliveryMethod.Network;
            MailMessage mail = new MailMessage();
            mail.From = new MailAddress(sender);
            mail.To.Add(mailId);
            mail.To.Add(sender);
            mail.Subject = subject;
            mail.Body = message;
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            mail.IsBodyHtml = true;
            server.Send(mail);
            return "Mail Send";
        }
    }
}
