using DataAccess.Dto.Request;
using DataAccess.Dto.Response;

namespace DataAccess.Dto
{
    public class DtoWrapper
    {
        private CheckCredentialResDto _checkCredRes;
        private HostCheckReqDto _hostCheckReq;
        private UserCredReqDto _userCredReq;
        private ChangePasswordResDto _changePwdRes;
        private PwdChangeReqDto _pwdChangeReq;
        private GetRolesResDto _getRolesRes;
        private PwdResetReqDto _pwdResetReq;
        private EmpStatusReqDto _empStatusReq;
        private EmployeeStatusResDto _empStatusRes;
        private SMSSendReqDto _smsSendReq;
        private SMSSendResDto _smsSendRes;

        public CheckCredentialResDto checkCredRes
        {
            get
            {
                if (_checkCredRes == null)
                {
                    _checkCredRes = new CheckCredentialResDto();
                }
                return _checkCredRes;
            }
        }

        public HostCheckReqDto hostCheckReq
        {
            get
            {
                if (_hostCheckReq == null)
                {
                    _hostCheckReq = new HostCheckReqDto();
                }
                return _hostCheckReq;
            }
        }

        public UserCredReqDto userCredkReq
        {
            get
            {
                if (_userCredReq == null)
                {
                    _userCredReq = new UserCredReqDto();
                }
                return _userCredReq;
            }
        }

        public ChangePasswordResDto changePwdRes
        {
            get
            {
                if (_changePwdRes == null)
                {
                    _changePwdRes = new ChangePasswordResDto();
                }
                return _changePwdRes;
            }
        }

        public PwdChangeReqDto pwdChangeReq
        {
            get
            {
                if (_pwdChangeReq == null)
                {
                    _pwdChangeReq = new PwdChangeReqDto();
                }
                return _pwdChangeReq;
            }
        }

        public GetRolesResDto getRolesRes
        {
            get
            {
                if (_getRolesRes == null)
                {
                    _getRolesRes = new GetRolesResDto();
                }
                return _getRolesRes;
            }
        }

        public PwdResetReqDto pwdResetReq
        {
            get
            {
                if (_pwdResetReq == null)
                {
                    _pwdResetReq = new PwdResetReqDto();
                }
                return _pwdResetReq;
            }
        }

        public EmpStatusReqDto EmpStatusReq
        {
            get
            {
                if (_empStatusReq == null)
                {
                    _empStatusReq = new EmpStatusReqDto();
                }
                return _empStatusReq;
            }
        }

        public EmployeeStatusResDto EmpStatusRes
        {
            get
            {
                if (_empStatusRes == null)
                {
                    _empStatusRes = new EmployeeStatusResDto();
                }
                return _empStatusRes;
            }
        }

        public SMSSendReqDto SmsSendReq
        {
            get
            {
                if (_smsSendReq == null)
                {
                    _smsSendReq = new SMSSendReqDto();
                }
                return _smsSendReq;
            }
        }

        public SMSSendResDto SmsSendRes
        {
            get
            {
                if (_smsSendRes == null)
                {
                    _smsSendRes = new SMSSendResDto();
                }
                return _smsSendRes;
            }
        }
    }
}
