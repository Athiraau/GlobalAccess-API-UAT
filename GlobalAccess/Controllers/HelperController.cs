using Microsoft.AspNetCore.Mvc;
using DataAccess.Contracts;
using DataAccess.Dto;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Cors;
using Business.Contracts;
using FluentValidation;
using DataAccess.Dto.Request;
using Business.Services;
using Microsoft.AspNetCore.Authorization;

namespace GlobalAccess.Controllers
{
    //[AllowAnonymous]
    [Authorize]
    [Route("api/helper")]
    [EnableCors("MyPolicy")]
    [ApiController]
    public class HelperController : ControllerBase
    {
        private readonly ILoggerService _logger;
        private readonly IServiceWrapper _service;
        private readonly DtoWrapper _dto;
        private readonly ServiceHelper _helper;
        private readonly IValidator<HostCheckReqDto> _hcValidator;
        private readonly IValidator<UserCredReqDto> _ucValidator;
        private readonly IValidator<PwdChangeReqDto> _pwdcValidator;
        private readonly IValidator<EmpStatusReqDto> _empSValidator;
        private readonly IValidator<SMSSendReqDto> _smsSendValidator;
        private readonly IValidator<MailSendReqDto> _mailSendValidator;

        public HelperController(ILoggerService logger, IServiceWrapper service, DtoWrapper dto, IValidator<HostCheckReqDto> hcValidator,
            IValidator<UserCredReqDto> ucValidator, ServiceHelper helper, IValidator<PwdChangeReqDto> pwdcValidator, 
            IValidator<EmpStatusReqDto> empSValidator, IValidator<SMSSendReqDto> smsSendValidator, IValidator<MailSendReqDto> mailSendValidator)
        {
            _logger = logger;
            _service = service;
            _dto = dto;
            _helper = helper;
            _hcValidator = hcValidator;
            _ucValidator = ucValidator;
            _pwdcValidator = pwdcValidator;
            _empSValidator = empSValidator;
            _smsSendValidator = smsSendValidator;
        }
        [AllowAnonymous]
        [HttpGet("CheckCredentials/{empCode}/{password}", Name = "CheckCredentials")]
        public async Task<IActionResult> CheckEmployeeCredentials([FromRoute] int empCode, string password)
        {
            _dto.userCredkReq.empCode = empCode;
            _dto.userCredkReq.password = password;
            var validationResult = await _ucValidator.ValidateAsync(_dto.userCredkReq);
            var errorRes = _helper.VHelper.ReturnErrorRes(validationResult);

            if (errorRes.errorMessage.Count > 0)
            {
                _logger.LogError("Invalid user credentials sent from client.");
                return BadRequest(errorRes);
            }

            var employeeCnt = await _service.Helper.CheckEmployeeCredentials(empCode, password);
            if (employeeCnt == null)
            {
                _logger.LogError($"Employee with employee code: {empCode}, hasn't been found in db.");
                return NotFound();
            }
            else
            {
                _logger.LogInfo($"Returned employee with empoloyee code: {empCode}");
                return Ok(JsonConvert.SerializeObject(employeeCnt));
            }
        }

        [AllowAnonymous]
        [HttpGet("HostNameCheck/{hostname}", Name = "HostNameCheck")]
        public async Task<IActionResult> HostnameCheck([FromRoute] string hostname)
        {
            _dto.hostCheckReq.hostname = hostname;
            var validationResult = await _hcValidator.ValidateAsync(_dto.hostCheckReq);
            var errorRes = _helper.VHelper.ReturnErrorRes(validationResult);

            if (errorRes.errorMessage.Count > 0)
            {
                _logger.LogError("Invalid host name sent from client.");
                return BadRequest(errorRes);
            }

            var employeeCnt = await _service.Helper.HostnameCheck(hostname);
            if (employeeCnt == null)
            {
                _logger.LogError($"Details with hostname: {hostname}, hasn't been found in db.");
                return NotFound();
            }
            else
            {
                _logger.LogInfo($"Returned details with hostname: {hostname}");
                return Ok(JsonConvert.SerializeObject(employeeCnt));
            }
        }

        [AllowAnonymous]
        [HttpPost("ResetPassword", Name = "ResetPassword")]
        public async Task<IActionResult> ResetPassword([FromBody] PwdResetReqDto pwdReq)
        {
            _dto.userCredkReq.empCode = pwdReq.empCode;
            _dto.userCredkReq.password = pwdReq.ifStaff;
            var validationResult = await _ucValidator.ValidateAsync(_dto.userCredkReq);
            var errorRes = _helper.VHelper.ReturnErrorRes(validationResult);

            if (errorRes.errorMessage.Count > 0)
            {
                _logger.LogError("Invalid input data sent from client.");
                return BadRequest(errorRes);
            }

            var msgFlag = await _service.Helper.ResetPassword(pwdReq);
            if (msgFlag == null)
            {
                _logger.LogError($"Password reset failed for employee with employee code: {pwdReq.empCode}.");
                return NotFound();
            }
            else
            {
                _logger.LogInfo($"Password reset complete for employee with empoloyee code: {pwdReq.empCode}");
                return Ok(JsonConvert.SerializeObject(msgFlag));
            }
        }

