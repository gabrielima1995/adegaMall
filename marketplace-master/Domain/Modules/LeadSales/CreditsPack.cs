using System;

namespace Domain.Entities
{
    public class CreditsPack : BaseEntity<Guid>
    {
        public string Name { get; set; }

        public string Name_EN { get; set; }

        public string Description { get; set; }

        public string Description_EN { get; set; }

        public int CreditsAmount { get; set; }

        public decimal Price { get; set; }

        public int? ValidityDays { get; set; }

        public CreditsPack()
        {
            Id = Guid.NewGuid();
        }

        public string NameLocalized(string lang)
        {
            if (lang.Equals("en-us", StringComparison.OrdinalIgnoreCase) && !string.IsNullOrWhiteSpace(Name_EN))
                return Name_EN;

            return Name;
        }

        public string DescriptionLocalized(string lang)
        {
            if (lang.Equals("en-us", StringComparison.OrdinalIgnoreCase) && !string.IsNullOrWhiteSpace(Description_EN))
                return Description_EN;

            return Description;
        }
    }
}
