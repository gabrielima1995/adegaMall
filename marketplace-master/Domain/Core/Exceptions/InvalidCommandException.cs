using FluentValidation;
using FluentValidation.Results;
using System;

namespace Domain.Core
{
    public class InvalidCommandException : Exception
    {
        public InvalidCommandException(Type commandType, string validationErrors)
            : base($"Invalid command of type '{commandType.FullName}'. Errors: {validationErrors}")
        {
        }

        public InvalidCommandException(Type entityType, ValidationResult validationResult)
            : this(entityType, validationResult.ErrorsForExceptionMessage())
        {
        }
    }
}
