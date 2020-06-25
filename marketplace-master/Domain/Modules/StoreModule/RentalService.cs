using Application.Config;
using Domain.DTO;
using Domain.Entities.StoreModule;
using Domain.Interfaces;
using Domain.Interfaces.Repositories;
using Domain.Modules.StoreModule;
using Domain.Modules.StoreModule.Entities;
using Domain.Modules.StoreModule.Interfaces;
using Domain.StoreModule.Entities;
using Domain.StoreModule.Interfaces;
using Framework;
using Infra.Crosscutting.Globalization;
using System;
using System.Linq;

namespace Domain.Services.StoreModule
{
    public class RentalService
    {
        private static readonly TimeSpan OneMinute = TimeSpan.FromMinutes(1);

        private readonly ITimeZoneProvider _userTimeZoneProvider;
        private readonly ProductAvailabilityService _productAvailabilityService;
        private readonly IRentalLockQueries _rentalLockQueries;
        private readonly IRentalShiftQueries _rentalShiftQueries;
        private readonly IOrderRepository _orderRepository;
        private readonly IProductAvailabilityRepository _productAvailabilityRepository;
        private readonly IRepository<OrderProduct> _orderProductRepository;

        public RentalService(
            ITimeZoneProvider userTimeZoneProvider,
            ProductAvailabilityService productAvailabilityService,
            IRentalLockQueries rentalLockQueries,
            IRentalShiftQueries rentalShiftQueries,
            IOrderRepository orderRepository,
            IProductAvailabilityRepository productAvailabilityRepository,
            IRepository<OrderProduct> orderProductRepository)
        {
            _userTimeZoneProvider = userTimeZoneProvider;
            _productAvailabilityService = productAvailabilityService;
            _rentalLockQueries = rentalLockQueries;
            _rentalShiftQueries = rentalShiftQueries;
            _orderRepository = orderRepository;
            _productAvailabilityRepository = productAvailabilityRepository;
            _orderProductRepository = orderProductRepository;
        }

