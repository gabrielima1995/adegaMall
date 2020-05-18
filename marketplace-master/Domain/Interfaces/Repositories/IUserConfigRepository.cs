using Domain.UserConfig;
using Framework;

namespace Domain.Interfaces.Repositories
{
    public interface IUserConfigRepository
    {
        OperationResult SaveConfig(UserConfiguration config);

        UserConfiguration ReadConfig();
    }
}
