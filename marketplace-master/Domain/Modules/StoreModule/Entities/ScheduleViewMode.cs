using Framework;
using System;
using System.Linq;

namespace Domain.Modules.StoreModule.Entities
{
    public sealed class ScheduleViewMode
    {
        private ScheduleViewMode()
        {
        }

        private ScheduleViewMode(string name) : this() => Name = name;

        public string Name { get; private set; }

        public static ScheduleViewMode Default => new ScheduleViewMode("default");
        public static ScheduleViewMode Timeline => new ScheduleViewMode("timeline");

        public static OperationResult<ScheduleViewMode> Create(string name)
        {
            var possibleValues = new string[]
            {
                Default, Timeline
            };

            return possibleValues.All(value => value != name)
                ? OperationResult<ScheduleViewMode>.Fail("", $"Schedule view mode '{name}' is not valid. Choose from: " + string.Join(", ", possibleValues))
                : new ScheduleViewMode(name);
        }

        public static implicit operator string(ScheduleViewMode mode) => mode.Name;

        public static implicit operator ScheduleViewMode(string name)
        {
            var result = Create(name); if (result.IsFailure) throw new Exception(result.Errors.ToSimpleString()); return result.Result;
        }

        public static bool operator ==(ScheduleViewMode a, ScheduleViewMode b) => a?.Equals(b) ?? false;

        public static bool operator !=(ScheduleViewMode a, ScheduleViewMode b) => !a?.Equals(b) ?? true;

        public override bool Equals(object obj)
        {
            var other = obj as ScheduleViewMode;

            if (other is null)
                return false;

            return Name == other.Name;
        }

        public override int GetHashCode() => Name.GetHashCode();

        public override string ToString() => Name;
    }
}