        public OperationResult<RentalCalendarViewModel> GetRentalCalendar(Product product, int quantity,
            DateTime startDate, MarketplaceTimeZone timeZone, Func<DateTime, string> dateUrlGenerator,
            Guid? ignoreOrderProductId, Guid? ignoreOrderProductRentalInfoId, Guid? productAvailabilityId)
        {
            #region Verifica se produto foi excluído

            if (product.Deleted) return new OperationResult<RentalCalendarViewModel>("", GlobalizationHelper.GetString("RENTAL_PRODUCT_DELETED_MESSAGE"));

            #endregion Verifica se produto foi excluído

            #region Verifica data de início da grade

            var isMinStartDate = false;
            var minStartDate = timeZone.ConvertTimeFromUtc(DateTime.UtcNow);

            var minRentalNoticeInHours = product.RentalSettings.GetMinRentalNoticeInHours();

            minStartDate = minStartDate.AddHours(minRentalNoticeInHours);

            if (startDate.Date <= minStartDate.Date)
            {
                startDate = minStartDate;

                isMinStartDate = true;
            }

            #endregion Verifica data de início da grade

            var shifts = _rentalShiftQueries.ListRentalShifts().ToArray();
            var productShifts = _rentalShiftQueries.ListProductShifts(product.Id);

            var totalDays = 7;
            var endDate = startDate.AddDays(totalDays);
            var startDateUtc = timeZone.ConvertTimeToUtc(startDate);
            var endDateUtc = timeZone.ConvertTimeToUtc(endDate);
            var startDateFirstHourUtc = startDateUtc.Subtract(startDate.TimeOfDay);
            var endDateLastHourUtc = startDateFirstHourUtc.AddDays(totalDays);

            // Buscar todos os bloqueios neste periodo para este produto
            var locks = _rentalLockQueries.ListLocksProductRental(product.Id, product.Store_SellerId, startDateFirstHourUtc, endDateLastHourUtc, timeZone, false);

            // Buscar todas as locações realizadas neste periodo para este produto
            var rentals = _orderRepository.ListProductRentalByDates(product.Id, startDateUtc, endDateUtc, true, ignoreOrderProductId, ignoreOrderProductRentalInfoId, productAvailabilityId);

            var calendar = new RentalCalendarViewModel(product,
                startDate, totalDays, timeZone,
                shifts, productShifts, dateUrlGenerator);

            #region Remove os horários de bloqueio

            foreach (var rentalLock in locks)
            {
                var lockStart = timeZone.ConvertTimeFromUtc(rentalLock.LockStartUtc);
                var lockEnd = timeZone.ConvertTimeFromUtc(rentalLock.LockEndUtc).AddMinutes(1);

                calendar.UnavailablePeriods(lockStart, lockEnd);
            }

            #endregion Remove os horários de bloqueio

            var maxRentalPeriodInHours = product.RentalSettings.GetMaxRentalPeriodInHours();

            #region Remove os horários de locação já agendadas

            var rentalMarginInHours = product.RentalSettings.GetRentalMarginInHours();
            var rentalStartMargin = TimeSpan.FromHours(-rentalMarginInHours - maxRentalPeriodInHours);

            if (maxRentalPeriodInHours > 0) rentalStartMargin = rentalStartMargin.Add(OneMinute);

            foreach (var rental in rentals)
            {
                var rentalStart = timeZone.ConvertTimeFromUtc(rental.RentalStartDateTime)
                    .Add(rentalStartMargin);
                var rentalEnd = timeZone.ConvertTimeFromUtc(rental.RentalEndDateTime)
                    .AddHours(rentalMarginInHours);

                calendar.UnavailablePeriods(rentalStart, rentalEnd, (int)rental.Quantity);
            }

            #endregion Remove os horários de locação já agendadas

            #region Remove os horários anteriores a data de início

            if (isMinStartDate)
            {
                var endHour = startDate.AddMinutes(1);

                calendar.UnavailablePeriods(startDate, endHour);
            }

            #endregion Remove os horários anteriores a data de início

            #region Remove os horários que não tem estoque disponível para a quantidade informada

            calendar.AdjustAvailableStockPeriod(quantity);

            #endregion Remove os horários que não tem estoque disponível para a quantidade informada

            return calendar;
        }

