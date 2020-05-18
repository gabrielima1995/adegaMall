namespace Domain.Entities
{
    public class UserAndProfile
    {
        public UserAndProfile(ApplicationUser user, Person profile)
        {
            User = user;
            Profile = profile;
        }

        public ApplicationUser User { get; }
        public Person Profile { get; }
    }

    public class UserAndProfile<TPerson> : UserAndProfile where TPerson : Person
    {
        public UserAndProfile(ApplicationUser user, TPerson profile) : base(user, profile) { }
        public new TPerson Profile => base.Profile as TPerson;
    }

    public class AdminUserAndProfile : UserAndProfile<AdminPerson>
    {
        public AdminUserAndProfile(ApplicationUser user, AdminPerson profile) : base(user, profile) { }

        public static AdminUserAndProfile Create(string userId, string email, string fullName) =>
            new AdminUserAndProfile(
                new ApplicationUser(userId, email),
                new AdminPerson(userId, email, fullName));

        public static AdminUserAndProfile AddFrom(ApplicationUser user, string fullName) =>
            new AdminUserAndProfile(
                user,
                new AdminPerson(user.Id, user.Email, fullName));
    }

    public class SellerAssistantUserAndProfile : UserAndProfile<SellerAssistant>
    {
        public SellerAssistantUserAndProfile(ApplicationUser user, SellerAssistant profile) : base(user, profile) { }

        public static SellerAssistantUserAndProfile Create(string userId, string email, string sellerId) =>
            new SellerAssistantUserAndProfile(
                new ApplicationUser(userId, email),
                new SellerAssistant(userId, email, sellerId));

        public static SellerAssistantUserAndProfile AddFrom(ApplicationUser user, string sellerId) =>
            new SellerAssistantUserAndProfile(
                user,
                new SellerAssistant(user.Id, user.Email, sellerId));
    }

    public class SellerUserAndProfile : UserAndProfile<Seller>
    {
        public SellerUserAndProfile(ApplicationUser user, Seller profile) : base(user, profile) { }

        public static SellerUserAndProfile Create(string userId, string email, bool requireModeration) =>
            new SellerUserAndProfile(
                new ApplicationUser(userId, email),
                new Seller(userId, email, requireModeration));

        public static SellerUserAndProfile AddFromUser(ApplicationUser user, bool requireModeration) =>
            new SellerUserAndProfile(
                user,
                new Seller(user.Id, user.Email, requireModeration));
    }

    public class BuyerUserAndProfile : UserAndProfile<Buyer>
    {
        public BuyerUserAndProfile(ApplicationUser user, Buyer profile) : base(user, profile) { }

        public static BuyerUserAndProfile Create(string userId, string email, string fullName, bool requireModeration) =>
            new BuyerUserAndProfile(
                new ApplicationUser(userId, email),
                new Buyer(userId, email, fullName, requireModeration));

        public static BuyerUserAndProfile AddFromUser(ApplicationUser user, string fullName, bool requireModeration) =>
            new BuyerUserAndProfile(
                user,
                new Buyer(user.Id, user.Email, fullName, requireModeration));
    }
}
