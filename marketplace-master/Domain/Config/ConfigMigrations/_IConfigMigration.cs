using Application.Config;

namespace Domain.Config.ConfigMigrations
{
    internal interface IConfigMigration
    {
        int ExpectedVersion { get; }
        int NewVersion { get; }
        bool CanMigrate();
        void Migrate(MaaSConfig config);
    }
}
