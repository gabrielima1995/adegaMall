using BatchImport;
using BatchImport.Fields;
using PaymentIntegrator.Core.Common;
using System;

namespace Infra.BatchImportAdapter.Fields
{
    public class BrazilianBankCodeField : FieldBase
    {
        private const string ValidCodes = // According to PaymentIntegrator.Core.Common.Bank
            "001,033,104,237,341,041,748,756,003,004,010,018,021,025,036,037,040," +
            "047,062,063,065,069,070,074,077,079,083,084,085,089,090,094,097,098," +
            "099,107,114,120,121,122,124,125,132,133,163,184,204,212,217,218,222," +
            "224,233,241,243,246,248,249,250,254,263,265,266,300,318,320,366,376," +
            "389,394,399,412,422,464,473,477,479,487,494,505,600,604,610,611,612," +
            "613,623,626,633,634,637,643,652,653,654,655,707,719,735,739,741,743," +
            "745,746,747,751,753,755,757";

        public BrazilianBankCodeField(string name, string displayName, string sample, bool isMandatory = false)
            : base(name, displayName, sample, isMandatory)
        {
        }

        public override object Parse(string input)
        {
            if (input == null) return null;
            return Enum.Parse(typeof(Bank), input);
        }

        public override FieldValidation Validate(string input)
        {
            var validation = base.Validate(input);

            if (input != null && !ValidCodes.Contains(input))
                validation.AddError("Código de banco inválido");

            return validation;
        }
    }
}
