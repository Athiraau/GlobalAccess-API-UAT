using DataAccess.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Business.Helpers
{
    public class ValidationHelper
    {
        private readonly ErrorResponse _error;

        public ValidationHelper(ErrorResponse error)
        {
            _error = error;
        }

        public ErrorResponse ReturnErrorRes(FluentValidation.Results.ValidationResult Res)
        {
            List<string> errors = new List<string>();
            foreach (var row in Res.Errors.ToArray())
            {
                errors.Add(row.ErrorMessage.ToString());
            }
            _error.errorMessage = errors;
            return _error;
        }
    }
}
