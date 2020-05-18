namespace Domain.Config.Modules
{
    public class LeadSalesConfig
    {
        public bool Enabled { get; set; } = false;

        public bool CreditsExpire { get; set; } = false;

        public IndividualPacks IndividualPacks { get; set; } = new IndividualPacks();

        public int CreditsDefaultDuration { get; set; } = 31;

        public LeadSalesFieldsConfig HiddenFields { get; set; } = new LeadSalesFieldsConfig();
    }

    public class IndividualPacks
    {
        public bool Enabled { get; set; } = true;

        public bool ShowPricePerCredit { get; set; } = true;
    }

    public class LeadSalesFieldsConfig
    {
        public bool Description { get; set; } = true;

        public bool Name { get; set; } = true;

        public bool Address { get; set; } = true;

        public bool Email { get; set; } = true;

        public bool Amount { get; set; } = false;

        public bool Phone { get; set; } = true;

        public bool Avatar { get; set; } = true;

        public bool Title { get; set; } = false;

        public bool Categories { get; set; } = false;

        public bool Date { get; set; } = false;

        public bool CustomFields { get; set; } = true;
    }
}
