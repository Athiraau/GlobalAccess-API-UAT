using Dapper;
using Dapper.Oracle;
using DataAccess.Context;
using Microsoft.Extensions.Configuration;
using DataAccess.Contracts;
using DataAccess.Entities;
using System.Data;
using System.Security.Claims;
using System.Text;
using DataAccess.Dto.Request;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;

namespace DataAccess.Repository
{
    public class UserRepository : IUserRepository
    {
		private readonly DapperContext _context;
        private readonly IConfiguration _config;

		public UserRepository(DapperContext context, IConfiguration config)
		{
			_context = context;
            _config = config;
		}

		public async Task<UserInfo> GetUserInfo(UserCredReqDto userCred)
		{
			var query = "select emp_code empCode, emp_name empName, join_dt joinDt from employee_master where emp_code = :empCode and password = :password";

			var parameters = new OracleDynamicParameters();
			parameters.Add("empCode", userCred.empCode, OracleMappingType.Int32, ParameterDirection.Input);
			parameters.Add("password", userCred.password, OracleMappingType.NVarchar2, ParameterDirection.Input);

            using var connection = _context.CreateConnection();
            var userInfo = await connection.QuerySingleOrDefaultAsync<UserInfo>(query, parameters);
            return userInfo;
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
					new Claim(JwtRegisteredClaimNames.Sub, _config["Jwt:Subject"]),
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
	}
}
