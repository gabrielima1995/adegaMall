namespace Framework
{
    public static class AppRole
    {
        public const string GestaoIna = "GestaoINA";
        public const string Admin = "Admin";
        public const string Buyer = "Buyer";
        public const string Seller = "Seller";
        public const string SellerAssistant = "SellerAssistant";

        public static string[] AllRoles => new[] { GestaoIna, Admin, Buyer, Seller, SellerAssistant };
    }
}
