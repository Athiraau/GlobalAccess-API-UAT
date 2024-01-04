using System;
using System.Collections.Generic;
using System.Text;

namespace DataAccess.Dto.Response
{
    public class EmployeeStatusResDto
    {
        public int emp_code { get; set; }
        public string emp_name { get; set; }
        public string status { get; set; }
        public int branch_id { get; set; }
    }
}
