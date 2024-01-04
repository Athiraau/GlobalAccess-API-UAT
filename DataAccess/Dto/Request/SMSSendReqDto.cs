using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Dto.Request
{
    public class SMSSendReqDto
    {
        public int accId { get;set; } = 0;
        public string pNumber { get;set; } = string.Empty;
        public string message { get;set; } = string.Empty;  
    }
}
