using Domain.StoreModule.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Presentation.Areas.Admin.Models
{
    public class RentalShiftsIndexViewModel
    {
        public RentalShiftsIndexViewModel(IReadOnlyList<RentalShiftDto> rentalShiftItems)
        {
            RentalShiftItems = rentalShiftItems.Select(dto => ShiftViewModel.From(dto)).ToList();
        }

        public IReadOnlyList<ShiftViewModel> RentalShiftItems { get; private set; }

        public class ShiftViewModel
        {
            public Guid Id { get; set; }
            public string Name { get; set; }
            public string FormattedStartTime { get; set; }
            public string FormattedEndTime { get; set; }

            public static ShiftViewModel From(RentalShiftDto dto)
            {
                return new ShiftViewModel
                {
                    Id = dto.Id,
                    Name = dto.Name,
                    FormattedStartTime = new DateTime(1, 1, 1, dto.StartHours, dto.StartMinutes, 0).ToShortTimeString(),
                    FormattedEndTime = new DateTime(1, 1, 1, dto.EndHours, dto.EndMinutes, 0).ToShortTimeString()
                };
            }
        }
    }
}