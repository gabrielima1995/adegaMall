namespace Domain.Modules.BatchImport
{
    public class Field
    {
        public Field(string name, string displayName, DataType dataType, string sampleValue, bool isMandatory = false)
        {
            Name = name;
            DisplayName = displayName;
            DataType = dataType;
            SampleValue = sampleValue;
            IsMandatory = isMandatory;
        }

        public string Name { get; }
        public string DisplayName { get; }
        public DataType DataType { get; }
        public string SampleValue { get; }
        public bool IsMandatory { get; }

        public override string ToString()
        {
            return $"{Name}, {DataType}";
        }
    }
}
