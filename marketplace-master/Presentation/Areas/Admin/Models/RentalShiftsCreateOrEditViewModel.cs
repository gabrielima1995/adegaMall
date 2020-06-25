using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Presentation.Areas.Admin.Models
{
    public class RentalShiftsCreateOrEditViewModel : IValidatableObject
    {
        public Guid? Id { get; set; }

        [Required]
        [MaxLength(50)]
        public string Name { get; set; }

        [Required]
        [RegularExpression(@"\d{1,2}\:\d{2}", ErrorMessage = "Informe o início no formato hh:mm.")]
        public string StartTime { get; set; }

        [Required]
        [RegularExpression(@"\d{1,2}\:\d{2}", ErrorMessage = "Informe o fim no formato hh:mm.")]
        public string EndTime { get; set; }

        public bool TryParseStartTime(out int startHours, out int startMinutes)
            => TryParseTime(StartTime, out startHours, out startMinutes);

        public bool TryParseEndTime(out int endHours, out int endMinutes)
            => TryParseTime(EndTime, out endHours, out endMinutes);

        private bool TryParseTime(string timeString, out int hours, out int minutes)
        {
            hours = minutes = 0;

            if (string.IsNullOrEmpty(timeString))
                return false;

            if (!timeString.Contains(":"))
                return false;

            var parts = timeString.Split(':');

            if (!int.TryParse(parts[0], out hours))
                return false;

            if (!int.TryParse(parts[1], out minutes))
                return false;

            if (hours < 0 || hours > 23)
            {
                hours = minutes = 0;
                return false;
            }

            if (minutes < 0 || minutes > 59)
            {
                hours = minutes = 0;
                return false;
            }

            return true;
        }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (!TryParseStartTime(out int startHours, out int startMinutes))
                yield return new ValidationResult("Horário de início inválido", new[] { nameof(StartTime) });

            if (!TryParseEndTime(out int endHours, out int endMinutes))
                yield return new ValidationResult("Horário de término inválido", new[] { nameof(EndTime) });

            var startTime = new DateTime(1, 1, 1, startHours, startMinutes, 0);
            var endTime = new DateTime(1, 1, 1, endHours, endMinutes, 0);

            if (startTime >= endTime)
                yield return new ValidationResult("Horários inválidos, não é possível terminar antes de começar", new[] { nameof(StartTime) });
        }
    }
}