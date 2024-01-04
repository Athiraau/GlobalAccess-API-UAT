using Business.Contracts;
using DataAccess.Contracts;
using DataAccess.Dto.Request;
using DataAccess.Entities;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Business.Services
{
    public class UserService: IUserService
    {
		private readonly IConfiguration _config;
		private readonly IRepositoryWrapper _repository;
        private string _Npassword = "";
        private string _Dpassword = "";

        public UserService(IConfiguration config, IRepositoryWrapper repository)
		{
			_config = config;
			_repository = repository;
		}

		public async Task<UserInfo> GetUserInfo(UserCredReqDto userCred)
		{
           // _Dpassword = FromHexString(userCred.password);  //hex to base64
           // _Npassword = DecryptText(_Dpassword);           //base64 to string
           // userCred.password = _Npassword;
            UserInfo user = await _repository.User.GetUserInfo(userCred);
			return user;
		}

		public TokenResponse AuthenticateUser(UserInfo user)
		{
			TokenResponse tokenRes = new TokenResponse();
			// generate token that is valid for 30 minutes
			var tokenHandler = new JwtSecurityTokenHandler();
			var key = Encoding.ASCII.GetBytes(_config["Jwt:Key"]);
			var tokenDescriptor = new SecurityTokenDescriptor
			{
				Subject = new ClaimsIdentity(new[] {
					new Claim(JwtRegisteredClaimNames.Sub, _config["Jwt:Issuer"]),
					new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
					new Claim(JwtRegisteredClaimNames.Iat, DateTime.UtcNow.ToString()),
					new Claim("empCode", user.empCode.ToString()),
					new Claim("empName", user.empName),
					new Claim("joinDt", user.joinDt)
				}),
				Expires = DateTime.UtcNow.AddMinutes(30),
				SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
			};
			var result = tokenHandler.CreateToken(tokenDescriptor);
			tokenRes.Token = new JwtSecurityTokenHandler().WriteToken(result);
			return tokenRes;
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


    }
}
