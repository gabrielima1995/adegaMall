using System;

namespace BatchImport
{
    public class InvalidValueForFieldException : Exception
    {
        public InvalidValueForFieldException(object value, IField field)
            : base($"Value '{value}' is not valid for field '{field.Name}' of type '{field.GetType()}'.")
        {
        }
    }
}