        public OperationResult<RentalGridView> GetRentalGrid(Product product, int quantity,
            DateTime startDate, MarketplaceTimeZone timeZone,
            Guid? ignoreOrderProductId = null, Guid? ignoreOrderProductRentalInfoId = null, Guid? productAvailabilityId = null,
            int? totalDaysToGrid = null, bool onlyAvailableStock = true, bool forceMinStartDate = true)
        {
            if (!MaaSConfig.Current.Product.BusinessModel.Rental.Enabled ||
                (product.RentalSettings.RentalPeriodUnit != RentalPeriodUnit.Hour && product.RentalSettings.RentalPeriodUnit != RentalPeriodUnit.Minute) ||
                !MaaSConfig.Current.Product.BusinessModel.Rental.FixedPeriodRental.Enabled ||
                !MaaSConfig.Current.Product.BusinessModel.Rental.GridView.Enabled ||
                !product.RentalSettings.IsFixedRental)
                return new OperationResult<RentalGridView>("", GlobalizationHelper.GetString("RENTAL_GRID_VIEW_UNALLOWED"));

            #region Verifica se produto foi excluído

            if (product.Deleted) return new OperationResult<RentalGridView>("", GlobalizationHelper.GetString("RENTAL_PRODUCT_DELETED_MESSAGE"));

            #endregion Verifica se produto foi excluído

            #region Verifica data de início da grade

            var isMinStartDate = false;

            if (forceMinStartDate)
            {
                var minStartDate = timeZone.ConvertTimeFromUtc(DateTime.UtcNow);
                var minRentalNoticeInHours = product.RentalSettings.GetMinRentalNoticeInHours();

                minStartDate = minStartDate.AddHours(minRentalNoticeInHours);

                if (startDate.Date <= minStartDate.Date)
                {
                    startDate = minStartDate;
                    isMinStartDate = true;
                }
            }

            #endregion Verifica data de início da grade

            var totalDays = totalDaysToGrid ?? MaaSConfig.Current.Product.BusinessModel.Rental.GridView.Days;
            var productAvailabilityEnabled =
                MaaSConfig.Current.Product.BusinessModel.Rental.ProductAvailability.Enabled &&
                product.IsAvailabilityManual;
            var endDate = startDate.AddDays(totalDays);
            var startDateUtc = timeZone.ConvertTimeToUtc(startDate);
            var endDateUtc = timeZone.ConvertTimeToUtc(endDate);
            var startDateFirstHourUtc = startDateUtc.Subtract(startDate.TimeOfDay);
            var endDateLastHourUtc = startDateFirstHourUtc.AddDays(totalDays);

            RentalGridView grid;

            if (!productAvailabilityEnabled)
            {
                grid = RentalGridView.New(product, startDate, totalDays, (int)product.CurrentStock);
            }
            else
            {
                var availabilities = _productAvailabilityRepository.GetListByProduct(product.Id, false, false, productAvailabilityId);

                // Se não for disponibilidade recorrente, desconsidera a busca por disponibilidade
                if (availabilities.Any(i => i.Id == productAvailabilityId && !i.IsRecurring))
                    productAvailabilityId = null;

                grid = RentalGridView.New(product, startDate, totalDays, availabilities);
            }

            // Buscar todos os bloqueios neste periodo para este produto
            var locks = _rentalLockQueries.ListLocksProductRental(product.Id, product.Store_SellerId, startDateFirstHourUtc, endDateLastHourUtc, timeZone, false);

            var rentalLocksForSeller = _orderRepository.ListBlocksForProductInPeriod(product, startDate, endDate, ignoreOrderProductId, ignoreOrderProductRentalInfoId);

            // Buscar todas as locações realizadas neste periodo para este produto
            var rentals = _orderRepository.ListProductRentalByDates(
                product.Id, startDateUtc, endDateUtc, true,
                ignoreOrderProductId, ignoreOrderProductRentalInfoId, productAvailabilityId);

            #region Remove os horários de bloqueio

            foreach (var rentalLock in locks)
            {
                var lockStart = timeZone.ConvertTimeFromUtc(rentalLock.LockStartUtc);
                var lockEnd = timeZone.ConvertTimeFromUtc(rentalLock.LockEndUtc).Add(OneMinute);

                grid.RemovePeriods(lockStart, lockEnd);
            }

            foreach (var rentalLock in rentalLocksForSeller)
            {
                if (!product.RentalSettings.BlockScheduleForProduct && !rentalLock.BlockScheduleForProduct)
                    continue;

                var lockStart = timeZone.ConvertTimeFromUtc(rentalLock.RentalStartDateTime);
                var lockEnd = timeZone.ConvertTimeFromUtc(rentalLock.RentalEndDateTime).Add(OneMinute);

                grid.RemovePeriods(lockStart, lockEnd);
            }

            #endregion Remove os horários de bloqueio

            #region Remove os horários dentro da duração fixa

            var fixedPeriodInHours = product.RentalSettings.GetMaxRentalPeriodInHours();

            if (!productAvailabilityEnabled)
            {
                grid.AdjustFixedPeriod(TimeSpan.FromHours(fixedPeriodInHours),
                    product.RentalSettings.RentalPeriodUnit.GetValueOrDefault());
            }

            #endregion Remove os horários dentro da duração fixa

            #region Remove os horários de locação já agendadas

            var rentalMarginInHours = product.RentalSettings.GetRentalMarginInHours();
            var rentalStartMargin = TimeSpan.FromHours(-rentalMarginInHours - fixedPeriodInHours);

            if (fixedPeriodInHours > 0) rentalStartMargin = rentalStartMargin.Add(OneMinute);

            foreach (var rental in rentals)
            {
                var rentalStart = timeZone.ConvertTimeFromUtc(rental.RentalStartDateTime)
                    .Add(rentalStartMargin);
                var rentalEnd = timeZone.ConvertTimeFromUtc(rental.RentalEndDateTime)
                    .AddHours(rentalMarginInHours);

                grid.RemovePeriods(rentalStart, rentalEnd, (int)rental.Quantity, rental.ProductAvailabilityId);
            }

            #endregion Remove os horários de locação já agendadas

            #region Remove os horários anteriores a data de início

            if (isMinStartDate)
            {
                var endHour = startDate.Add(OneMinute);
                grid.Days[0].RemovePeriodsUntil(endHour.TimeOfDay);
            }

            #endregion Remove os horários anteriores a data de início

            #region Remove os horários que não tem estoque disponível para a quantidade informada

            if (onlyAvailableStock)
            {
                grid.AdjustAvailableStockPeriod(quantity);
            }
            else
            {
                grid.AdjustAvailabilitiesDeletedAndRentalsOffPeriods();
            }

            #endregion Remove os horários que não tem estoque disponível para a quantidade informada

            return grid;
        }

