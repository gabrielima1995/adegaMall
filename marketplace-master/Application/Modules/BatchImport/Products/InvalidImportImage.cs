using Application.Modules.BatchImport.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Application.Modules.BatchImport.Products
{
    internal class InvalidImportImage : IInvalidRecord
    {
        public InvalidImportImage(IEnumerable<InvalidImportField> invalidFields)
        {
            if (invalidFields == null || invalidFields.Count() == 0)
                throw new ArgumentNullException(nameof(invalidFields), $"Cannot create an instance of {nameof(InvalidImportProduct)} with null or zero invalid fields.");

            Fields = invalidFields.ToArray();
        }

        public IInvalidRecordField[] Fields { get; }
    }
}
