using System;
using System.Collections.Generic;

namespace Domain.Entities
{
    public class EcommerceHome : BaseEntity<Guid>
    {
        public List<EcommerceHomeCategory> Categories { get; set; }

        public EcommerceHome()
        {
            Id = Guid.NewGuid();
            Categories = new List<EcommerceHomeCategory>();
        }
    }
}
