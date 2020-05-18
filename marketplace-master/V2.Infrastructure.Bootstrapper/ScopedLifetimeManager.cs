using System.Web;
using Unity.AspNet.Mvc;
using Unity.Lifetime;

namespace V2.Infrastructure.Bootstrapper.Internals
{
    public class ScopedLifetimeManager : LifetimeManager, ITypeLifetimeManager
    {
        private readonly MyPerRequestLifetimeManager perRequest = new MyPerRequestLifetimeManager();
        private readonly MyPerThreadLifetimeManager perThread = new MyPerThreadLifetimeManager();

        public override object GetValue(ILifetimeContainer container = null)
        {
            return GetLifetimeManager().GetValue(container);
        }

        public override void SetValue(object newValue, ILifetimeContainer container = null)
        {
            GetLifetimeManager().SetValue(newValue, container);
        }

        public override void RemoveValue(ILifetimeContainer container = null)
        {
            GetLifetimeManager().RemoveValue(container);
        }

        protected override LifetimeManager OnCreateLifetimeManager()
        {
            return HttpContext.Current != null
                ? perRequest.OnCreateLifetimeManagerImpl()
                : perThread.OnCreateLifetimeManagerImpl();
        }

        private LifetimeManager GetLifetimeManager()
        {
            return HttpContext.Current == null ? perThread as LifetimeManager : perRequest;
        }
    }

    public class MyPerRequestLifetimeManager : PerRequestLifetimeManager
    {
        public LifetimeManager OnCreateLifetimeManagerImpl()
        {
            return base.OnCreateLifetimeManager();
        }
    }

    public class MyPerThreadLifetimeManager : PerThreadLifetimeManager
    {
        public LifetimeManager OnCreateLifetimeManagerImpl()
        {
            return base.OnCreateLifetimeManager();
        }
    }
}
