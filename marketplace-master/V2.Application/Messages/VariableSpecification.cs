namespace V2.Application.Messages
{
    public class VariableSpecification
    {
        public string Name { get; private set; }

        public string DescriptionLabel { get; private set; }

        internal VariableSpecification(string name, string descriptionLabel)
        {
            Name = name;
            DescriptionLabel = descriptionLabel;
        }
    }
}