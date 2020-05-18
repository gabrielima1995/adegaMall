using Framework;

namespace Presentation.Models
{
    public class ApiResultEnvelope
    {
        private ApiResultEnvelope(object data, string errorMessage)
        {
            Data = data;
            ErrorMessage = errorMessage;
            IsSuccess = errorMessage == null;
        }

        public static ApiResultEnvelope Success(object data = null)
        {
            return new ApiResultEnvelope(data, null);
        }

        public static ApiResultEnvelope Error(string errorMessage)
        {
            return new ApiResultEnvelope(null, errorMessage ?? "Erro desconhecido");
        }

        public static ApiResultEnvelope From(OperationResult result)
        {
            return result.IsFailure ? Error(result.Errors.ToSimpleString()) : Success();
        }

        public static ApiResultEnvelope From<T>(OperationResult<T> result)
        {
            return result.IsFailure ? Error(result.Errors.ToSimpleString()) : Success(result.Result);
        }

        public object Data { get; }
        public string ErrorMessage { get; }
        public bool IsSuccess { get; }
        public bool IsFailure => !IsSuccess;
    }
}