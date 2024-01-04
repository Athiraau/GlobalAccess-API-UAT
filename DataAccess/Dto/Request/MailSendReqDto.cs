using System;
using System.Collections.Generic;
using System.Text;

namespace DataAccess.Dto.Request
{
    public class MailSendReqDto
    {
        public string mailId { get; set; } = string.Empty;
        public string message { get; set; } = string.Empty;
        public string subject { get; set; } = string.Empty;

    }
}
