using Application.Config;

namespace Domain.Config.ConfigMigrations
{
    internal abstract class ConfigMigrationBase : IConfigMigration
    {
        public abstract int ExpectedVersion { get; }
        public abstract int NewVersion { get; }

        public virtual bool CanMigrate()
        {
            return true;
        }

        public abstract void Migrate(MaaSConfig config);
    }
}
