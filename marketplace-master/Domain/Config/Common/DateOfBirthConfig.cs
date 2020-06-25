namespace Application.Config.Common
{
    public class DateOfBirthConfig : RegisterConfig
    {
        public DateOfBirthConfig(bool enabled = true, bool mandatory = true, bool blockChangesAfterApprovedInModeration = false) : base(enabled, mandatory)
        {
            this.BlockChangesAfterApprovedInModeration = blockChangesAfterApprovedInModeration;
        }

        public int MinAge { get; set; } = 0;
        public int MaxAge { get; set; } = 100;
        public bool BlockChangesAfterApprovedInModeration { get; set; } = false;
    }
}