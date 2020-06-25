using Domain.Entities;
using Domain.Entities.StoreModule;
using Domain.Modules.StoreModule.Commands;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace Domain.Modules.StoreModule.Entities
{
    public class ProductAvailability : SoftDeletableEntity<Guid>
    {
        public Guid Product_Id { get; protected set; }

        [Required]
        [ForeignKey("Product_Id")]
        public Product Product { get; protected set; }

        public ProductAvailabilityType Type { get; protected set; }

        public DayOfWeek DayOfWeek { get; protected set; }

        [NotMapped]
        public DayOfWeek[] Recurring_DaysOfWeek
        {
            get
            {
#pragma warning disable S2365 // Properties should not make collection or array copies
                if (string.IsNullOrEmpty(Recurring_DaysOfWeekInline)) return Array.Empty<DayOfWeek>();

                return Array
                    .ConvertAll(Recurring_DaysOfWeekInline.Split(';'), input => Enum.Parse(typeof(DayOfWeek), input))
                    .Cast<DayOfWeek>()
                    .ToArray();
#pragma warning restore S2365 // Properties should not make collection or array copies
            }
            set
            {
                var v = value;

                Recurring_DaysOfWeekInline = v == null || v.Length == 0
                    ? null
                    : string.Join(";", v.Select(p => p.GetHashCode()));
            }
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public string Recurring_DaysOfWeekInline { get; set; }

        public RecurringProductAvailabilityUnit Recurring_Unit { get; protected set; }

        public int Recurring_UnitValue { get; protected set; }

        [Required]
        public TimeSpan StartTime { get; protected set; }

        [Required]
        public TimeSpan EndTime { get; protected set; }

        public int CurrentStock { get; protected set; }

        [NotMapped]
        public int UpdatedStock { get; protected set; }

        public int DiscountPercentage { get; protected set; }

        public bool HasStock => CurrentStock > 0;
        public bool HasDiscount => DiscountPercentage > 0;
        public bool IsRecurring => Type == ProductAvailabilityType.Recurring;

        public static ProductAvailability New(NewProductAvailabilityCommand command)
        {
            var item = new ProductAvailability
            {
                Id = command.AvailabilityId,
                Product_Id = command.ProductId,
                Type = ProductAvailabilityType.Default,
                DayOfWeek = command.DayOfWeek,
                StartTime = command.StartTime,
                EndTime = command.EndTime,
                CurrentStock = command.CurrentStock,
                DiscountPercentage = command.DiscountPercentage
            };

            item.AddDomainEvent(new ProductAvailabilityAdded(item));

            return item;
        }

        public ProductAvailability Update(EditProductAvailabilityCommand command)
        {
            CurrentStock = command.CurrentStock;
            DiscountPercentage = command.DiscountPercentage;

            AddDomainEvent(new ProductAvailabilityUpdated(this));

            return this;
        }

        public static ProductAvailability New(NewRecurringProductAvailabilityCommand command)
        {
            var item = new ProductAvailability
            {
                Id = command.AvailabilityId,
                Product_Id = command.ProductId,
                Type = ProductAvailabilityType.Recurring,
                Recurring_DaysOfWeek = command.DaysOfWeek,
                Recurring_Unit = command.Unit,
                Recurring_UnitValue = command.UnitValue,
                StartTime = command.StartTime,
                EndTime = command.EndTime,
                CurrentStock = command.CurrentStock,
                DiscountPercentage = command.DiscountPercentage
            };

            item.AddDomainEvent(new ProductAvailabilityAdded(item));

            return item;
        }

        public ProductAvailability Update(EditRecurringProductAvailabilityCommand command)
        {
            CurrentStock = command.CurrentStock;
            DiscountPercentage = command.DiscountPercentage;

            AddDomainEvent(new ProductAvailabilityUpdated(this));

            return this;
        }

        public ProductAvailability Delete()
        {
            Deleted = true;
            DeletedAt = DateTime.UtcNow;

            AddDomainEvent(new ProductAvailabilityDeleted(this));

            return this;
        }

        public bool InDayOfWeek(DayOfWeek dayOfWeek)
        {
            // disponibilidade recorrente
            if (IsRecurring)
            {
                if (Recurring_Unit == RecurringProductAvailabilityUnit.Weeks)
                    return Recurring_DaysOfWeek.Contains(dayOfWeek);

                return true;
            }

            // disponibilidade livre por dia da semana
            return DayOfWeek == dayOfWeek;
        }
    }
}