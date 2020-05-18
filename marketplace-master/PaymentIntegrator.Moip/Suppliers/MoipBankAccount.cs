using PaymentIntegrator.Core.Common;
using PaymentIntegrator.Core.Suppliers;
using PaymentIntegrator.Moip.Helpers;
using System.Text.RegularExpressions;

namespace PaymentIntegrator.Moip.Suppliers
{
    internal class MoipBankAccount
    {
        public static object CreateBankAccount(ISupplier supplier, IAccountable accountable, IBankAccount account)
        {
            string accountNumber = account.Number.Split('-')[0];
            string accountDigit = account.Number.Split('-')[1];
            string agencyNumber = account.Branch;
            string agencyDigit = null;

            var supplierDocDigits = Regex.Replace(supplier.DocumentNumber, "[^0-9]", string.Empty);
            bool isCompany = supplierDocDigits.Length == 14;

            if (account.Branch.Contains("-"))
            {
                agencyNumber = account.Branch.Split('-')[0];
                agencyDigit = account.Branch.Split('-')[1];
            }

            object obj;
            if (agencyDigit != null)
            {
                obj = new
                {
                    bankNumber = ((int)account.Bank).ToString(),
                    agencyNumber,
                    agencyCheckNumber = agencyDigit,
                    accountNumber,
                    accountCheckNumber = accountDigit,
                    type = account.AccountType == AccountType.ContaCorrente ? "CHECKING" : "SAVING",
                    holder = new
                    {
                        taxDocument = new
                        {
                            type = isCompany ? "CNPJ" : "CPF",
                            number = DocumentHelper.FormatDocument(isCompany ? supplier.DocumentNumber : accountable.DocumentNumber),
                        },
                        fullname = isCompany ? supplier.Name : accountable.Name
                    }
                };
            }
            else
            {
                obj = new
                {
                    bankNumber = ((int)account.Bank).ToString(),
                    agencyNumber,
                    accountNumber,
                    accountCheckNumber = accountDigit,
                    type = account.AccountType == AccountType.ContaCorrente ? "CHECKING" : "SAVING",
                    holder = new
                    {
                        taxDocument = new
                        {
                            type = isCompany ? "CNPJ" : "CPF",
                            number = DocumentHelper.FormatDocument(isCompany ? supplier.DocumentNumber : accountable.DocumentNumber),
                        },
                        fullname = isCompany ? supplier.Name : accountable.Name
                    }
                };
            }

            return obj;
        }

        public static object UpdateBankAccount(dynamic retrievedAccount, IBankAccount updatedAccount, IAccountable accountable)
        {
            string accountNumber = updatedAccount.Number.Split('-')[0];
            string accountDigit = updatedAccount.Number.Split('-')[1];
            string agencyNumber = updatedAccount.Branch;
            string agencyDigit = null;

            if (updatedAccount.Branch.Contains("-"))
            {
                agencyNumber = updatedAccount.Branch.Split('-')[0];
                agencyDigit = updatedAccount.Branch.Split('-')[1];
            }

            var obj = new
            {
                bankNumber = ((int)updatedAccount.Bank).ToString(),
                agencyNumber,
                agencyCheckNumber = agencyDigit,
                accountNumber,
                accountCheckNumber = accountDigit,
                type = updatedAccount.AccountType == AccountType.ContaCorrente ? "CHECKING" : "SAVING",
                holder = new
                {
                    taxDocument = new
                    {
                        type = accountable.DocumentNumber.Length == 11 ? "CPF" : "CNPJ",
                        number = DocumentHelper.FormatDocument(accountable.DocumentNumber),
                    },
                    fullname = accountable.Name,

                    /* NÃO ESTAVA ATUALIZANDO O DOCUMENTO E O NOME DO RESPONSÁVEL PELA CONTA BANCÁRIA
                    taxDocument = new
                    {
                        retrievedAccount.holder.taxDocument.type,
                        retrievedAccount.holder.taxDocument.number,
                    },
                    retrievedAccount.holder.fullname,
                    */
                }
            };


            return obj;
        }
    }
}
