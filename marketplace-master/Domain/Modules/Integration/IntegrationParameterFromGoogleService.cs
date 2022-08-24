using Domain.Interfaces.Repositories;
using Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Modules.Integration
{
    public class IntegrationParameterFromGoogleService : IIntegrationParameterFromGoogleService
    {
        private readonly IIntegrationParameterFromGoogleRepository repository;

        public IntegrationParameterFromGoogleService(IIntegrationParameterFromGoogleRepository _repository)
        {
            repository = _repository;
        }

        public IntegrationParameterFromGoogleService()
        {

        }

        public void Delete(Guid id)
        {
            var entity = repository.GetById(id);
            repository.DeleteAndSave(entity);
        }

        public IntegrationParameterFromGoogleVM Get(Guid id)
        {
            return new IntegrationParameterFromGoogleVM(repository.GetById(id));
        }

        public IList<IntegrationParameterFromGoogleVM> List(bool onlyActive)
        {
            return repository.List(onlyActive)
                .Select(x => new IntegrationParameterFromGoogleVM(x))
                .ToList();
        }

        //public IntegrationParameterFromGoogleVM PrepareNew(string seller_id)
        //{
        //    return new IntegrationParameterFromGoogleVM() { Seller_Id = seller_id };
        //}

        public OperationResult<IntegrationParameterFromGoogle> SaveNew(IntegrationParameterFromGoogleVM model)
        {
            var entity = model.ToClass();

            var resp = repository.AddAndSave(entity);
            if (resp.OK)
            {
                return repository.GetById(entity.Id);
            }
            else
            {
                return null;
            }
        }

        public OperationResult<IntegrationParameterFromGoogle> Update(IntegrationParameterFromGoogleVM model)
        {
            var resp = repository.Update(model.ToClass(), true);
            if (resp.OK)
            {
                return repository.GetById(model.Id);
            }
            else
            {
                return null;
            }
        }
    }
}
