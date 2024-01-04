using System;
using System.Collections.Generic;
using System.Text;

namespace DataAccess.Dto.Response
{
    public class GetRolesResDto
    {
        public int accessId { get; set; }
        public int roleId { get; set; }
        public int branchId { get; set; }
        public int flag { get; set; }
        public string isSuccess { get; set; }
        public string message { get; set; }
    }
}
