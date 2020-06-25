using FluentValidation;
using FluentValidation.Results;
using System;

namespace Domain.Core
{
    public class InvalidEntityException : Exception
    {
        public InvalidEntityException(Type entityType, string validationErrors)
            : base($"Invalid entity of type '{entityType.FullName}'. Errors: {validationErrors}")
        {
        }

        public InvalidEntityException(Type entityType, ValidationResult validationResult)
            : this(entityType, validationResult.ErrorsForExceptionMessage())
        {
        }
    }
}
