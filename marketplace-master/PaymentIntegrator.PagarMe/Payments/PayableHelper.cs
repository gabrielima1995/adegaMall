using Framework;
using Newtonsoft.Json;
using PaymentIntegrator.Core.Payments;
using PaymentIntegrator.PagarMe.SimpleImpl;
using System;

namespace PaymentIntegrator.PagarMe.Payments
{
    internal class PayableHelper
    {
        public OperationResult<PayableDto> GetPayableDetails(string transactionId, int installment)
        {
            var client = new Client();
            var response = client.Execute(new GetPayableDetailsRequest(transactionId, installment));
            if (response.StatusCode != 200)
                return OperationResult<PayableDto>.Fail(response.StatusCode.ToString(),response.Content);
            return Map(response.Content);
        }

        private PayableDto Map(string content)
        {
            dynamic json = JsonConvert.DeserializeObject(content);
            return new PayableDto
            {
                Amount = int.Parse((string)json[1]["amount"]),
                AnticipationFee = int.Parse((string)json[1]["anticipation_fee"]),
                BulkAnticipationId = (string)json[1]["bulk_anticipation_id"],
                Installment = int.Parse((string)json[1]["installment"]),
                PayableType = (string)json[1]["type"],

                OriginalPaymentDate = (string)json[1]["original_payment_date"] != null
                                      ? DateTime.Parse((string)json[1]["original_payment_date"])
                                      : DateTime.UtcNow,
                PaymentDate = (string)json[1]["original_payment_date"] != null
                              ? DateTime.Parse((string)json[1]["payment_date"])
                              : DateTime.UtcNow,

                PaymentMethod = (string)json[1]["payment_method"],
                RecipientId = (string)json[1]["recipient_id"],
                SplitRuleId = (string)json[1]["split_rule_id"],
                TransactionId = int.Parse((string)json[1]["transaction_id"]),
                PayableId = (string)json[1]["id"],
                Status = (string)json[1]["status"],
                Fee = int.Parse((string)json[1]["fee"])
            };
        }
    }
}