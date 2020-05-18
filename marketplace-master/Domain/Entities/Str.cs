using Domain.Entities;

namespace Infra.Crosscutting.Globalization
{
    /// <summary>
    /// String em determinado idioma.
    /// </summary>
    public class Str : BaseEntity<int>
    {
        public Lang Lang { get; set; }

        public string Label { get; set; }

        public string Value { get; set; }
    }
}