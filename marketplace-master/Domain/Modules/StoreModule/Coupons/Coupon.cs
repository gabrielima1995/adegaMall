using Domain.Entities;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Modules.StoreModule.Coupons
{
    public enum CouponBenefitType
    {
        PercentDiscount = 0,
        ValueDiscount = 1,
        DeliveryPercentDiscount = 2,
        DeliveryValueDiscount = 3
    }

    public enum CouponUseMode
    {
        FirstOrder = 0,
        OneTimeByUser = 1,
        OneTimeEver = 2
    }

    public class Coupon : SoftDeletableEntity<Guid>
    {
        public string Code { get; private set; }

        public bool Enabled { get; private set; } = true;

        [ForeignKey("Seller")]
        public string SellerId { get; private set; }
        public Seller Seller { get; private set; }

        public CouponBenefitType BenefitType { get; private set; }

        public decimal Value { get; private set; }

        public CouponUseMode UseMode { get; private set; }

        public int TotalMaxUse { get; private set; } = -1;

        public int TotalUse { get; private set; }

        public int DistanceInKm { get; private set; }

        public decimal MinOrderAmount { get; private set; }

        public bool IsDiscount =>
            BenefitType == CouponBenefitType.PercentDiscount ||
            BenefitType == CouponBenefitType.ValueDiscount;

        public bool IsDeliveryDiscount =>
            BenefitType == CouponBenefitType.DeliveryPercentDiscount ||
            BenefitType == CouponBenefitType.DeliveryValueDiscount;

        public bool IsBySeller => !string.IsNullOrWhiteSpace(SellerId);

        protected Coupon()
        {
        }

        public Coupon(string code, string sellerId,
            CouponBenefitType benefitType, decimal value, CouponUseMode useMode,
            int distanceInKm, int minOrderAmount)
        {
            if (code == null) throw new ArgumentNullException(nameof(code));
            if (value <= 0) throw new ArgumentOutOfRangeException(nameof(value));

            Code = code.ToUpper().Replace(" ", string.Empty);
            SellerId = sellerId;
            BenefitType = benefitType;
            Value = value;
            UseMode = useMode;
            DistanceInKm = distanceInKm;
            MinOrderAmount = minOrderAmount;
        }

        public Coupon Update(bool enabled)
        {
            Enabled = enabled;

            return this;
        }

        public bool IsValid()
        {
            return Enabled &&
                   !Deleted &&
                   (TotalMaxUse == -1 || TotalMaxUse > TotalUse);
        }

        public decimal CalculateDiscount(decimal amount, decimal totalAmount)
        {
            if(amount == decimal.Zero || totalAmount == decimal.Zero) return decimal.Zero;

            if (IsDiscount)
            {
                switch (BenefitType)
                {
                    case CouponBenefitType.PercentDiscount:
                        return Math.Round(amount * Value / 100, 2, MidpointRounding.AwayFromZero);
                    case CouponBenefitType.ValueDiscount:
                        return Math.Min(Value * (amount / totalAmount), amount);
                    default:
                        throw new NotImplementedException();
                }
            }

            throw new ArgumentException();
        }

        public decimal CalculateDeliveryDiscount(decimal deliveryAmount, decimal totalDeliveryAmount)
        {
            if (deliveryAmount == decimal.Zero || totalDeliveryAmount == decimal.Zero) return decimal.Zero;

            if (IsDeliveryDiscount)
            {
                switch (BenefitType)
                {
                    case CouponBenefitType.DeliveryPercentDiscount:
                        return Math.Round(deliveryAmount * Value / 100, 2, MidpointRounding.AwayFromZero);
                    case CouponBenefitType.DeliveryValueDiscount:
                        return Math.Min(Value * (deliveryAmount / totalDeliveryAmount), deliveryAmount);
                    default:
                        throw new NotImplementedException();
                }
            }

            throw new ArgumentException();
        }
    }
}
