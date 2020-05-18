using System.Web;
using Unity.AspNet.Mvc;
using Unity.Lifetime;

namespace Presentation
{
    public class ScopedLifetimeManager : LifetimeManager, ITypeLifetimeManager
    {
        private readonly CustomPerRequestLifetimeManager _perRequestLifetimeManager = new CustomPerRequestLifetimeManager();
        private readonly CustomPerThreadLifetimeManager _perThreadLifetimeManager = new CustomPerThreadLifetimeManager();

        public override object GetValue(ILifetimeContainer container = null)
            => GetAppropriateLifetimeManager().GetValue();

        public override void SetValue(object newValue, ILifetimeContainer container = null)
            => GetAppropriateLifetimeManager().SetValue(newValue);

        public override void RemoveValue(ILifetimeContainer container = null)
            => GetAppropriateLifetimeManager().RemoveValue();

        protected override LifetimeManager OnCreateLifetimeManager()
        {
            if (HttpContext.Current != null) return _perRequestLifetimeManager.OnCreateLifetimeManagerImpl();
            return _perThreadLifetimeManager.OnCreateLifetimeManagerImpl();
        }

        private LifetimeManager GetAppropriateLifetimeManager()
        {
            if (HttpContext.Current == null) return _perThreadLifetimeManager;
            return _perRequestLifetimeManager;
        }

        private class CustomPerRequestLifetimeManager : PerRequestLifetimeManager
        {
            public LifetimeManager OnCreateLifetimeManagerImpl() => base.OnCreateLifetimeManager();
        }

        private class CustomPerThreadLifetimeManager : PerThreadLifetimeManager
        {
            public LifetimeManager OnCreateLifetimeManagerImpl() => base.OnCreateLifetimeManager();
        }
    }
}