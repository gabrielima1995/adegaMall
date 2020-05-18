using Framework;
using System.Configuration;
using Unity;

namespace V2.Infrastructure.Bootstrapper.Internals
{
    internal class ApplicationDependencies : IDependenciesRegistration
    {
        private const string connectionStringName = "ScaffoldContext";

        public void Register(IUnityContainer container)
        {
            var connStr = ConfigurationManager.ConnectionStrings[connectionStringName].ConnectionString;
            container.RegisterInstance(new ConnectionString(connStr));
        }
    }
}