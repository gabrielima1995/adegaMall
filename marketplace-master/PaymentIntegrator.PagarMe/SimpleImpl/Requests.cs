using System;
using System.Collections.Generic;

namespace PaymentIntegrator.PagarMe.SimpleImpl
{
    public enum RequestMethod
    {
        GET,
        POST,
        PUT,
        DELETE
    }

    public abstract class Request
    {
        public abstract string Endpoint { get; }
        public abstract RequestMethod Method { get; }
        public virtual List<Tuple<string, string>> Parameters { get; set; } = new List<Tuple<string, string>>();
        public virtual string Body { get; set; }
    }

    public class GetTransactionRequest : Request
    {
        public override string Endpoint => $"/transactions/{Id}";
        public override RequestMethod Method => RequestMethod.GET;
        public string Id { get; set; }
        public GetTransactionRequest(string id)
        {
            Id = id;
        }
    }

    public class ListPlansRequest : Request
    {
        private readonly string endpoint;

        public override string Endpoint => endpoint;
        public override RequestMethod Method => RequestMethod.GET;

        public ListPlansRequest(int page, int pageSize)
        {
            endpoint = $"/plans?page={page}&count={pageSize}";
        }
    }

    public class GetPayableDetailsRequest : Request
    {
        public override string Endpoint { get; }
        public override RequestMethod Method => RequestMethod.GET;

        public GetPayableDetailsRequest(string transactionId, int installment)
        {
            Endpoint = $"/payables/?transaction_id={transactionId}&installment={installment}";
        }
    }
}
