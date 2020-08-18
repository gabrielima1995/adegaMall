namespace Framework.ApiProviders
{
    public class ApiErrorMessage
    {
        public string Message { get; }

        public ApiErrorMessage(string message)
        {
            Message = message;
        }
    }
}