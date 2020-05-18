using System.IO;
using System.Threading.Tasks;

namespace BatchImport
{
    public interface ITemplateGenerator
    {
        void WriteTemplate(IDataModel dataModel, Stream output);

        Task WriteTemplateAsync(IDataModel dataModel, Stream output);
    }
}