        public OperationResult<AvailabilityDates> CheckAvailability(Product product, int quantity, DateTime rentalStartUtc, DateTime rentalEndUtc, MarketplaceTimeZone timeZone, Guid? ignoreOrderProductId = null, Guid? ignoreOrderProductRentalInfoId = null, Guid? productAvailabilityId = null)
        {
            int availableUnits;
            OperationResult<AvailabilityDates> errors;

            errors = CheckAvailability_Deleted(product, quantity, rentalStartUtc, rentalEndUtc, timeZone);
            if (errors != null) return errors;

            errors = CheckAvailability_RentalPeriodInvalid(product, quantity, rentalStartUtc, rentalEndUtc, timeZone);
            if (errors != null) return errors;

            errors = CheckAvailability_MinNoticePeriod(product, quantity, rentalStartUtc, rentalEndUtc, timeZone);
            if (errors != null) return errors;

            errors = CheckAvailability_MinMaxLimits(product, quantity, rentalStartUtc, rentalEndUtc, timeZone);
            if (errors != null) return errors;

            errors = CheckAvailability_Locks(product, quantity, rentalStartUtc, rentalEndUtc, timeZone);
            if (errors != null) return errors;

            ProductAvailability productAvailability = null;

            if (MaaSConfig.Current.Product.BusinessModel.Rental.ProductAvailability.Enabled &&
                product.IsAvailabilityManual &&
                productAvailabilityId.HasValue)
            {
                productAvailability = _productAvailabilityService.GetById(
                    productAvailabilityId.Value, product.Store_SellerId);
            }

            var currentStock = GetCurrentStock(product, rentalStartUtc, rentalEndUtc, timeZone, productAvailability);

            errors = CheckAvailability_Rentals(product, quantity, rentalStartUtc, rentalEndUtc, timeZone, out availableUnits, ignoreOrderProductId, ignoreOrderProductRentalInfoId, currentStock, productAvailability);
            if (errors != null) return errors;

            errors = CheckAvailability_Without_BlockScheduleForProduct(product, rentalStartUtc, rentalEndUtc, timeZone, ignoreOrderProductId, ignoreOrderProductRentalInfoId);
            if (errors != null) return errors;

            return new OperationResult<AvailabilityDates>(
                AvailabilityDates.Available(rentalStartUtc, rentalEndUtc, timeZone, availableUnits));
        }

