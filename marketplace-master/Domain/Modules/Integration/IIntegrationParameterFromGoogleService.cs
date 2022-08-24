using Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Modules.Integration
{
    public interface IIntegrationParameterFromGoogleService
    {
        IList<IntegrationParameterFromGoogleVM> List(bool onlyActive);

        //IntegrationParameterFromGoogleVM PrepareNew(String seller_id);

        IntegrationParameterFromGoogleVM Get(Guid id);

        OperationResult<IntegrationParameterFromGoogle> SaveNew(IntegrationParameterFromGoogleVM model);

        OperationResult<IntegrationParameterFromGoogle> Update(IntegrationParameterFromGoogleVM model);

        void Delete(Guid id);

    }
}
