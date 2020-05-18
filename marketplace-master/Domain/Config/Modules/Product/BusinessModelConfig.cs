using Application.Config.Common;
using Domain.Modules.StoreModule.Entities;
using Domain.StoreModule.Entities;
using FluentValidation;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Domain.Config.Modules.Product
{
    public class BusinessModelConfig
    {
        public BusinessModelConfig_Sale Sale { get; set; } = new BusinessModelConfig_Sale();

        public BusinessModelConfig_Rental Rental { get; set; } = new BusinessModelConfig_Rental();

        [JsonIgnore]
        public bool ShowStock => (Rental.Enabled && Rental.QuantityEnabled) || Sale.Enabled;
    }

    public class BusinessModelConfig_Sale
    {
        public bool Enabled { get; set; } = true;
    }

    public class BusinessModelConfig_Rental
    {
        public bool Enabled { get; set; } = false;

        public bool QuantityEnabled { get; set; } = false;

        public RentalPeriodUnit RentalPeriodUnit { get; set; } = RentalPeriodUnit.Hour;

        public string RentalPeriodMode { get; set; } = StoreModule.Entities.RentalPeriodMode.Unrestricted;

        public bool FormatPriceByRentalPeriodUnit { get; set; } = false;

        public bool HideShiftDurationFromUsers { get; set; } = false;

        public bool EnablePeriodPerProduct { get; set; } = false;

        public int MinRentalPeriod { get; set; } = 8;

        public bool EnforceWholeUnits { get; set; } = false;

        public bool EnforceNoOverlapingInCart { get; set; } = true;

        public bool HideMinMaxPeriod { get; set; } = false;

        public bool EnablePriceMultiplier { get; set; } = false;

        public StatusConfig MinRentalAmount { get; set; } = new StatusConfig();

        public BusinessModelConfig_RentalFixedPeriodRental FixedPeriodRental { get; set; } = new BusinessModelConfig_RentalFixedPeriodRental();

        public RentalFixedPeriodForAllProductsConfig RentalFixedPeriodForAllProducts { get; set; } = new RentalFixedPeriodForAllProductsConfig();

        public BusinessModelConfig_RentalReminderNotification ReminderNotification { get; set; } = new BusinessModelConfig_RentalReminderNotification();

        public BusinessModelConfig_RentalPeriodInHours ConvertRentalPeriodInHours(RentalPeriodUnit unit, int period) => BusinessModelConfig_RentalPeriodInHours.FromUnit(unit, period);

        /// <summary>
        /// Configuração de ações do vendedor.
        /// </summary>
        public BusinessModelConfig_RentalSeller Seller { get; set; } = new BusinessModelConfig_RentalSeller();

        /// <summary>
        /// Configuração de ações do comprador.
        /// </summary>
        public BusinessModelConfig_RentalBuyer Buyer { get; set; } = new BusinessModelConfig_RentalBuyer();

        /// <summary>
        /// Configuração de grade de horários para locação.
        /// </summary>
        public BusinessModelConfig_RentalGridView GridView { get; set; } = new BusinessModelConfig_RentalGridView();

        /// <summary>
        /// Configuração de produto Pacote de créditos.
        /// </summary>
        public BusinessModelConfig_RentalPackage Package { get; set; } = new BusinessModelConfig_RentalPackage();

        /// <summary>
        /// Configuração de disponibilidade de vagas por horário.
        /// </summary>
        public BusinessModelConfig_ProductAvailability ProductAvailability { get; set; } = new BusinessModelConfig_ProductAvailability();

        /// <summary>
        /// Configuração responsável por definir o 2x o preço do frete em caso de devolução
        /// </summary>
        public StatusConfig ChargeDoubleShipping { get; set; } = new StatusConfig(true);
    }

    public struct BusinessModelConfig_RentalPeriodInHours
    {
        private BusinessModelConfig_RentalPeriodInHours(double low, double high)
        {
            Low = low;
            High = high;
        }

        public double Low { get; }
        public double High { get; }

        public bool IsInBounds(double value) => value >= Low && value <= High;

        public static BusinessModelConfig_RentalPeriodInHours Zero => new BusinessModelConfig_RentalPeriodInHours(0, 1000);

        public static BusinessModelConfig_RentalPeriodInHours FromUnit(RentalPeriodUnit unit, double period)
        {
            switch (unit)
            {
                case RentalPeriodUnit.Minute:
                    var valMinute = Math.Round(period / 60D, 7);
                    return new BusinessModelConfig_RentalPeriodInHours(valMinute, valMinute);

                case RentalPeriodUnit.Hour:
                    return new BusinessModelConfig_RentalPeriodInHours(period, period);

                case RentalPeriodUnit.Day:
                    var valDay = period * 24;
                    return new BusinessModelConfig_RentalPeriodInHours(valDay, valDay);

                case RentalPeriodUnit.Month:
                    return new BusinessModelConfig_RentalPeriodInHours(period * 24 * 30, period * 24 * 31);

                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public static implicit operator double(BusinessModelConfig_RentalPeriodInHours period) => period.Low;

        public static implicit operator BusinessModelConfig_RentalPeriodInHours(double hours) => new BusinessModelConfig_RentalPeriodInHours(hours, hours);
    }

    public class BusinessModelConfig_RentalFixedPeriodRental : RegisterConfig
    {
        public bool RegisterByBasePrice { get; set; } = false;

        public BusinessModelConfig_RentalFixedPeriodRental()
            : base(false, false)
        {
        }
    }

    public class BusinessModelConfig_RentalReminderNotification : StatusConfig
    {
        public RentalPeriodUnit Unit { get; set; } = RentalPeriodUnit.Day;

        public int UnitValue { get; set; } = 1;

        public BusinessModelConfig_RentalReminderNotification()
            : base(false)
        {
        }
    }

    public class BusinessModelConfigValidator : AbstractValidator<BusinessModelConfig>
    {
        public BusinessModelConfigValidator()
        {
            RuleFor(x => x.Sale).NotNull();
            RuleFor(x => x.Sale.Enabled).Equal(true).Unless(x => x.Rental.Enabled == true);

            RuleFor(x => x.Rental).NotNull();
            RuleFor(x => x.Rental).SetValidator(new BusinessModelConfig_RentalValidator());
        }
    }

    public class BusinessModelConfig_RentalValidator : AbstractValidator<BusinessModelConfig_Rental>
    {
        public BusinessModelConfig_RentalValidator()
        {
            RuleFor(x => x.MinRentalPeriod).GreaterThanOrEqualTo(0);
            RuleFor(x => x.ReminderNotification).SetValidator(new BusinessModelConfig_RentalReminderNotificationValidator());
        }
    }

    public class BusinessModelConfig_RentalReminderNotificationValidator : AbstractValidator<BusinessModelConfig_RentalReminderNotification>
    {
        public BusinessModelConfig_RentalReminderNotificationValidator()
        {
            RuleFor(x => x.UnitValue).GreaterThanOrEqualTo(0);
        }
    }

    /// <summary>
    /// Configuração de grade de horários para locação.
    /// </summary>
    public class BusinessModelConfig_RentalGridView
    {
        public bool Enabled { get; set; } = false;

        /// <summary>
        /// Número de dias para exibir na grade.
        /// </summary>
        public int Days { get; set; } = 5;

        /// <summary>
        /// Número de horários para exibir por dia na grade.
        /// </summary>
        public int Periods { get; set; } = 5;
    }

    /// <summary>
    /// Configuração de ações do vendedor.
    /// </summary>
    public class BusinessModelConfig_RentalSeller
    {
        public BusinessModelConfig_RentalSellerSchedule Schedule { get; set; } = new BusinessModelConfig_RentalSellerSchedule();
    }

    /// <summary>
    /// Configuração da agenda do vendedor.
    /// </summary>
    public class BusinessModelConfig_RentalSellerSchedule
    {
        public BusinessModelConfig_RentalSellerScheduleConfiguration Configuration { get; set; } = new BusinessModelConfig_RentalSellerScheduleConfiguration();
    }

    /// <summary>
    /// Configuração das formas disponíveis para o vendedor configurar sua agenda.
    /// </summary>
    public class BusinessModelConfig_RentalSellerScheduleConfiguration
    {
        private string minTime = "00:00:00";
        private string maxTime = "24:00:00";

        public string ViewMode { get; set; } = ScheduleViewMode.Default;

        public bool TimelineViewModeEnabled() => ViewMode == ScheduleViewMode.Timeline;

        public StatusConfig Availability { get; set; } = new StatusConfig(enabled: true);
        public StatusConfig BatchLocks { get; set; } = new StatusConfig(enabled: true);
        public StatusConfig SingleLocks { get; set; } = new StatusConfig(enabled: true);

        /// <summary>
        /// Configuração da agenda do vendedor na sua página de perfil.
        /// </summary>
        public BusinessModelConfig_RentalSellerScheduleConfigurationProfile Profile { get; set; } = new BusinessModelConfig_RentalSellerScheduleConfigurationProfile();

        public string MinTime
        {
            get => minTime;
            set => minTime = ValidateTimeFormat(value);
        }

        public string MaxTime
        {
            get => maxTime;
            set => maxTime = ValidateTimeFormat(value);
        }

        private string ValidateTimeFormat(string input)
        {
            if (!Regex.IsMatch(input, @"^\d{2}\:\d{2}\:\d{2}$"))
                throw new ArgumentException($"config > product > rental > seller > schedule: invalid input {input}; should be in the format hh:mm:ss");
            return input;
        }
    }

    /// <summary>
    /// Configuração da agenda do vendedor na sua página de perfil.
    /// </summary>
    public class BusinessModelConfig_RentalSellerScheduleConfigurationProfile : StatusConfig
    {
        private string minTime = "00:00:00";
        private string maxTime = "24:00:00";
        private string slotDuration = "01:00:00";
        private string slotLabelInterval = null;

        public BusinessModelConfig_RentalSellerScheduleConfigurationProfile()
            : base(false)
        {
        }

        public string MinTime
        {
            get => minTime;
            set => minTime = ValidateTimeFormat(value);
        }

        public string MaxTime
        {
            get => maxTime;
            set => maxTime = ValidateTimeFormat(value);
        }

        public string SlotDuration
        {
            get => slotDuration;
            set => slotDuration = ValidateTimeFormat(value);
        }

        public string SlotLabelInterval
        {
            get => slotLabelInterval;
            set => slotLabelInterval = string.IsNullOrEmpty(value) // campo pode ser vazio, nulo ou com valor no formato hora
                ? value 
                : ValidateTimeFormat(value);
        }

        public List<int> HiddenDays { get; set; } = new List<int>();

        private string ValidateTimeFormat(string input)
        {
            if (!Regex.IsMatch(input, @"^\d{2}\:\d{2}\:\d{2}$"))
                throw new ArgumentException($"config > product > rental > seller > schedule > profile: invalid input {input}; should be in the format hh:mm:ss");
            return input;
        }
    }

    /// <summary>
    /// Configuração de ações do comprador.
    /// </summary>
    public class BusinessModelConfig_RentalBuyer
    {
        public BusinessModelConfig_RentalBuyerCancel Cancel { get; set; } = new BusinessModelConfig_RentalBuyerCancel();
        public BusinessModelConfig_RentalBuyerSchedule Schedule { get; set; } = new BusinessModelConfig_RentalBuyerSchedule();
        public BusinessModelConfig_RentalBuyerChangePeriod ChangePeriod { get; set; } = new BusinessModelConfig_RentalBuyerChangePeriod();
    }

    public class BusinessModelConfig_RentalBuyerCancel : StatusConfig
    {
        public int BeforeInHours { get; set; } = 12;

        public BusinessModelConfig_RentalBuyerCancel()
            : base(false)
        {
        }
    }

    public class BusinessModelConfig_RentalBuyerSchedule : StatusConfig
    {
        private string minTime = "00:00:00";
        private string maxTime = "24:00:00";

        public BusinessModelConfig_RentalBuyerSchedule() : base(false)
        {
        }

        public string MinTime
        {
            get => minTime;
            set => minTime = ValidateTimeFormat(value);
        }

        public string MaxTime
        {
            get => maxTime;
            set => maxTime = ValidateTimeFormat(value);
        }

        private string ValidateTimeFormat(string input)
        {
            if (!Regex.IsMatch(input, @"^\d{2}\:\d{2}\:\d{2}$"))
                throw new ArgumentException($"config > product > rental > buyer > schedule: invalid input {input}; should be in the format hh:mm:ss");
            return input;
        }
    }

    public class BusinessModelConfig_RentalBuyerChangePeriod : StatusConfig
    {
        public int BeforeInHours { get; set; } = 12;

        public BusinessModelConfig_RentalBuyerChangePeriod()
            : base(false)
        {
        }
    }

    /// <summary>
    /// Configuração de produto Pacote de créditos.
    /// </summary>
    public class BusinessModelConfig_RentalPackage : StatusConfig
    {
        public BusinessModelConfig_RentalPackage()
            : base(false)
        {
        }
    }

    /// <summary>
    /// Configuração de disponibilidade de vagas por horário.
    /// </summary>
    public class BusinessModelConfig_ProductAvailability : StatusConfig
    {
        /// <summary>
        /// Configuração de disponibilidade de vagas com recorrência.
        /// </summary>
        public BusinessModelConfig_ProductAvailability_Recurring Recurring { get; set; } = new BusinessModelConfig_ProductAvailability_Recurring();

        public BusinessModelConfig_ProductAvailability()
            : base(false)
        {
        }
    }

    /// <summary>
    /// Configuração de disponibilidade de vagas com recorrência.
    /// </summary>
    public class BusinessModelConfig_ProductAvailability_Recurring : StatusConfig
    {
        public BusinessModelConfig_ProductAvailability_Recurring()
            : base(false)
        {
        }
    }
}