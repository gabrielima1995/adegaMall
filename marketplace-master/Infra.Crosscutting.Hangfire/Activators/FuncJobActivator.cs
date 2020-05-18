using System;

namespace Hangfire
{
    /// <summary>
    /// Activator responsible for resolving IoC dependencies.
    /// </summary>
    public class FuncJobActivator : JobActivator
    {
        private readonly Func<Type, object> _resolverAction;

        /// <summary>
        /// Creates an instance of activator.
        /// </summary>
        /// <param name="resolverAction">Action responsible for resolving an IoC dependency.</param>
        public FuncJobActivator(Func<Type, object> resolverAction)
        {
            _resolverAction = resolverAction;
        }

        public override object ActivateJob(Type jobType)
        {
            return _resolverAction(jobType);
        }
    }
}