namespace PaymentIntegrator.PagarMe.SimpleImpl
{
    public class Response
    {
        public int StatusCode { get; private set; }
        public string Content { get; private set; }
        public Response(int statusCode, string data = null)
        {
            StatusCode = statusCode;
            Content = data;
        }
    }
}