        [AllowAnonymous]
        [HttpPost("ChangePassword", Name = "ChangePassword")]
        public async Task<IActionResult> ChangePassword([FromBody] PwdChangeReqDto pwdReq)
        {
            var validationResult = await _pwdcValidator.ValidateAsync(pwdReq);
            var errorRes = _helper.VHelper.ReturnErrorRes(validationResult);

            if (errorRes.errorMessage.Count > 0)
            {
                _logger.LogError("Invalid input data sent from client.");
                return BadRequest(errorRes);
            }

            var msgFlag = await _service.Helper.ChangePassword(pwdReq);
            if (msgFlag == null)
            {
                _logger.LogError($"Password reset failed for employee with employee code: {pwdReq.empCode}.");
                return NotFound();
            }
            else
            {
                _logger.LogInfo($"Password reset complete for employee with empoloyee code: {pwdReq.empCode}");
                return Ok(JsonConvert.SerializeObject(msgFlag));
            }
        }

        /* [HttpGet("GetRoles/{empCode}/{password}", Name = "GetRoles")]
         public async Task<IActionResult> GetRoles([FromRoute] int empCode, string password)
         {
             _dto.userCredkReq.empCode = empCode;
             _dto.userCredkReq.password = password;
             var validationResult = await _ucValidator.ValidateAsync(_dto.userCredkReq);
             var errorRes = _helper.VHelper.ReturnErrorRes(validationResult);

             if (errorRes.errorMessage.Count > 0)
             {
                 _logger.LogError("Invalid user credentials sent from client.");
                 return BadRequest(errorRes);
             }

             var employeeCnt = await _service.Helper.GetRoles(empCode, password);
             if (employeeCnt == null)
             {
                 _logger.LogError($"Employee with employee code: {empCode}, hasn't been found in db.");
                 return NotFound();
             }
             else
             {
                 _logger.LogInfo($"Returned employee with empoloyee code: {empCode}");
                 return Ok(JsonConvert.SerializeObject(employeeCnt));
             }
         }
        */
        [AllowAnonymous]
        [HttpGet("GetEmployeeStatus/{mobile}", Name = "GetEmployeeStatus")]
        public async Task<IActionResult> GetEmployeeStatus([FromRoute] Int64 mobile)
        {
            _dto.EmpStatusReq.mobile = mobile;
            var validationResult = await _empSValidator.ValidateAsync(_dto.EmpStatusReq);
            var errorRes = _helper.VHelper.ReturnErrorRes(validationResult);

            if (errorRes.errorMessage.Count > 0)
            {
                _logger.LogError("Invalid mobile number sent from client.");
                return BadRequest(errorRes);
            }

            var empStatus = await _service.Helper.GetEmployeeStatus(mobile);
            if (empStatus == null)
            {
                _logger.LogError($"Details of employee with mobile: {mobile}, hasn't been found in db.");
                return NotFound();
            }
            else
            {
                _logger.LogInfo($"Returned details of employee with mobile: {mobile}");
                return Ok(JsonConvert.SerializeObject(empStatus));
            }
        }

        [AllowAnonymous]
        [HttpPost("SendSMS", Name = "SendSMS")]
        public async Task<IActionResult> SendSMS([FromBody] SMSSendReqDto smsReq)
        {
            var validationResult = await _smsSendValidator.ValidateAsync(smsReq);
            var errorRes = _helper.VHelper.ReturnErrorRes(validationResult);

            if (errorRes.errorMessage.Count > 0)
            {
                _logger.LogError("Invalid details sent from client.");
                return BadRequest(errorRes);
            }

            var smsStatus = await _service.Helper.SendSMS(smsReq);
            if (smsStatus == null)
            {
                _logger.LogError($"Details of SMS sent to employee hasn't been found in db.");
                return NotFound();
            }
            else
            {
                _logger.LogInfo($"Returned details of SMS sent to employee.");
                return Ok(JsonConvert.SerializeObject(smsStatus));
            }
        }
        
        [AllowAnonymous]
        [HttpPost("SendMail", Name = "SendMail")]
        public async Task<IActionResult> SendMail([FromBody] MailSendReqDto mailReq)
        {
           
            //var validationResult = await _mailSendValidator.ValidateAsync(mailReq);
            //var errorRes = _helper.VHelper.ReturnErrorRes(validationResult);

            //if (errorRes.errorMessage.Count > 0)
            //{
            //    _logger.LogError("Invalid details sent from client.");
            //    return BadRequest(errorRes);
            //}

            var mailStatus = await _service.Helper.SendMail(mailReq);
            if (mailStatus == null)
            {
                _logger.LogError($"Details of Mail sent to employee hasn't been found.");
                return NotFound();
            }
            else
            {
                _logger.LogInfo($"Returned details of mail sent to customer.");
                return Ok(JsonConvert.SerializeObject(mailStatus));
            }
        }

        [AllowAnonymous]
        [HttpPost("GetRoles", Name = "GetRoles")]
        public async Task<IActionResult> GetRoles([FromBody] UserCredReqDto credReq)
        {
             _dto.userCredkReq.empCode = credReq.empCode;
             _dto.userCredkReq.password = credReq.password;
            int empCode = _dto.userCredkReq.empCode;
            string password = _dto.userCredkReq.password;


            var validationResult = await _ucValidator.ValidateAsync(_dto.userCredkReq);
            var errorRes = _helper.VHelper.ReturnErrorRes(validationResult);

            if (errorRes.errorMessage.Count > 0)
            {
                _logger.LogError("Invalid user credentials sent from client.");
                return BadRequest(errorRes);
            }

            var employeeCnt = await _service.Helper.GetRoles(empCode, password);
            if (employeeCnt == null)
            {
                _logger.LogError($"Employee with employee code: {empCode}, hasn't been found in db.");
                return NotFound();
            }
            else
            {
                _logger.LogInfo($"Returned employee with empoloyee code: {empCode}");
                return Ok(JsonConvert.SerializeObject(employeeCnt));
            }
        }



    }
}
