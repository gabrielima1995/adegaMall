using System;

namespace Domain.DTO
{
    public class LockProductRentalDto
    {
        public DateTime LockStartUtc { get; set; }

        public DateTime LockEndUtc { get; set; }
    }

    public class LockProductRentalDtoTest
    {
        public DateTime LockStartUtc { get; set; }

        public string LockEndUtc { get; set; }
    }
}