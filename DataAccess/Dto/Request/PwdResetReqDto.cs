using System;
using System.Collections.Generic;
using System.Text;

namespace DataAccess.Dto.Request
{
    public class PwdResetReqDto
    {
        public int empCode { get; set; }
        public string ifStaff { get; set; }
    }
}
