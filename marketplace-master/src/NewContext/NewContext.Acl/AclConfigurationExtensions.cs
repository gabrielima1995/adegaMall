using NewContext.Acl.Languages;
using Unity;

namespace NewContext.Acl
{
    public static class AclConfigurationExtensions
    {
        public static IUnityContainer AddNewContextAcl(this IUnityContainer container, string connectionString)
        {
            container
                .RegisterInstance(new ConnectionString(connectionString))
                .RegisterType<IStringsRepository, StringsRepository>();

            return container;
        }
    }
}
