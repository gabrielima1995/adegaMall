using Application.Modules.Products.Orders;
using Application.Modules.Products.Rental;
using Domain.Modules.StoreModule.Interfaces;
using Domain.StoreModule.Commands;
using Domain.StoreModule.Interfaces;
using Infrastructure.Data.Repositories;
using Unity;
using V2.Application.Modules.StoreModule;
using V2.Application.Modules.StoreModule.Queries;
using V2.Infrastructure.Modules.StoreModule.Queries;

namespace V2.Infrastructure.Bootstrapper.Internals
{
    internal class ProductRentalDependencies : IDependenciesRegistration
    {
        public void Register(IUnityContainer container)
        {
            container
                .RegisterType<NewTransactionRentalOnOrder>()
                .RegisterType<ReminderNotificationRentalRecurrentJob>()
                .RegisterType<IRentalLockQueries, ScheduleQueries>()
                .RegisterType<IRentalLockRepository, RentalLockRepository>()
                .RegisterType<IProductAvailabilityRepository, ProductAvailabilityRepository>()
                .RegisterType<IScheduleQueries, ScheduleQueries>()
                .RegisterType<IScheduleTimelineQueries, ScheduleTimelineQueries>()

                .RegisterType<AddRentalShiftCommandHandler>()
                .RegisterType<RemoveRentalShiftCommandHandler>()
                .RegisterType<SetProductRentalShiftsCommandHandler>()
                .RegisterType<UpdateRentalShiftCommandHandler>()

                .RegisterType<IRentalShiftQueries, RentalShiftQueries>()
                .RegisterType<IRentalShiftRepository, RentalShiftRepository>()
                .RegisterType<RentalShiftsAppService>();
        }
    }
}