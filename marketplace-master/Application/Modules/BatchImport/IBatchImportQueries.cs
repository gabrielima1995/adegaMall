using System.Collections.Generic;
using System.Threading.Tasks;

namespace Application.Modules.BatchImport
{
    public interface IBatchImportQueries
    {
        Task<List<string>> ListUserEmailsAsync();
    }
}
