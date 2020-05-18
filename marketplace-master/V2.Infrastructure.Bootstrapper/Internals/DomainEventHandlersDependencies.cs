using Domain.Core;
using Domain.Events;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using Unity;

namespace V2.Infrastructure.Bootstrapper.Internals
{
    internal class DomainEventHandlersDependencies : IDependenciesRegistration
    {
        private static readonly string[] assemblies = new[]
        {
            "Application",
            "Domain",
            "Infra.SmartSearchAdapter",
            "Infrastructure",
            "UnitTests",
            "V2.Application",
            "V2.Infrastructure"
        };

        public void Register(IUnityContainer container)
        {
            container.RegisterType<IDomainEventDispatcher, DomainEventDispatcher>();
            AddFromAssemblies(container);
        }

        private void AddFromAssemblies(IUnityContainer container)
        {
            var interfaceType = typeof(IDomainEventHandler<>);
            IEnumerable<Type> types;

            try
            {
                types = AppDomain.CurrentDomain.GetAssemblies()
                    .Where(x => assemblies.Contains(x.FullName.Substring(0, x.FullName.IndexOf(",", StringComparison.InvariantCulture))))
                    .SelectMany(x => x.GetTypes())
                    .Where(x => x.GetInterfaces().Any(i =>
                        i.IsGenericType &&
                        i.GetGenericTypeDefinition() == interfaceType))
                    .ToList();
            }
            catch (ReflectionTypeLoadException ex)
            {
                var sb = new StringBuilder();

                foreach (Exception exSub in ex.LoaderExceptions)
                {
                    sb.AppendLine(exSub.Message);

                    if (exSub is FileNotFoundException exFileNotFound)
                    {
                        if (!string.IsNullOrEmpty(exFileNotFound.FusionLog))
                        {
                            sb.AppendLine("Fusion Log:");
                            sb.AppendLine(exFileNotFound.FusionLog);
                        }
                    }

                    sb.AppendLine();
                }

                string errorMessage = sb.ToString();
                throw new Exception(errorMessage, ex);
            }

            foreach (var t in types)
            {
                var interfaces = t.GetInterfaces().Where(x =>
                    x.IsGenericType &&
                    x.GetGenericTypeDefinition() == interfaceType);

                foreach (var item in interfaces)
                {
                    var eventType = item.GenericTypeArguments.Single();
                    AddHandler(container, eventType, t);
                }
            }
        }

        private void AddHandler(IUnityContainer container, Type eventType, Type handlerType)
        {
            var genericHandlerType = typeof(IDomainEventHandler<>).MakeGenericType(eventType);
            var registrationName = $"{eventType.Name}_{handlerType.Name}";
            container.RegisterType(genericHandlerType, handlerType, registrationName);
        }
    }
}