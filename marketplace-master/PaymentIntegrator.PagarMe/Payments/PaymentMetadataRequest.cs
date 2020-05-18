using PagarMe;
using PagarMe.Base;
using PaymentIntegrator.Core.Payments;
using System.Collections.Generic;
using System.Linq;

namespace PaymentIntegrator.PagarMe.Payments
{
    /// <summary>
    /// Metadata do pagamento de pedido(s).
    /// </summary>
    public class PaymentMetadataRequest : AbstractModel
    {
        public PaymentMetadataRequest(PagarMeService service, IList<IOrder> orders)
            : base(service)
        {
            Orders = orders.Select(order => order.OrderId).ToArray();
            if (orders.Any(order => order.Supplier != null))
            {
                Sellers = orders.Select(order => order.Supplier.GatewayId).ToArray();
                SellerNames = orders.Select(order => order.Supplier.Name).ToArray();
                SellerAltNames = orders.Select(order => order.Supplier.AlternativeName).ToArray();
                SellerDocs = orders.Select(order => order.Supplier.DocumentNumber).ToArray();
                SellerEmails = orders.Select(order => order.Supplier.Email).ToArray();
                SplitRules = orders.GenerateSplitRulesOrNull()
                    .Select(rule => $"{rule.Recipient.Id}: {rule.Amount} cents").ToArray();
            }
        }

        public string[] Orders
        {
            get { return GetAttribute<string[]>("orders"); }
            set { SetAttribute("orders", value); }
        }

        public string[] Sellers
        {
            get { return GetAttribute<string[]>("sellers"); }
            set { SetAttribute("sellers", value); }
        }

        public string[] SellerNames
        {
            get { return GetAttribute<string[]>("sellerNames"); }
            set { SetAttribute("sellers", value); }
        }

        public string[] SellerAltNames
        {
            get { return GetAttribute<string[]>("sellerAltNames"); }
            set { SetAttribute("sellers", value); }
        }

        public string[] SellerDocs
        {
            get { return GetAttribute<string[]>("sellerDocs"); }
            set { SetAttribute("sellers", value); }
        }

        public string[] SellerEmails
        {
            get { return GetAttribute<string[]>("sellerEmails"); }
            set { SetAttribute("sellers", value); }
        }

        public string[] SplitRules
        {
            get { return GetAttribute<string[]>("splitRules"); }
            set { SetAttribute("splitRules", value); }
        }
    }
}