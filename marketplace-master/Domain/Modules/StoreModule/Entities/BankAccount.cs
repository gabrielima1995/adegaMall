using PaymentIntegrator.Core.Common;
using PaymentIntegrator.Core.Suppliers;
using System;

namespace Domain.Entities.StoreModule
{
    public class BankAccount : BaseEntity<Guid>, IBankAccount
    {
        public Bank Bank { get; set; }

        public AccountType AccountType { get; set; }

        public string Branch { get; set; }

        public string Number { get; private set; }

        public BankAccount()
        {
            Id = Guid.NewGuid();
        }

        public string GetBranchOnly()
        {
            if (!string.IsNullOrWhiteSpace(Branch))
            {
                if (Branch.Contains("-"))
                    return Branch.Split('-')[0];

                return Branch;
            }

            return null;
        }

        public string GetBranchDvOnly()
        {
            if (!string.IsNullOrWhiteSpace(Branch) && Branch.Contains("-"))
            {
                var split = Branch.Split('-');

                if (split.Length > 1)
                {
                    return split[1];
                }
            }

            return null;
        }

        public string GetNumberOnly()
        {
            if (!string.IsNullOrWhiteSpace(Number))
            {
                if (Number.Contains("-"))
                    return Number.Split('-')[0];

                return Number;
            }

            return null;
        }

        public string GetNumberDvOnly()
        {
            if (!string.IsNullOrWhiteSpace(Number) && Number.Contains("-"))
            {
                var split = Number.Split('-');

                if (split.Length > 1)
                {
                    return split[1];
                }
            }

            return null;
        }

        public void SetBranch(string branch, string branchDv)
        {
            if (!string.IsNullOrWhiteSpace(branch))
            {
                Branch = branch.Replace("-", "");

                if (!string.IsNullOrWhiteSpace(branchDv))
                {
                    Branch += "-" + branchDv.Replace("-", "");
                }
            }
        }

        public void SetNumber(string number, string numberDv)
        {
            if (!string.IsNullOrWhiteSpace(number))
            {
                Number = number.Replace("-", "");

                if (!string.IsNullOrWhiteSpace(numberDv))
                {
                    Number += "-" + numberDv.Replace("-", "");
                }
            }
        }
    }
}