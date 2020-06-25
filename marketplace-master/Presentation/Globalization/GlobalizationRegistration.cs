using Domain.Interfaces.Repositories;
using Infra.Crosscutting.Globalization;
using Owin;
using Unity;

namespace System.Web.Mvc
{
    public static class GlobalizationRegistration
    {
        public static IAppBuilder LoadStrings(this IAppBuilder app, IUnityContainer container)
        {
            var stringsRepository = container.Resolve<IStringRepository>();
            GlobalizationHelper.LoadStrings(stringsRepository);
            return app;
        }

        public static void RegisterBinders(ModelBinderDictionary binders)
        {
            binders.Add(typeof(DateTime), new DateTimeBinder());
            binders.Add(typeof(DateTime?), new NullableDateTimeBinder());
            binders.Add(typeof(decimal), new DecimalBinder());
            binders.Add(typeof(decimal?), new NullableDecimalBinder());
        }
    }
}