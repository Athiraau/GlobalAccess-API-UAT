using DataAccess.Dto.Request;
using FluentValidation;
using System;

namespace GlobalAccess.Validators
{
    public class PwdChangeValidator : AbstractValidator<PwdChangeReqDto>
    {
        public PwdChangeValidator()
        {
            RuleFor(d => d.empCode).NotNull().GreaterThan(0).WithMessage("EmpID must be greater than 0");
            RuleFor(d => d.password).NotNull().NotEmpty().WithMessage("Password is required");
            RuleFor(d => d.oldPassword).NotNull().NotEmpty().WithMessage("Old password is required");
            RuleFor(d => d.empCode).Must(EmpcodeLength).WithMessage("Employee Code Length must be equal or less than 6");
        }

        private bool EmpcodeLength(int empCode)
        {
            if (Convert.ToString(empCode).Length > 6)
            {
                return false;
            }
            else
            {
                return true;
            }
        }
    }
}
