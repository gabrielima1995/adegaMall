using Domain.Entities;
using Framework;
using System;

namespace Domain.StoreModule.Entities
{
    public class RentalShift : SoftDeletableEntity<Guid>
    {
        private RentalShift()
        {
        }

        private RentalShift(Guid id, string name, RentalShiftTime start, RentalShiftTime end)
        {
            Id = id;
            Name = name;
            Start = start;
            End = end;
        }

        public string Name { get; private set; }
        public RentalShiftTime Start { get; private set; }
        public RentalShiftTime End { get; private set; }

        public static OperationResult<RentalShift> Create(Guid id, string name, RentalShiftTime start, RentalShiftTime end)
        {
            if (id == Guid.Empty)
                return new OperationResult<RentalShift>(nameof(id), "Id is mandatory.");

            if (string.IsNullOrEmpty(name))
                return new OperationResult<RentalShift>(nameof(name), "Name is mandatory.");

            if (start > end)
                return new OperationResult<RentalShift>(nameof(start), "Cannot start after the end time.");

            return new OperationResult<RentalShift>(new RentalShift(id, name, start, end));
        }

        public OperationResult SetName(string name)
        {
            if (string.IsNullOrEmpty(name))
                return new OperationResult(nameof(name), "Name is mandatory.");

            Name = name;
            return OperationResult.Success;
        }

        public OperationResult SetStartTime(RentalShiftTime start)
        {
            if (start > End)
                return OperationResult.Fail("Cannot start after the end.");

            Start.UpdateValues(start);
            return OperationResult.Success;
        }

        public OperationResult SetEndTime(RentalShiftTime end)
        {
            if (end < Start)
                return OperationResult.Fail("Cannot end before the start.");

            End.UpdateValues(end);
            return OperationResult.Success;
        }
    }
}
