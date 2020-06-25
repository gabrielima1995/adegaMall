using System.Collections.Generic;

namespace Domain.Modules.StoreModule.CompleteProfile
{
    public class Registration
    {
        public bool IsComplete { get; set; } = false;

        public bool HasAvatar { get; set; }
        public bool NeedsAvatar { get; set; } = false;

        public bool HasWhatsAppNumber { get; set; }
        public bool NeedsWhatsAppNumber { get; set; } = false;

        public bool HasAddress { get; set; } = false;
        public bool NeedsAddress { get; set; } = false;

        public bool HasDocumentNumber { get; set; } = false;
        public bool NeedsDocumentNumber { get; set; } = false;

        public bool HasDateOfBirth { get; set; } = false;
        public bool NeedsDateOfBirth { get; set; } = false;

        public List<PendingCustomField> PendingCustomFields { get; set; } = new List<PendingCustomField>();
    }
}
