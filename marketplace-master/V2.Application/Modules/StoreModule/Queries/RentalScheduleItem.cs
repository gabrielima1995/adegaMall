using Application.Config;
using Domain.DTO;
using Domain.Entities.StoreModule;
using Framework;
using Infra.Crosscutting.Globalization;
using Newtonsoft.Json;
using System;
using System.Globalization;

namespace V2.Application.Modules.StoreModule.Queries
{
    public class RentalScheduleItem
    {
        [JsonProperty("id")]
        public Guid Id { get; private set; }

        [JsonProperty("resourceId")]
        public string ResourceId { get; private set; }

        [JsonProperty("friendlyUrl")]
        public string FriendlyUrl { get; private set; }

        [JsonProperty("productAvailability")]
        public string ProductAvailability { get; private set; }

        [JsonProperty("outOfStock")]
        public bool OutOfStock { get; private set; }

        [JsonProperty("start")]
        public DateTime Start { get; private set; }

        [JsonProperty("start_utc")]
        public DateTime StartUTC { get; private set; }

        [JsonProperty("end")]
        public DateTime End { get; private set; }

        [JsonProperty("end_utc")]
        public DateTime EndUTC { get; private set; }

        [JsonProperty("block")]
        public bool Block { get; private set; }

        [JsonProperty("title")]
        public string Title { get; private set; }

        [JsonProperty("tooltip_title")]
        public string TooltipTitle { get; private set; }

        [JsonProperty("description")]
        public string Description { get; private set; }

        [JsonProperty("color")]
        public string Color { get; private set; }

        [JsonProperty("editable")]
        public bool Editable { get; private set; }

        public RentalScheduleItem(RentalForScheduleDto item, MarketplaceTimeZone timeZone, bool isScheduleSeller = true)
        {
            var resourceId = item.ProductId.ToString();
            var productAvailability = item.ProductAvailabilityId?.ToString();
            var productTitle = item.ProductTitle;
            var seller = item.SellerName;
            var buyer = item.BuyerName;
            var status = item.Status.GetDescriptionLocalized();
            var rentalPeriodDescription = GetRentalPeriodDescription(item.StartDateTime, item.EndDateTime, timeZone);

            if (MaaSConfig.Current.Product.BusinessModel.Rental.QuantityEnabled)
            {
                productTitle = item.Quantity.ToString("N0", CultureInfo.CurrentCulture) + "x " + productTitle;
            }

            string GetDescription()
            {
                if (isScheduleSeller)
                {
                    return GlobalizationHelper.FormatString("SCHEDULE_ITEM_RENTAL_DESCRIPTION",
                        buyer,
                        productTitle,
                        status,
                        rentalPeriodDescription);
                }

                return GlobalizationHelper.FormatString("SCHEDULE_BUYER_ITEM_RENTAL_DESCRIPTION",
                    seller,
                    productTitle,
                    status,
                    rentalPeriodDescription);
            }

            string GetColor()
            {
                // entrega atrasada
                if ((item.Status == OrderStatus.AwaitingDelivery || item.Status == OrderStatus.AwaitingPickup) &&
                    item.StartDateTime <= DateTime.UtcNow)
                {
                    return "#d58512";
                }

                // devolução atrasada
                if (item.Status == OrderStatus.Delivered && item.EndDateTime <= DateTime.UtcNow)
                {
                    return "#ac2925";
                }

                return "#398439";  // ações em dia
            }

            var title = GlobalizationHelper.FormatString("SCHEDULE_ITEM_RENTAL_TITLE",
                item.Id.ToString().Substring(0, 8));

            var description = GetDescription();

            var color = GetColor();

            Load(item.Id, resourceId, string.Empty, productAvailability, false,
                item.StartDateTime, item.EndDateTime, timeZone,
                title, title, description, color,
                false);
        }

        public RentalScheduleItem(RentalLockForScheduleDto item, MarketplaceTimeZone timeZone, DateTime startUtc, DateTime endUtc)
        {
            var resourceId = item.ProductId?.ToString() ?? "store";
            var rentalPeriodDescription = GetRentalPeriodDescription(startUtc, endUtc, timeZone);

            string GetDescription()
            {
                return GlobalizationHelper.FormatString("SCHEDULE_ITEM_LOCK_DESCRIPTION",
                    !string.IsNullOrEmpty(item.ProductTitle)
                        ? item.ProductTitle
                        : GlobalizationHelper.GetString("SCHEDULE_ITEM_LOCK_WHOLE_STORE"),
                    rentalPeriodDescription);
            }

            var title = item.Mode.GetDescriptionLocalized();

            var description = GetDescription();

            const string color = "#888888";

            Load(item.Id, resourceId, string.Empty, null, false,
                startUtc, endUtc, timeZone,
                title, title, description, color,
                true);
        }

