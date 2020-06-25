namespace Domain.Config.Sections
{
    public class RatingsConfig
    {
        public bool AdminCanDeleteRatings { get; set; } = true;
        public bool DoubleBlindRatings { get; set; } = false;
        public int MaxWaitDays { get; set; } = 14;
    }
}
