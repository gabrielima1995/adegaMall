using Framework;
using PagarMe;
using PaymentIntegrator.Core.Suppliers;
using PaymentIntegrator.PagarMe.Common;
using System;

namespace PaymentIntegrator.PagarMe.Suppliers
{
    /// <summary>
    /// Representa um recebedor para salvar no Pagar.me.
    /// </summary>
    public class RecipientRequest : Recipient, IValidatable
    {
        public RecipientRequest(PagarMeClient client, ISupplier supplier, BankAccountRequest bankAccount)
            : base(client.GetService())
        {
            Id = supplier.GatewayId;

            var isNew = string.IsNullOrEmpty(Id);

            if (isNew)
            {
                TransferInterval = (TransferInterval)Enum.Parse(typeof(TransferInterval), PagarMeConfig.RecipientTransferInterval, true);
                TransferDay = PagarMeConfig.RecipientTransferDay;
                TransferEnabled = PagarMeConfig.RecipientTransferEnabled;
            }

            BankAccount = bankAccount;
        }

        public ErrorDictionary Validate()
        {
            return new ErrorDictionary();
        }
    }
}