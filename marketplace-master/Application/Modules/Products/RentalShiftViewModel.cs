using Domain.StoreModule.Interfaces;
using System;

namespace Application.Modules.Products
{
    public class RentalShiftViewModel
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string StartTime { get; set; }
        public string EndTime { get; set; }
        public bool Selected { get; set; }
        public int DurationInMinutes { get; set; }

        public static RentalShiftViewModel From(RentalShiftDto rentalShiftDto) => new RentalShiftViewModel
        {
            Id = rentalShiftDto.Id,
            Name = rentalShiftDto.Name,
            StartTime = new DateTime(1, 1, 1, rentalShiftDto.StartHours, rentalShiftDto.StartMinutes, 0).ToShortTimeString(),
            EndTime = new DateTime(1, 1, 1, rentalShiftDto.EndHours, rentalShiftDto.EndMinutes, 0).ToShortTimeString(),
            DurationInMinutes = (int)(
                new DateTime(1, 1, 1, rentalShiftDto.EndHours, rentalShiftDto.EndMinutes, 0) -
                new DateTime(1, 1, 1, rentalShiftDto.StartHours, rentalShiftDto.StartMinutes, 0)).TotalMinutes
        };
    }
}
