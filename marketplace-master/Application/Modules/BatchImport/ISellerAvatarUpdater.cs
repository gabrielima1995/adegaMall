namespace Application.Modules.BatchImport
{
    public interface ISellerAvatarUpdater
    {
        void UpdateSellerAvatar(string sellerId, string avatarUrl);
    }
}
