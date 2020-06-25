using Framework;
using System;

namespace Domain.Modules.Products.Store
{
    [Obsolete]
    public class StoreService_Obsolete : ServiceBase<Entities.StoreModule.Store>
    {
        public StoreService_Obsolete(IRepository<Entities.StoreModule.Store> repository) : base(repository)
        {
        }
    }
}