using Framework;
using Newtonsoft.Json;
using PaymentIntegrator.Core.Common;
using PaymentIntegrator.Core.Customers;
using PaymentIntegrator.Moip.Common;
using System;

namespace PaymentIntegrator.Moip.Customers
{
    public class MoipCustomerOperations : ICustomerOperations
    {
        public OperationResult Create(ICustomer customer, IAddress address)
        {
            if (customer == null || address == null)
                return OperationResult.Fail("Invalid address or customer information");

            var request = new MoipRequest("customers", RestSharp.Method.POST);

            request.AddJsonBody(MoipCustomer.Create(customer, address));

            var response = MoipClient.SendRequest(request);

            if (response.StatusCode == System.Net.HttpStatusCode.Created || response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                try
                {
                    dynamic obj = JsonConvert.DeserializeObject(response.Content);

                    customer.SetGatewayId((string)obj.id);
                    return OperationResult.Success;
                }
                catch
                {
                    return OperationResult.Fail("Error parsing Moip response");
                }
            }
            else if (response.StatusCode == System.Net.HttpStatusCode.BadRequest)
            {
                try
                {
                    var errorResponse = JsonConvert.DeserializeObject<MoipErrorResponse>(response.Content);

                    return errorResponse.Result;
                }
                catch
                {
                    return OperationResult.Fail("Error parsing Moip error response");
                }
            }
            else
            {
                return OperationResult.Fail($"Error creating Moip Customer - HTTP {(int)response.StatusCode}");
            }
        }

        public OperationResult Update(ICustomer customer, IAddress address)
        {
            throw new NotImplementedException();
        }
    }
}
