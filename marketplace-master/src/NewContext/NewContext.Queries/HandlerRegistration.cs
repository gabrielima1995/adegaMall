using NewContext.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Unity;
using Unity.Injection;

namespace NewContext.Queries
{
    internal static class HandlerRegistration
    {
        internal static void AddQueryHandlers(this IUnityContainer container)
        {
            var handlerTypes = typeof(HandlerRegistration).Assembly.GetTypes()
                .Where(x => x.GetInterfaces().Any(y => IsHandlerInterface(y)))
                .ToArray();

            foreach (Type type in handlerTypes)
                AddHandler(container, type);
        }

        private static void AddHandler(IUnityContainer services, Type type)
        {
            var attributes = type.GetCustomAttributes(false);
            var pipeline = attributes
                .Select(x => ToDecorator(x))
                .Concat(new[] { type })
                .Reverse()
                .ToList();

            var interfaceType = type.GetInterfaces().Single(y => IsHandlerInterface(y));
            var factory = BuildPipeline(pipeline, interfaceType);

            services.RegisterType(interfaceType, new InjectionFactory(factory));
        }

        private static Func<IUnityContainer, object> BuildPipeline(List<Type> pipeline, Type interfaceType)
        {
            var ctors = pipeline
                .Select(x =>
                {
                    Type type = x.IsGenericType ? x.MakeGenericType(interfaceType.GenericTypeArguments) : x;
                    return type.GetConstructors().Single();
                })
                .ToList();

            Func<IUnityContainer, object> func = provider =>
            {
                object current = null;

                foreach (ConstructorInfo ctor in ctors)
                {
                    var parameterInfos = ctor.GetParameters().ToList();
                    var parameters = GetParameters(parameterInfos, current, provider);
                    current = ctor.Invoke(parameters);
                }

                return current;
            };

            return func;
        }

        private static object[] GetParameters(List<ParameterInfo> parameterInfos, object current, IUnityContainer provider)
        {
            var result = new object[parameterInfos.Count];

            for (int i = 0; i < parameterInfos.Count; i++)
                result[i] = GetParameter(parameterInfos[i], current, provider);

            return result;
        }

        private static object GetParameter(ParameterInfo parameterInfo, object current, IUnityContainer provider)
        {
            Type parameterType = parameterInfo.ParameterType;

            if (IsHandlerInterface(parameterType))
                return current;

            object service = provider.Resolve(parameterType);
            if (service != null)
                return service;

            throw new ArgumentException($"Type {parameterType} not found");
        }

        private static Type ToDecorator(object attribute)
        {
            // Register decorators for query handlers in here.

            Type type = attribute.GetType();

            // Example 1
            // if (type == typeof(DatabaseRetryAttribute))
            //    return typeof(DatabaseRetryDecorator<>);

            // Example 2
            // if (type == typeof(AuditLogAttribute))
            //    return typeof(AuditLoggingDecorator<>);

            // other attributes go here

            throw new ArgumentException(attribute.ToString());
        }

        private static bool IsHandlerInterface(Type type)
        {
            if (!type.IsGenericType)
                return false;

            Type typeDefinition = type.GetGenericTypeDefinition();
            return typeDefinition == typeof(IQueryHandler<,>);
        }
    }
}
