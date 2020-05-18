using System;
using System.Runtime.Serialization;

namespace Domain.Core
{
    public static class Contract
    {
        public static void Ensure(bool condition, string errorMessage = null)
        {
            if (!condition)
                throw new ContractException(errorMessage ?? "A contract has been violated.");
        }
    }

    public class ContractException : Exception
    {
        public ContractException()
        {
        }

        public ContractException(string message) : base(message)
        {
        }

        public ContractException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected ContractException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
