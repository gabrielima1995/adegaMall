using Application.Modules.BatchImport.Models;
using BatchImport;
using BatchImport.Csv;
using BatchImport.MicrosoftExcel;
using System;
using System.IO;

namespace Infra.BatchImportAdapter.Internals
{
    internal class DataReaderFactory
    {
        public static IDataReader Create(BatchImportSourceType sourceType, Stream input)
        {
            switch (sourceType)
            {
                case BatchImportSourceType.Excel:
                    return new ExcelDataReader(input);

                case BatchImportSourceType.Csv:
                    return new CsvDataReader(input);

                default:
                    throw new UnknownBatchImportSourceTypeException();
            }
        }
    }

    internal class TemplateGeneratorFactory
    {
        public static ITemplateGenerator Create(BatchImportSourceType sourceType)
        {
            switch (sourceType)
            {
                case BatchImportSourceType.Excel:
                    return new ExcelTemplateGenerator();

                case BatchImportSourceType.Csv:
                    return new CsvTemplateGenerator();

                default:
                    throw new UnknownBatchImportSourceTypeException();
            }
        }
    }

    public class UnknownBatchImportSourceTypeException : Exception { }
}
