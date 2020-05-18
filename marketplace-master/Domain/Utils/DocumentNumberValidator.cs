using Application.Config;
using Application.Config.Registers;
using System.Text.RegularExpressions;

namespace Domain.Utils
{
    public class DocumentNumberValidator
    {
        public static bool IsValidForBuyer(string documentNumber)
            => IsValid(MaaSConfig.Current.Buyer.DocumentNumber, documentNumber, DocumentNumberSource.MainDocument);

        public static bool IsValidForSeller(string documentNumber, DocumentNumberSource source = DocumentNumberSource.MainDocument)
            => IsValid(MaaSConfig.Current.Seller.DocumentNumber, documentNumber, source);

        private static bool IsValid(DocumentNumberConfig config, string documentNumber, DocumentNumberSource source = DocumentNumberSource.MainDocument)
        {
            if ((config.Mandatory || config.IsRequired) && string.IsNullOrEmpty(documentNumber))
                return false;

            if (config.AllowFreeText)
                return true;

            var result = false;

            if (config.AllowCPF || source == DocumentNumberSource.AccountableDocument)
                result |= IsValidCpf(documentNumber);

            if (config.AllowCNPJ || source == DocumentNumberSource.AccountableDocument)
                result |= IsValidCnpj(documentNumber);

            return result;
        }

        private static bool IsValidCpf(string document)
        {
            var format = new Regex(@"^[0-9]{3}[\.]?[0-9]{3}[\.]?[0-9]{3}[-]?[0-9]{2}$");

            if (format.IsMatch(document))
                return IsCpf(document);

            return false;
        }

        private static bool IsCpf(string cpf)
        {
            int[] multipliers1 = new int[9] { 10, 9, 8, 7, 6, 5, 4, 3, 2 };
            int[] multipliers2 = new int[10] { 11, 10, 9, 8, 7, 6, 5, 4, 3, 2 };

            cpf = cpf.Trim();
            cpf = cpf.Replace(".", "").Replace("-", "");

            if (cpf.Length != 11)
                return false;

            string tempCpf = cpf.Substring(0, 9);

            int sum = 0;

            for (int i = 0; i < 9; i++)
                sum += int.Parse(tempCpf[i].ToString()) * multipliers1[i];

            int remainder = sum % 11;

            if (remainder < 2)
                remainder = 0;
            else
                remainder = 11 - remainder;

            string digit = remainder.ToString();
            tempCpf = tempCpf + digit;
            sum = 0;

            for (int i = 0; i < 10; i++)
                sum += int.Parse(tempCpf[i].ToString()) * multipliers2[i];

            remainder = sum % 11;

            if (remainder < 2)
                remainder = 0;
            else
                remainder = 11 - remainder;

            digit = digit + remainder.ToString();

            return cpf.EndsWith(digit);
        }

        private static bool IsValidCnpj(string document)
        {
            var format = new Regex(@"[0-9]{2}\.?[0-9]{3}\.?[0-9]{3}\/?[0-9]{4}\-?[0-9]{2}");

            if (format.IsMatch(document))
                return IsCnpj(document);

            return false;
        }

        private static bool IsCnpj(string cnpj)
        {
            int[] multiplier1 = new int[12] { 5, 4, 3, 2, 9, 8, 7, 6, 5, 4, 3, 2 };
            int[] multiplier2 = new int[13] { 6, 5, 4, 3, 2, 9, 8, 7, 6, 5, 4, 3, 2 };

            cnpj = cnpj.Trim();
            cnpj = cnpj.Replace(".", "").Replace("-", "").Replace("/", "");
            if (cnpj.Length != 14)
                return false;

            string tempCnpj = cnpj.Substring(0, 12);
            int sum = 0;

            for (int i = 0; i < 12; i++)
                sum += int.Parse(tempCnpj[i].ToString()) * multiplier1[i];

            int remainder = (sum % 11);
            if (remainder < 2)
                remainder = 0;
            else
                remainder = 11 - remainder;

            string digit = remainder.ToString();
            tempCnpj = tempCnpj + digit;
            sum = 0;

            for (int i = 0; i < 13; i++)
                sum += int.Parse(tempCnpj[i].ToString()) * multiplier2[i];

            remainder = (sum % 11);

            if (remainder < 2)
                remainder = 0;
            else
                remainder = 11 - remainder;

            digit = digit + remainder.ToString();
            return cnpj.EndsWith(digit);
        }
    }

    public enum DocumentNumberSource
    {
        MainDocument,
        AccountableDocument
    }
}