        private OperationResult<AvailabilityDates> CheckAvailability_Without_BlockScheduleForProduct(Product product, DateTime rentalStartUtc, DateTime rentalEndUtc, MarketplaceTimeZone timeZone, Guid? ignoreOrderProductId, Guid? ignoreOrderProductRentalInfoId)
        {
            rentalStartUtc = rentalStartUtc.AddMinutes(1);
            rentalEndUtc = rentalEndUtc.AddMinutes(-1);

            var locks = _orderRepository.ListBlocksForProductInPeriod(product, rentalStartUtc, rentalEndUtc, ignoreOrderProductId, ignoreOrderProductRentalInfoId);

            if (locks.Any())
            {
                bool blockSchedule = false;
                locks.ForEach(l => blockSchedule = l.BlockScheduleForProduct.Equals(true));
                if (blockSchedule || product.RentalSettings.BlockScheduleForProduct)
                    return new OperationResult<AvailabilityDates>(
                    AvailabilityDates.Unavailable(rentalStartUtc, rentalEndUtc, timeZone),
                    new ErrorDictionary("", GlobalizationHelper.GetString("RENTAL_PERIOD_NOT_AVAILABLE_MESSAGE_FOR_BLOCK")));
            }
            return null;
        }

        private OperationResult<AvailabilityDates> CheckAvailability_Deleted(Product product, int quantity, DateTime rentalStartUtc, DateTime rentalEndUtc, MarketplaceTimeZone timeZone)
        {
            if (product.Deleted)
            {
                return new OperationResult<AvailabilityDates>(
                    AvailabilityDates.Unavailable(rentalStartUtc, rentalEndUtc, timeZone),
                    new ErrorDictionary("", GlobalizationHelper.GetString("RENTAL_PRODUCT_DELETED_MESSAGE")));
            }

            return null;
        }

        private OperationResult<AvailabilityDates> CheckAvailability_RentalPeriodInvalid(Product product, int quantity, DateTime rentalStartUtc, DateTime rentalEndUtc, MarketplaceTimeZone timeZone)
        {
            if (rentalStartUtc >= rentalEndUtc || rentalStartUtc <= DateTime.UtcNow)
            {
                return new OperationResult<AvailabilityDates>(
                    AvailabilityDates.Unavailable(rentalStartUtc, rentalEndUtc, timeZone),
                    new ErrorDictionary("", GlobalizationHelper.GetString("RENTAL_INVALID_PERIOD_MESSAGE")));
            }

            return null;
        }

        private OperationResult<AvailabilityDates> CheckAvailability_MinNoticePeriod(Product product, int quantity, DateTime rentalStartUtc, DateTime rentalEndUtc, MarketplaceTimeZone timeZone)
        {
            var minRentalNoticeInHours = product.RentalSettings.GetMinRentalNoticeInHours();
            var periodMinStartUtc = DateTime.UtcNow.AddHours(minRentalNoticeInHours);

            if (rentalStartUtc < periodMinStartUtc)
            {
                var retryFrom = GlobalizationHelper.FormatString("RENTAL_TRY_FROM_MESSAGE",
                    FormatHelper.DisplayDateTime(periodMinStartUtc.RoundToHourUp(), timeZone));

                var msg = GlobalizationHelper.GetString("RENTAL_PERIOD_NOT_AVAILABLE_MESSAGE");

                return new OperationResult<AvailabilityDates>(
                    AvailabilityDates.Unavailable(rentalStartUtc, rentalEndUtc, timeZone),
                    new ErrorDictionary("", $"{msg} {retryFrom}"));
            }

            return null;
        }

