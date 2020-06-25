using Application.Modules.BatchImport.Models;
using System.IO;
using System.Threading.Tasks;

namespace Application.Modules.BatchImport.Products
{
    public class GenerateProductTemplateCommand
    {
        public GenerateProductTemplateCommand(BatchImportSourceType sourceType)
        {
            SourceType = sourceType;
        }

        public BatchImportSourceType SourceType { get; }
    }

    public class GenerateProductTemplateCommandHandler
    {
        private readonly IBatchImportService batchImportService;

        public GenerateProductTemplateCommandHandler(IBatchImportService batchImportService)
        {
            this.batchImportService = batchImportService;
        }

        public async Task<Stream> HandleAsync(GenerateProductTemplateCommand command)
        {
            var stream = new MemoryStream();

            await batchImportService.GenerateProductTemplateAsync(command.SourceType, stream);
            stream.Seek(0, SeekOrigin.Begin);

            return stream;
        }
    }
}
