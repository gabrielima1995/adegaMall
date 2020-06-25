using Application.Config;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Presentation.Areas.Admin.Models
{
    public class BusinessConfigViewModel
    {
        [Required]
        [DisplayName("Comissão percentual global")]
        public decimal GlobalFeePercent { get; set; }

        [Required]
        [DisplayName("Comissão fixa global")]
        public decimal GlobalFeeFixed { get; set; }

        [DisplayName("Permitir taxas personalizadas")]
        public bool AllowCustomFees { get; set; }

        [DisplayName("Boleto Bancário")]
        public bool AllowBoleto { get; set; }

        [DisplayName("Cartão de Crédito")]
        public bool AllowCreditCard { get; set; }

        [Range(1, 12)]
        [RequiredIfLocalized("AllowCreditCard")]
        [DisplayName("Número máximo de parcelas")]
        public int MaxInstallments { get; set; }

        [Range(5.0d, double.MaxValue, ErrorMessage = "Valor deve ser maior do que R$ 5,00")]
        [RequiredIfLocalized("AllowCreditCard")]
        [DisplayName("Valor mínimo por parcela")]
        public decimal MinInstallmentValue { get; set; }

        [DisplayName("Valor mínimo por pedido")]
        [Required]
        [Range(1.0d, double.MaxValue, ErrorMessage = "Valor deve ser maior do que R$ 1,00")]
        public decimal MinOrderValue { get; set; }

        [DisplayName("Configuração de parcelamento por produto")]
        public bool AllowInstallmentsPerProduct { get; set; }

        public BusinessConfigViewModel()
        {
        }

        public BusinessConfigViewModel(string activeGateway)
        {
            GlobalFeeFixed = MaaSConfig.Current.Payment.Common.GlobalStartupAmountFee;
            GlobalFeePercent = MaaSConfig.Current.Payment.Common.GlobalStartupPercentageFee;
            AllowCustomFees = MaaSConfig.Current.Payment.Common.EnableCustomFeesByVendor;

            AllowBoleto = MaaSConfig.Current.Payment.PaymentGateway.Boleto.Enabled;
            AllowCreditCard = MaaSConfig.Current.Payment.PaymentGateway.CreditCard.Enabled;

            MaxInstallments = MaaSConfig.Current.Payment.PaymentGateway.Installments.MaxInstallments;
            MinInstallmentValue = MaaSConfig.Current.Payment.PaymentGateway.Installments.MinInstallmentAmount;
            MinOrderValue = MaaSConfig.Current.Payment.PaymentGateway.Installments.MinOrderAmount;

            if (MaaSConfig.Current.Product.Enabled)
            {
                AllowInstallmentsPerProduct = MaaSConfig.Current.Product.EnableInstallmentsPerProduct;
            }
        }

        public void SaveToConfig(MaaSConfig config)
        {
            config.Payment.Common.GlobalStartupAmountFee = GlobalFeeFixed;
            config.Payment.Common.GlobalStartupPercentageFee = GlobalFeePercent;
            config.Payment.Common.EnableCustomFeesByVendor = AllowCustomFees;

            config.Payment.PaymentGateway.Boleto.Enabled = AllowBoleto;
            config.Payment.PaymentGateway.CreditCard.Enabled = AllowCreditCard;

            config.Payment.PaymentGateway.Installments.MaxInstallments = MaxInstallments;
            config.Payment.PaymentGateway.Installments.MinInstallmentAmount = MinInstallmentValue;
            config.Payment.PaymentGateway.Installments.MinOrderAmount = MinOrderValue;

            if (MaaSConfig.Current.Product.Enabled)
            {
                MaaSConfig.Current.Product.EnableInstallmentsPerProduct = AllowInstallmentsPerProduct;
            }
        }
    }
}