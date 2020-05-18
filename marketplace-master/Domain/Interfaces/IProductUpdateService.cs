using Domain.DTO;
using Framework;
using System.Collections.Generic;

namespace Domain.Interfaces
{
    public interface IProductUpdateService
    {
        OperationResult<IEnumerable<ExternalProductUpdate>> GetUpdatedInfo();
    }
}
