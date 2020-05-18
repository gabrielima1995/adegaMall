using BatchImport;
using BatchImport.Fields;
using Infra.BatchImportAdapter.Internals;
using System;

namespace Infra.BatchImportAdapter.Fields
{
    public static class DomainFieldAdapter
    {
        public static IField Convert(Domain.Modules.BatchImport.Field field)
        {
            switch (field.DataType)
            {
                case Domain.Modules.BatchImport.DataType.AlphanumericCode:
                    return new StringField(field.Name, L(field.DisplayName), L(field.SampleValue), field.IsMandatory);

                case Domain.Modules.BatchImport.DataType.NameOrTitle:
                    return new StringField(field.Name, L(field.DisplayName), L(field.SampleValue), field.IsMandatory, maxLength: 100);

                case Domain.Modules.BatchImport.DataType.GeneralText:
                    return new StringField(field.Name, L(field.DisplayName), L(field.SampleValue), field.IsMandatory);

                case Domain.Modules.BatchImport.DataType.Email:
                    return new EmailField(field.Name, L(field.DisplayName), L(field.SampleValue), field.IsMandatory);

                case Domain.Modules.BatchImport.DataType.Url:
                    return new UrlField(field.Name, L(field.DisplayName), L(field.SampleValue), field.IsMandatory);

                case Domain.Modules.BatchImport.DataType.YesNo:
                    return new YesNoField(field.Name, L(field.DisplayName), L(field.SampleValue), field.IsMandatory);

                case Domain.Modules.BatchImport.DataType.Price:
                    return new DecimalField(field.Name, L(field.DisplayName), L(field.SampleValue), field.IsMandatory);

                case Domain.Modules.BatchImport.DataType.Quantity:
                    return new IntField(field.Name, L(field.DisplayName), L(field.SampleValue), field.IsMandatory);

                case Domain.Modules.BatchImport.DataType.PackageSizeUnitInCm:
                    return new DecimalField(field.Name, L(field.DisplayName), L(field.SampleValue), field.IsMandatory);

                case Domain.Modules.BatchImport.DataType.PackageWeightInKg:
                    return new DecimalField(field.Name, L(field.DisplayName), L(field.SampleValue), field.IsMandatory);
            }

            throw new Exception($"DomainFieldAdapter does not know how to convert a field of DataType '{field.DataType}'.");
        }

        private static string L(string label) => Localization.Get(label);
    }
}
