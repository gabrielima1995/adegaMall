using Application.Config;
using Domain.DTO;
using Domain.Entities;
using Domain.Entities.StoreModule;
using Domain.Interfaces;
using Domain.Interfaces.Repositories;
using Framework;
using PaymentIntegrator.Core.Customers;
using PaymentIntegrator.Core.PaymentMethods;
using PaymentIntegrator.Core.Payments;
using PaymentIntegrator.Core.Suppliers;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Infrastructure
{
    public class PaymentService : IPaymentService
    {
        private readonly ICustomerOperations customerOperations;
        private readonly IPaymentMethodOperations paymentMethodOperations;
        private readonly IPaymentOperations paymentOperations;
        private readonly IScheduledTransferRepository scheduledTransferRepository;
        private readonly ISellerRepository sellerRepository;
        private readonly ISupplierOperations supplierOperations;

        public PaymentService(
            ICustomerOperations customerOperations,
            IPaymentMethodOperations paymentMethodOperations,
            IPaymentOperations paymentOperations,
            IScheduledTransferRepository scheduledTransferRepository,
            ISellerRepository sellerRepository,
            ISupplierOperations supplierOperations)
        {
            this.customerOperations = customerOperations;
            this.paymentMethodOperations = paymentMethodOperations;
            this.paymentOperations = paymentOperations;
            this.scheduledTransferRepository = scheduledTransferRepository;
            this.sellerRepository = sellerRepository;
            this.supplierOperations = supplierOperations;
        }

        public OperationResult<ICreditCardResult> CreateCreditCard(Buyer buyer, Domain.DTO.CreditCard creditCard)
        {
            var ccAdapter = new CreditCardAdapter(creditCard);
            return paymentMethodOperations.CreateCreditCard(buyer, ccAdapter);
        }

        public OperationResult<ITransaction> GetTransaction(string transactionId)
        {
            return paymentOperations.GetTransaction(transactionId);
        }

        public OperationResult<ITransaction> Pay(IEnumerable<Order> orders, Buyer buyer, PaymentOptions paymentOptions)
        {
            var paymentOptionsAdapter = new PaymentOptionsAdapter(paymentOptions);

            var payResult = paymentOperations.Pay(orders.Cast<IOrder>().ToList(), buyer, paymentOptionsAdapter, buyer.Address);
            if (payResult.IsFailure) return new OperationResult<ITransaction>(payResult);

            var transaction = payResult.Result;

            if (paymentOptions.Method == PaymentMethod.CreditCard)
            {
                if (!transaction.IsPaid() && !transaction.IsPendingPayment())
                    return new OperationResult<ITransaction>(new ErrorDictionary("", transaction.Message));
            }

            return new OperationResult<ITransaction>(transaction);
        }

        public OperationResult SetAutoTransfer(string id, bool enabled)
        {
            var seller = sellerRepository.GetById(id);

            var result = supplierOperations.SetAutoTransferEnabled(seller, enabled);
            if (result.OK && seller.GatewayTransferEnabled != enabled)
            {
                seller.GatewayTransferEnabled = enabled;
                sellerRepository.Update(seller, true);
            }

            return result;
        }

        public OperationResult ScheduleTransfersToSellersForWhenFundsAreAvailable(Order order)
        {
            if (order.Transaction == null)
                throw new InvalidOperationException("Cannot schedule transfers for an order without a transaction.");

            for (int installment = 1; installment <= order.Transaction.Installments; installment++)
            {
                if (MaaSConfig.Current.Payment.GatewayIsWirecard() && installment > 1)
                    break;

                var installmentForDetails = order.PaymentMethod == PaymentMethod.Boleto ? 0 : installment;
                var details = GetPayableDetails(order.Transaction.TransactionId, order.Seller.SupplierGatewayId, installmentForDetails);
                if (details.IsFailure)
                    return new OperationResult("error", details.Errors.First().Value);                
                if (details == null)
                    return new OperationResult("error", "Payable not found");

                var transfer = ScheduledTransfer.FromOrderAndPayable(order, details.Result);
                var result = scheduledTransferRepository.Add(transfer);
                if (result.IsFailure)
                    return new OperationResult("error", "Error adding item to repository");
            }

            return scheduledTransferRepository.SaveChanges();
        }

        public OperationResult<PayableDto> GetPayableDetails(string transactionId, string recipientId, int installment = 1)
        {
            return paymentOperations.GetPayableDetails(transactionId, recipientId, installment);
        }

        public OperationResult<ITransaction> Refund(IOrder order, ITransaction transaction, decimal totalRefund)
        {
            return new OperationResult<ITransaction>(paymentOperations.Refund(order, transaction, totalRefund));
        }

        public OperationResult RegisterAsCustomer(Buyer buyer, Address address)
        {
            return customerOperations.Create(buyer, address);
        }
    }

    public class CreditCardAdapter : ICreditCard
    {
        public CreditCardAdapter()
        {
        }

        public CreditCardAdapter(Domain.DTO.CreditCard dtoCreditCard)
        {
            Number = dtoCreditCard.Number;
            CVV = dtoCreditCard.CVV;
            FullName = dtoCreditCard.FullName;
            Month = dtoCreditCard.Month;
            Year = dtoCreditCard.Year;
        }

        public string Number { get; set; }
        public string CVV { get; set; }
        public string FullName { get; set; }
        public int Month { get; set; }
        public int Year { get; set; }

        public string DocumentNumber { get; set; }
        public DateTime? DateOfBirth { get; set; }
    }

    public class PaymentOptionsAdapter : IPaymentOptions
    {
        public PaymentOptionsAdapter()
        {
        }

        public PaymentOptionsAdapter(PaymentOptions dtoPaymentOptions)
        {
            Installments = dtoPaymentOptions.Installments;
            Method = dtoPaymentOptions.Method;
            CreditCardResult = dtoPaymentOptions.CreditCardResult;
            CreditCard = dtoPaymentOptions.CreditCard;
            PostbackUrl = dtoPaymentOptions.PostbackUrl;
        }

        public int Installments { get; set; }
        public PaymentMethod Method { get; set; }
        public ICreditCardResult CreditCardResult { get; set; }
        public ICreditCard CreditCard { get; set; }
        public string PostbackUrl { get; set; }
    }
}
