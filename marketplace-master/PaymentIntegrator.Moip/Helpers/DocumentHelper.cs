namespace PaymentIntegrator.Moip.Helpers
{
    public static class DocumentHelper
    {
        public static string FormatDocument(string docNumber)
        {
            string output = docNumber;

            if (docNumber.Length == 11) //CPF
            {
                output = output.Insert(9, "-");
                output = output.Insert(6, ".");
                output = output.Insert(3, ".");
            }
            else if (docNumber.Length == 14) //CNPJ
            {
                output = output.Insert(12, "-");
                output = output.Insert(8, "/");
                output = output.Insert(5, ".");
                output = output.Insert(2, ".");
            }

            return output;
        }
    }
}