using System;
using System.Collections.Generic;
using System.Text;

namespace DataAccess.Dto.Request
{
    public class PwdChangeReqDto
    {
        public int empCode { get; set; }
        public string password { get; set; }
        public string oldPassword { get; set; }
    }
}