        public RentalScheduleItem(Product product,
            RentalDayGridView day, RentalPeriodGridView period,
            MarketplaceTimeZone timeZone, bool shortData)
        {
            var resourceId = product.Id.ToString();
            var friendlyUrl = product.FriendlyUrl;
            var productAvailability = period.Availability?.ToString();

            var start = timeZone.DateTimeWithZone(day.Day.Add(period.Time));
            var end = timeZone.DateTimeWithZone(product.RentalSettings.CalculateEndDateTime(start.LocalTime).Value);

            var startUtc = start.UniversalTime;
            var endUtc = end.UniversalTime;

            var periodDescription = $"{FormatHelper.DisplayTime(startUtc, timeZone)} - {FormatHelper.DisplayTime(endUtc, timeZone)}"; ;

            string GetDescription()
            {
                string desc;

                if (!shortData)
                {
                    desc = GlobalizationHelper.FormatString(
                        "SCHEDULE_TIMELINE_ITEM_DESCRIPTION_1",
                        period.OriginalStock - period.CurrentStock);
                }
                else
                {
                    desc = periodDescription;
                }

                if (period.OutOfStock)
                {
                    desc += $"{(desc == string.Empty ? string.Empty : "<br>")}{GlobalizationHelper.GetString("SCHEDULE_TIMELINE_ITEM_DESCRIPTION_3")}";
                }
                else if (!shortData)
                {
                    desc += $"{(desc == string.Empty ? string.Empty : "<br>")}{GlobalizationHelper.FormatString("SCHEDULE_TIMELINE_ITEM_DESCRIPTION_2", period.OriginalStock, period.CurrentStock)}";
                }

                // tem locações agendadas
                if (!shortData && period.UnavailableStock > 0)
                {
                    desc += $"{(desc == string.Empty ? string.Empty : "<br>")}{GlobalizationHelper.GetString("SCHEDULE_TIMELINE_ITEM_DESCRIPTION_4")}";
                }

                return desc;
            }

            string GetColor()
            {
                if (period.OriginalStock == period.CurrentStock)
                    return "#3788d8"; // todas as unidades disponíveis

                if (period.OutOfStock)
                    return shortData ? "#BBBBBB" : "#d58512"; // sem unidades disponíveis

                return "#398439"; // com unidades disponíveis
            }

            var title = $"{period.UnavailableStock}/{period.OriginalStock}";

            var tooltipTitle = shortData
                ? string.Empty
                : GlobalizationHelper.FormatString(
                    "SCHEDULE_TIMELINE_ITEM_TOOLTIP_TITLE",
                    periodDescription,
                    title);

            var description = GetDescription();

            var color = GetColor();

            Load(Guid.Empty, resourceId, friendlyUrl, productAvailability, period.OutOfStock,
                startUtc, endUtc, timeZone,
                title, tooltipTitle, description, color,
                false);
        }

        private string GetRentalPeriodDescription(DateTime startUtc, DateTime endUtc, MarketplaceTimeZone timeZone)
        {
            return $"{FormatHelper.DisplayDateTime(startUtc, timeZone)} - {FormatHelper.DisplayDateTime(endUtc, timeZone)}";
        }

        private void Load(Guid id, string resourceId, string friendlyUrl, string productAvailability, bool outOfStock,
            DateTime startUtc, DateTime endUtc, MarketplaceTimeZone timeZone,
            string title, string tooltipTitle, string description, string color,
            bool block)
        {
            Id = id;
            ResourceId = resourceId;
            FriendlyUrl = friendlyUrl;
            ProductAvailability = productAvailability;
            OutOfStock = outOfStock;
            StartUTC = startUtc;
            EndUTC = endUtc;
            Block = block;
            Title = title;
            TooltipTitle = tooltipTitle;
            Description = description;
            Color = color;

            Start = timeZone.ConvertTimeFromUtc(StartUTC);
            End = timeZone.ConvertTimeFromUtc(EndUTC);
        }

        /// <summary>
        /// Verifica se o item está dentro de um determinado período
        /// </summary>
        /// <param name="startUtc">Data inicial do periodo, em UTC</param>
        /// <param name="endUtc">Data final do período, em UTC</param>
        /// <returns></returns>
        public bool Intersects(DateTime startUtc, DateTime endUtc)
        {
            return (startUtc <= StartUTC && StartUTC <= endUtc) ||  // data inicial dentro do periodo OU
                   (startUtc <= EndUTC && EndUTC <= endUtc) ||      // data final dentro do periodo OU
                   (StartUTC <= startUtc && endUtc <= EndUTC);     // data inicial anterior ao periodo E data final posterior ao período
        }
    }
}