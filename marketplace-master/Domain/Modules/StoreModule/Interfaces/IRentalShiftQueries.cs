using System;
using System.Collections.Generic;

namespace Domain.StoreModule.Interfaces
{
    public interface IRentalShiftQueries
    {
        IReadOnlyList<RentalShiftDto> ListRentalShifts();

        IReadOnlyList<ProductRentalShiftDto> ListRentalShiftsForAllProducts();

        IReadOnlyList<RentalShiftDto> ListProductShifts(Guid productId);
    }

    public class ProductRentalShiftDto : RentalShiftDto
    {
        private ProductRentalShiftDto(/* Empty constructor for reflection */) : base() { }

        public ProductRentalShiftDto(Guid id, Guid productId, string name, int startHours, int startMinutes, int endHours, int endMinutes)
            : base(id, name, startHours, startMinutes, endHours, endMinutes)
        {
            ProductId = productId;
        }

        public Guid ProductId { get; private set; }
    }

    public class RentalShiftDto
    {
        public Guid Id { get; private set; }
        public string Name { get; private set; }
        public int StartHours { get; private set; }
        public int StartMinutes { get; private set; }
        public int EndHours { get; private set; }
        public int EndMinutes { get; private set; }

        protected RentalShiftDto(/* Empty constructor for reflection */) { }

        protected RentalShiftDto(Guid id, string name, int startHours, int startMinutes, int endHours, int endMinutes)
        {
            Id = id;
            Name = name;
            StartHours = startHours;
            StartMinutes = startMinutes;
            EndHours = endHours;
            EndMinutes = endMinutes;
        }

        public int DurationMinutes => (int)(
            new DateTime(1, 1, 1, EndHours, EndMinutes, 0) -
            new DateTime(1, 1, 1, StartHours, StartMinutes, 0)).TotalMinutes;

        public override bool Equals(object obj)
        {
            if (obj == null || obj.GetType() != GetType())
                return false;

            return ((RentalShiftDto)obj).Id == Id;
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }

        public override string ToString()
        {
            return $"{Name} {StartHours}:{StartMinutes} - {EndHours}:{EndMinutes}";
        }
    }
}
