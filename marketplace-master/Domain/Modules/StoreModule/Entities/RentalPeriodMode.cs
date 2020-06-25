using Framework;
using System;
using System.Linq;

namespace Domain.StoreModule.Entities
{
    public class RentalPeriodMode
    {
        private RentalPeriodMode() { }
        private RentalPeriodMode(string name) => Name = name;

        public string Name { get; private set; }

        public static RentalPeriodMode Unrestricted => new RentalPeriodMode("unrestricted");
        public static RentalPeriodMode Shifts => new RentalPeriodMode("shifts");

        public static OperationResult<RentalPeriodMode> Create(string name)
        {
            var possibleValues = new string[]
            {
                Unrestricted, Shifts
            };

            if (!possibleValues.Any(value => value == name))
                return OperationResult<RentalPeriodMode>.Fail("", $"Rental period mode '{name}' is not valid. Choose from: " + string.Join(", ", possibleValues));

            return new RentalPeriodMode(name);
        }

        public static implicit operator string(RentalPeriodMode mode) => mode.Name;
        public static implicit operator RentalPeriodMode(string name) { var result = Create(name); if (result.IsFailure) throw new Exception(result.Errors.ToSimpleString()); return result.Result; }
        public static bool operator ==(RentalPeriodMode a, RentalPeriodMode b) => a?.Equals(b) ?? false;
        public static bool operator !=(RentalPeriodMode a, RentalPeriodMode b) => !a?.Equals(b) ?? true;

        public override bool Equals(object obj)
        {
            var other = obj as RentalPeriodMode;

            if (other is null)
                return false;

            return Name == other.Name;
        }

        public override int GetHashCode() => Name.GetHashCode();

        public override string ToString() => Name;
    }
}
