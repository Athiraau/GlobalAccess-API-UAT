using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Threading.Tasks;
using DataAccess.Contracts;
using DataAccess.Entities;
using Business.Services;
using DataAccess.Dto.Request;
using FluentValidation;
using Business.Contracts;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Authorization;

namespace Authentication.Controllers
{
    [AllowAnonymous]
    [Route("api/authentication")]
    [EnableCors("MyPolicy")]
    [ApiController]
    public class TokenController : ControllerBase
    {
        private readonly ILoggerService _logger;
        private readonly IServiceWrapper _service;
        private readonly ServiceHelper _helper;
        private readonly IValidator<UserCredReqDto> _ucValidator;
        private readonly IWebHostEnvironment _env;

        public TokenController(ILoggerService logger, IServiceWrapper service,
            ServiceHelper helper, IValidator<UserCredReqDto> ucValidator, IWebHostEnvironment env)
        {
            _logger = logger;
            _service = service;
            _helper = helper;
            _ucValidator = ucValidator;
            _env = env;
        }

        [HttpPost("AuthenticateUser")]
        public async Task<IActionResult> AuthenticateUser([FromBody] UserCredReqDto userData)
        {

           // _Dpassword = FromHexString(password);
            // _Npassword = DecryptText(_Dpassword);
            //var pw = userData.password;

            var _Dpassword = _service.Helper.FromHexString(userData.password);
            var _Npassword = _service.Helper.DecryptText(_Dpassword);

            userData.password = _Npassword;

            var validationResult = await _ucValidator.ValidateAsync(userData);
            var errorRes = _helper.VHelper.ReturnErrorRes(validationResult);

            if (errorRes.errorMessage.Count > 0)
            {
                _logger.LogError("Invalid user credentials sent from client.");
                return BadRequest(errorRes);
            }
            else
            {
                if (!_env.IsDevelopment())
                {

                    userData.password = _helper.PHelper.RemoveSpecialCharacters(_helper.PHelper.Encrypt(Convert.ToString(userData.empCode), userData.password));
                }

                if (userData != null && userData.password != null && userData.empCode >= 0)
                {
                    var userInfo = await GetUser(userData);

                    if (userInfo != null)
                    {
                        var token = _service.User.AuthenticateUser(userInfo);
                        _logger.LogInfo($"Token generated successfully for empCode: {userData.empCode}.");
                        return Ok(JsonConvert.SerializeObject(token));
                    }
                    else
                    {
                        _logger.LogError($"Invalid credentials. User with id: {userData.empCode} hasn't been found in db.");
                        return BadRequest("Invalid credentials");
                    }
                }
                else
                {
                    return BadRequest();
                }
            }
        }

        private async Task<UserInfo> GetUser(UserCredReqDto userCred)
        {
            return await _service.User.GetUserInfo(userCred);
        }
    }
}