        private OperationResult<AvailabilityDates> CheckAvailability_MinMaxLimits(Product product, int quantity, DateTime rentalStartUtc, DateTime rentalEndUtc, MarketplaceTimeZone timeZone)
        {
            var periodInHours = Math.Round((rentalEndUtc - rentalStartUtc).TotalHours, 7);
            var rentalConfig = MaaSConfig.Current.Product.BusinessModel.Rental;

            var minPeriod = product.RentalSettings.GetMinRentalPeriodInHours();
            var maxPeriod = product.RentalSettings.GetMaxRentalPeriodInHours();

            var datesForMessage = AvailabilityDates.Unavailable(rentalStartUtc, rentalEndUtc, timeZone);
            var minErrorResult = new OperationResult<AvailabilityDates>(datesForMessage, new ErrorDictionary("", GlobalizationHelper.GetString("RENTAL_MIN_PERIOD_NOT_REACHED_MESSAGE")));
            var maxErrorResult = new OperationResult<AvailabilityDates>(datesForMessage, new ErrorDictionary("", GlobalizationHelper.GetString("RENTAL_MAX_PERIOD_EXCEEDED_MESSAGE")));

            if (product.RentalSettings.IsFixedRental)
            {
                if (!minPeriod.IsInBounds(periodInHours))
                    return minErrorResult;

                if (!maxPeriod.IsInBounds(periodInHours))
                    return maxErrorResult;
            }
            else
            {
                if (periodInHours < minPeriod.Low)
                    return minErrorResult;

                if (periodInHours > maxPeriod.High)
                    return maxErrorResult;
            }

            if (rentalConfig.EnforceWholeUnits && product.RentalSettings.RentalPeriodUnit != RentalPeriodUnit.Minute)
            {
                var unitInHours = MaaSConfig.Current.Product.BusinessModel.Rental.ConvertRentalPeriodInHours(product.RentalSettings.RentalPeriodUnit.Value, 1);
                var isMatchLow = periodInHours % unitInHours.Low == 0;
                var isMatchHigh = periodInHours % unitInHours.High == 0;

                if (!isMatchLow && !isMatchHigh)
                {
                    string errorMessageLabel;
                    switch (product.RentalSettings.RentalPeriodUnit)
                    {
                        case RentalPeriodUnit.Hour:
                            errorMessageLabel = "RENTAL_PERIOD_MUST_BE_WHOLE_HOUR";
                            break;

                        case RentalPeriodUnit.Day:
                            errorMessageLabel = "RENTAL_PERIOD_MUST_BE_WHOLE_DAY";
                            break;

                        case RentalPeriodUnit.Month:
                            errorMessageLabel = "RENTAL_PERIOD_MUST_BE_WHOLE_MONTH";
                            break;

                        default:
                            throw new ArgumentOutOfRangeException($"RentalPeriodUnit {product.RentalSettings.RentalPeriodUnit} is not handled.");
                    }

                    return new OperationResult<AvailabilityDates>(
                        AvailabilityDates.Unavailable(rentalStartUtc, rentalEndUtc, timeZone),
                        new ErrorDictionary("", GlobalizationHelper.GetString(errorMessageLabel)));
                }
            }

            return null;
        }

        private OperationResult<AvailabilityDates> CheckAvailability_Locks(Product product, int quantity, DateTime rentalStartUtc, DateTime rentalEndUtc, MarketplaceTimeZone timeZone)
        {
            rentalStartUtc = rentalStartUtc.AddMinutes(1);
            rentalEndUtc = rentalEndUtc.AddMinutes(-1);

            var locks = _rentalLockQueries.ListLocksProductRental(product.Id, product.Store_SellerId, rentalStartUtc, rentalEndUtc, timeZone);

            if (locks.Any())
            {
                var lastLockUTC = locks.OrderBy(i => i.LockEndUtc).Last().LockEndUtc;

                var earliestDate = lastLockUTC.Subtract(rentalEndUtc - rentalStartUtc);

                if (earliestDate < rentalStartUtc)
                    earliestDate = rentalStartUtc;

                var endDate = earliestDate.Add(rentalEndUtc - rentalStartUtc).AddMinutes(2);

                return SuggestAvailableDate(product, quantity, earliestDate, endDate, timeZone, 0);
            }

            return null;
        }

