using Framework;
using System;

namespace Domain.Core.Exceptions
{
    public class OperationResultException : Exception
    {
        public OperationResultException(OperationResult operationResult)
            : base(operationResult.Errors.ToSimpleString())
        {
        }
    }
}
