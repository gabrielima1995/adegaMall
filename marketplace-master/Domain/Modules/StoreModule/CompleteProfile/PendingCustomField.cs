using System;

namespace Domain.Modules.StoreModule.CompleteProfile
{
    public class PendingCustomField
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public string ErrorMessageLabel { get; set; }

        public PendingCustomField(Guid id, string name, string errorMessageLabel)
        {
            Id = id;
            Name = name;
            ErrorMessageLabel = errorMessageLabel;
        }
    }
}