using System.Collections.Generic;
using Domain.Modules.Integration;
using Framework;

namespace Domain.Interfaces.Repositories
{
    public interface IIntegrationParameterFromGoogleRepository
    {
        IntegrationParameterFromGoogle GetById(params object[] idValues);
        //List<IntegrationParameterFromGoogle> List(bool onlyEnabled, string sellerId);
        List<IntegrationParameterFromGoogle> List(bool onlyEnabled);
        OperationResult Add(IntegrationParameterFromGoogle entry);
        OperationResult AddAndSave(IntegrationParameterFromGoogle entry);
        OperationResult Update(IntegrationParameterFromGoogle entry, bool saveChanges);
        OperationResult UpdateAndSave(IntegrationParameterFromGoogleVM entry);
        OperationResult Delete(IntegrationParameterFromGoogle entry);
        OperationResult DeleteAndSave(IntegrationParameterFromGoogle entry);
        OperationResult SaveChanges();

    }
}
