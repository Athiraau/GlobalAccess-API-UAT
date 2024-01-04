using DataAccess.Dto.Request;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GlobalAccess.Validators
{
    public class MailSendValidator : AbstractValidator<MailSendReqDto>
    {
        public MailSendValidator()
        {
            RuleFor(d => d.mailId).NotEmpty().WithMessage("MailId is required");
            RuleFor(d => d.subject).NotEmpty().WithMessage("Mail subject is required");
            RuleFor(d => d.message).NotEmpty().WithMessage("Mail message is required");

        }       

    }
    
}
