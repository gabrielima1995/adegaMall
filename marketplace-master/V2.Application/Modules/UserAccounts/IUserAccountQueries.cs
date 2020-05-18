namespace V2.Application.Modules.UserAccounts
{
    public interface IUserAccountQueries
    {
        BasicAccountInfo GetBasicInfo(string userId);
    }

    public class BasicAccountInfo
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string AlternativeName { get; set; }
        public string Email { get; set; }
        public bool IsEnabled { get; set; }
        public bool IsBuyer { get; set; }
        public bool IsSeller { get; set; }
        public string AvatarUrl { get; set; }
    }
}
