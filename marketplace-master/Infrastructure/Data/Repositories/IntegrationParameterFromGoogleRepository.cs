using Domain.Interfaces.Repositories;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Framework;
using Domain.Modules.Integration;
using Infrastructure.Data.Context;
using System.Data.Entity;

namespace Infrastructure.Data.Repositories
{
    public class IntegrationParameterFromGoogleRepository : EfCrudRepository, IIntegrationParameterFromGoogleRepository
    {
        public IntegrationParameterFromGoogleRepository(ScaffoldContext context) : base(context)
        {
        }

        public OperationResult Add(IntegrationParameterFromGoogle entry)
        {
            return Add<IntegrationParameterFromGoogle>(entry, false);

        }

        public OperationResult AddAndSave(IntegrationParameterFromGoogle entry)
        {
            return Add<IntegrationParameterFromGoogle>(entry, true);
        }

        public OperationResult Delete(IntegrationParameterFromGoogle entry)
        {
            return Delete<IntegrationParameterFromGoogle>(entry, false);
        }

        public OperationResult DeleteAndSave(IntegrationParameterFromGoogle entry)
        {
            return Delete<IntegrationParameterFromGoogle>(entry, true);
        }

        public IntegrationParameterFromGoogle GetById(params object[] idValues)
        {
            return GetById<IntegrationParameterFromGoogle>(idValues);
        }

        //public List<IntegrationParameterFromGoogle> List(bool onlyEnabled, string sellerId)
        //{
        //    if (onlyEnabled)
        //    {
        //        return Context.IntegrationParameterFromGoogle
        //                    .Where(x => x.Enabled && x.Seller_Id == sellerId)
        //                    .ToList();
        //    }
        //    else 
        //    {
        //        return Context.IntegrationParameterFromGoogle
        //                    .Where(x => x.Seller_Id == sellerId)
        //                    .ToList();
        //    }
        //}

        public List<IntegrationParameterFromGoogle> List(bool onlyEnabled)
        {
            if (onlyEnabled)
            {
                return Context.IntegrationParameterFromGoogle
                            .Where(x => x.Enabled)
                            .ToList();
            }
            else
            {
                return Context.IntegrationParameterFromGoogle
                            .ToList();
            }
        }

        public OperationResult Update(IntegrationParameterFromGoogle entry, bool saveChanges = false)
        {
            Context.Entry(entry).State = EntityState.Modified;

            var updateResult = base.Update(entry, saveChanges);

            return updateResult;

            //Update<IntegrationParameterFromGoogle>(entry, false);
            //Context.SaveChanges();
        }

        public OperationResult UpdateAndSave(IntegrationParameterFromGoogleVM entry)
        {

            return Update<IntegrationParameterFromGoogleVM>(entry, true);
        }

        //private void ClearCache()
        //{
        //    CacheHelper.Evict(CacheKeyFmt_ListActive);
        //}
    }
}
