namespace Domain.Entities
{
    /// <summary>
    /// Rede social
    /// </summary>
    public class SocialNetwork : BaseEntity<int>
    {
        public string Name { get; set; }

        public string Image { get; set; }
    }
}