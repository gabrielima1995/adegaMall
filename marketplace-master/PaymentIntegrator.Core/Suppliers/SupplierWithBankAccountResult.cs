namespace PaymentIntegrator.Core.Suppliers
{
    public class SupplierWithBankAccountResult
    {
        private SupplierWithBankAccountResult(string supplierId, string bankAccountId, string apiToken, string extraInfoJson)
        {
            SupplierId = supplierId;
            BankAccountId = bankAccountId;
            ApiToken = apiToken;
            ExtraInfoJson = extraInfoJson;
        }

        public static SupplierWithBankAccountResult Pagarme(string supplierId, string bankAccountId, string extraInfoJson)
        {
            return new SupplierWithBankAccountResult(supplierId, bankAccountId, null, extraInfoJson);
        }

        public static SupplierWithBankAccountResult Wirecard(string supplierId, string bankAccountId, string apiToken, string extraInfoJson)
        {
            return new SupplierWithBankAccountResult(supplierId, bankAccountId, apiToken, extraInfoJson);
        }

        public string SupplierId { get; }
        public string BankAccountId { get; }
        public string ApiToken { get; }
        public string ExtraInfoJson { get; }
    }
}