        private OperationResult<AvailabilityDates> SuggestAvailableDate(Product product, int quantity, DateTime rentalStartUtc, DateTime rentalEndUtc, MarketplaceTimeZone timeZone, int depth)
        {
            var locks = _rentalLockQueries.ListLocksProductRental(product.Id, product.Store_SellerId, rentalStartUtc, rentalEndUtc, timeZone);

            var msg = GlobalizationHelper.GetString("RENTAL_PERIOD_NOT_AVAILABLE_MESSAGE");

            //Stop trying after 5 retries
            if (depth >= 5)
                return new ErrorDictionary("", $"{ msg }");

            if (locks.Any())
            {
                var firstLockUTC = locks.OrderBy(i => i.LockStartUtc).First().LockStartUtc;
                var lastLockUTC = locks.OrderBy(i => i.LockEndUtc).Last().LockEndUtc;

                var newStartDate = lastLockUTC.AddMinutes(1);
                var newEndDate = newStartDate.Add(rentalEndUtc - rentalStartUtc);

                return SuggestAvailableDate(product, quantity, newStartDate, newEndDate, timeZone, depth + 1);
            }

            var dateString = GlobalizationHelper.FormatString("RENTAL_TRY_FROM_MESSAGE", FormatHelper.DisplayDateTime(rentalStartUtc, timeZone));

            return new ErrorDictionary("", $"{msg} {dateString}");
        }

        private OperationResult<AvailabilityDates> CheckAvailability_Rentals(Product product, int quantity, DateTime rentalStartUtc, DateTime rentalEndUtc, MarketplaceTimeZone timeZone, out int availableUnits, Guid? ignoreOrderProductId, Guid? ignoreOrderProductRentalInfoId, int currentStock, ProductAvailability productAvailability)
        {
            var rentalMargin = product.RentalSettings.GetRentalMarginInHours();
            var startMinUtc = rentalStartUtc.AddHours(-rentalMargin).AddSeconds(1);
            var endMaxUtc = rentalEndUtc.AddHours(rentalMargin).AddSeconds(-1);

            var rentals = _orderRepository.ListProductRentalByDates(product.Id, startMinUtc, endMaxUtc, true, ignoreOrderProductId, ignoreOrderProductRentalInfoId, productAvailability?.Id);
            availableUnits = currentStock - (int)rentals.Sum(i => i.Quantity);

            if (rentals.Any() && availableUnits <= 0)
            {
                var firstLockUTC = rentals.OrderBy(i => i.RentalStartDateTime).Last().RentalStartDateTime;
                var lastLockUTC = rentals.OrderBy(i => i.RentalEndDateTime).First().RentalEndDateTime;

                var dates = AvailabilityDates.Unavailable(rentalStartUtc, rentalEndUtc, timeZone,
                    firstLockUTC.AddHours(rentalMargin * -1), lastLockUTC.AddHours(rentalMargin));

                var msg = GlobalizationHelper.GetString("RENTAL_PERIOD_NOT_AVAILABLE_MESSAGE");

                return new OperationResult<AvailabilityDates>(
                    dates,
                    new ErrorDictionary("", $"{msg} {dates.SuggestionDateMessage(rentalStartUtc, rentalEndUtc)}"));
            }

            if (quantity < 1)
            {
                return new OperationResult<AvailabilityDates>(
                    AvailabilityDates.Unavailable(rentalStartUtc, rentalEndUtc, timeZone),
                    new ErrorDictionary("", GlobalizationHelper.GetString("RENTAL_QUANTITY_GREATER_ZERO_MESSAGE")));
            }

            if (availableUnits < quantity)
            {
                return new OperationResult<AvailabilityDates>(
                    AvailabilityDates.Unavailable(rentalStartUtc, rentalEndUtc, timeZone),
                    new ErrorDictionary("", GlobalizationHelper.GetString("RENTAL_QUANTITY_NOT_AVAILABLE_MESSAGE")));
            }

            return null;
        }

        private int GetCurrentStock(Product product, DateTime rentalStartUtc, DateTime rentalEndUtc, MarketplaceTimeZone timeZone, ProductAvailability productAvailability)
        {
            if (MaaSConfig.Current.Product.BusinessModel.Rental.ProductAvailability.Enabled &&
                product.IsAvailabilityManual)
                return productAvailability?.CurrentStock ?? 0;

            return (int)product.CurrentStock;
        }

