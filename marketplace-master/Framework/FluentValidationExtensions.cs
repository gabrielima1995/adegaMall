using FluentValidation.Results;
using System;
using System.Linq;

namespace FluentValidation
{
    public static class FluentValidationExtensions
    {
        public static string ErrorsForExceptionMessage(this ValidationResult result)
        {
            if (result.IsValid)
                return "";

            return string.Join(
                Environment.NewLine,
                result.Errors.Select(x => x.ErrorMessage ?? x.ErrorCode));
        }
    }
}
