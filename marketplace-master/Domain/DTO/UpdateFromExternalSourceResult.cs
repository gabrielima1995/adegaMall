using Framework;
using System;
using System.Collections.Generic;

namespace Domain.DTO
{
    public class UpdateFromExternalSourceResult : OperationResult
    {
        public List<string> ExternalSourceIds { get; set; } = new List<string>();
        public List<UpdateFromExternalSourceResultItem> UpdatedItems { get; set; } = new List<UpdateFromExternalSourceResultItem>();
    }

    public class UpdateFromExternalSourceResultItem
    {
        public Guid Id { get; set; }
        public string CustomId { get; set; }
        public int PreviousStock { get; set; }
        public int NewStock { get; set; }
        public decimal? PreviousPrice { get; set; }
        public decimal? NewPrice { get; set; }

        public UpdateFromExternalSourceResultItem()
        {
        }
    }

}
