using Domain.Core;
using Domain.Events.StoreModule;
using Domain.StoreModule.Entities;
using Domain.StoreModule.Interfaces;
using Framework;
using System;

namespace Domain.StoreModule.Commands
{
    public class UpdateRentalShiftCommand : Command
    {
        public UpdateRentalShiftCommand(Guid id, string name, int startHours, int startMinutes, int endHours, int endMinutes) : base(id.ToString())
        {
            Id = id;
            Name = name;
            StartHours = startHours;
            StartMinutes = startMinutes;
            EndHours = endHours;
            EndMinutes = endMinutes;
        }

        public Guid Id { get; }
        public string Name { get; }
        public int StartHours { get; }
        public int StartMinutes { get; }
        public int EndHours { get; }
        public int EndMinutes { get; }
    }

    public class UpdateRentalShiftCommandHandler
    {
        private readonly IRentalShiftRepository rentalShiftRepository;

        public UpdateRentalShiftCommandHandler(IRentalShiftRepository rentalShiftRepository)
        {
            this.rentalShiftRepository = rentalShiftRepository;
        }

        public OperationResult Handle(UpdateRentalShiftCommand command)
        {
            var rentalShift = rentalShiftRepository.Get(command.Id);

            if (rentalShift == null)
                return OperationResult.Fail("", $"Rental shift id '{command.Id}' was not found.");

            var errors = new ErrorDictionary();

            var startTimeResult = RentalShiftTime.Create(command.StartHours, command.StartMinutes);
            if (startTimeResult.IsFailure)
                errors.Merge(startTimeResult.Errors);

            var endTimeResult = RentalShiftTime.Create(command.EndHours, command.EndMinutes);
            if (endTimeResult.IsFailure)
                errors.Merge(endTimeResult.Errors);

            if (errors.HasErrors)
                return new OperationResult(errors);

            var setNameResult = rentalShift.SetName(command.Name);
            if (setNameResult.IsFailure)
                errors.Merge(setNameResult.Errors);

            var setStartTimeResult = rentalShift.SetStartTime(startTimeResult.Result);
            if (setStartTimeResult.IsFailure)
                errors.Merge(setStartTimeResult.Errors);

            var setEndTimeResult = rentalShift.SetEndTime(endTimeResult.Result);
            if (setEndTimeResult.IsFailure)
                errors.Merge(setEndTimeResult.Errors);

            if (errors.HasErrors)
                return new OperationResult(errors);

            rentalShift.AddDomainEvent(new RentalShiftUpdatedEvent(rentalShift));

            return rentalShiftRepository.UpdateRentalShift(rentalShift);
        }
    }
}
