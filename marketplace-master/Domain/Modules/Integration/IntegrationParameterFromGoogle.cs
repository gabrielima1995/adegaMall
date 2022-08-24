using System;
using System.ComponentModel.DataAnnotations;

namespace Domain.Modules.Integration
{
    public class IntegrationParameterFromGoogle
    {
        [Key]
        public Guid Id { get; set; }

        [MaxLength(200)]
        public String AccountId { get; set; }

        [MaxLength(200)]
        public String ClientId { get; set; }

        [MaxLength(300)]
        public String ClientEmail { get; set; }

        public String PrivateKey { get; set; }

        public bool Enabled { get; set; } = true;

        public IntegrationParameterFromGoogle()
        {
            Id = Guid.Parse(Guid.NewGuid().ToString().ToUpper());
        }
    }
}