        public OperationResult RegisterRecurringRental(OrderProduct orderProduct, bool saveChanges)
        {
            var config = MaaSConfig.Current;

            if (!config.Product.BusinessModel.Rental.Enabled ||
                !config.Product.BusinessModel.Rental.ProductAvailability.Enabled ||
                !config.Product.BusinessModel.Rental.ProductAvailability.Recurring.Enabled ||
                !orderProduct.IsPackageProduct ||
                !orderProduct.RentalInfo.ProductAvailability_Id.HasValue)
                return OperationResult.Fail("Register recurring rental not allowed.");

            var rentalInfo = orderProduct.RentalInfo;
            var productAvailabilityId = rentalInfo.ProductAvailability_Id.Value;

            var availability = _productAvailabilityService.GetById(productAvailabilityId, orderProduct.Product.Store_SellerId);

            if (!availability.IsRecurring) return OperationResult.Fail("Register recurring rental not allowed. Availability is not recurring.");

            var timeZone = _userTimeZoneProvider.GetTimeZone();

            var packageCredits = orderProduct.PackageCredits.Value;
            var rentalInfoIndex = 0;

            var rentalStartLocal = timeZone.ConvertTimeFromUtc(rentalInfo.StartDate.Value);
            var rentalEndLocal = timeZone.ConvertTimeFromUtc(rentalInfo.EndDate.Value);
            var rentalInterval = rentalInfo.EndDate.Value - rentalInfo.StartDate.Value;

            var recurringTotalDays = availability.Recurring_UnitValue *
                                     (availability.Recurring_Unit == RecurringProductAvailabilityUnit.Weeks ? 7 : 1);

            var totalDaysToGrid = packageCredits * recurringTotalDays * 3;
            var recurringEndLimit = rentalEndLocal.AddDays(totalDaysToGrid);

            var operation = GetRentalGrid(orderProduct.Product, 1, rentalStartLocal, timeZone,
                productAvailabilityId: productAvailabilityId,
                totalDaysToGrid: totalDaysToGrid);

            if (operation.IsFailure) return operation;

            var grid = operation.Result;

            int NextRentalInfoDay(DateTime rentalStart)
            {
                if (availability.Recurring_Unit == RecurringProductAvailabilityUnit.Weeks)
                {
                    var nextDayOfWeek = availability.Recurring_DaysOfWeek.FirstOrDefault(dow => dow > rentalStart.DayOfWeek);

                    if (nextDayOfWeek > rentalStart.DayOfWeek)
                        return nextDayOfWeek - rentalStart.DayOfWeek;

                    return recurringTotalDays + availability.Recurring_DaysOfWeek[0].GetHashCode() - rentalStart.DayOfWeek.GetHashCode();
                }

                return recurringTotalDays;
            }

            while (rentalInfoIndex < packageCredits && rentalStartLocal < recurringEndLimit)
            {
                var day = grid.Days.FirstOrDefault(i => i.Day == rentalStartLocal.Date);

                var period = day?.Periods.FirstOrDefault(i => i.Time == rentalStartLocal.TimeOfDay && !i.OutOfStock);

                if (period != null)
                {
                    var rentalStartUtc = timeZone.ConvertTimeToUtc(rentalStartLocal);
                    var rentalEndUtc = rentalStartUtc + rentalInterval;

                    orderProduct.NewRental(rentalStartUtc, rentalEndUtc, productAvailabilityId);

                    rentalInfoIndex++;
                }

                var nextRentalInfoDay = NextRentalInfoDay(rentalStartLocal);

                rentalStartLocal = rentalStartLocal.AddDays(nextRentalInfoDay);
            }

            _orderProductRepository.Update(orderProduct);

            if (saveChanges) _orderProductRepository.SaveChanges();

            return OperationResult.Success;
        }
    }
}