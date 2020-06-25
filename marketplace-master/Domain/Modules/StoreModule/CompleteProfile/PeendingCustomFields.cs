using System;

namespace Domain.Modules.StoreModule.CompleteProfile
{
    public class PeendingCustomFields
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public string ErrorMessageLabel { get; set; }

        public PeendingCustomFields(Guid id, string name, string errorMessageLabel)
        {
            Id = id;
            Name = name;
            ErrorMessageLabel = errorMessageLabel;
        }
    }
}