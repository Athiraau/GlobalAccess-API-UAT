using DataAccess.Dto.Request;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GlobalAccess.Validators
{
    public class EmployeeMobileValidator : AbstractValidator<EmpStatusReqDto>
    {
        public EmployeeMobileValidator()
        {
            RuleFor(d => d.mobile).NotNull().GreaterThan(0).WithMessage("EmpID must be greater than 0");
            RuleFor(d => d.mobile).Must(MobileLength).WithMessage("Mobile number length should be 10 (without country code)");
        }

        private bool MobileLength(long mobile)
        {
            if (Convert.ToString(mobile).Length != 10)
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
