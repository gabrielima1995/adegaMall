using Application.Config.Common;

namespace Application.Config.Registers
{
    public class DocumentNumberConfig : PublicProfileRegisterConfig
    {
        public bool AllowCPF { get; set; } = true;

        public bool AllowCNPJ { get; set; } = true;

        public bool AllowFreeText { get; set; } = false;

        public bool BlockChangesAfterApprovedInModeration { get; set; } = false;

        public DocumentNumberConfig()
            : base(false, true, true)
        {

        }
    }
}
