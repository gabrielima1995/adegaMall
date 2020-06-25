using Application.Config;

namespace Presentation.Areas.Admin.Models
{
    public class BaseAdminModel
    {
        public MaaSConfig Config => MaaSConfig.Current;
    }
}