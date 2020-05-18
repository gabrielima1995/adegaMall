using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Domain.DTO
{
    public class StoreProductResultsVM
    {
        public StoreProductResultsVM(int totalCount, int filteredCount, IEnumerable<StoreProductVM> items)
        {
            RecordsTotal = totalCount;
            RecordsFiltered = filteredCount;
            Items = items.ToList().AsReadOnly();
        }

        public int RecordsTotal { get; }
        public int RecordsFiltered { get; }
        public ReadOnlyCollection<StoreProductVM> Items { get; }
    }

    public class StoreProductVM
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string CoverImageUrl { get; set; }
        public decimal CurrentStock { get; set; }
        public string CategoryName { get; set; }
        public string CustomId { get; set; }

        public decimal Price { get; set; }
        public string PriceFormatted { get; set; }
        public string DiscountPriceFormatted { get; set; }

    }
}