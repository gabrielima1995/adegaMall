using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using V2.Application.Modules.ProductTemplateModule;

namespace Presentation.Models
{
    public class StoreTemplatesViewModel
    {
        public StoreTemplatesViewModel()
        {
        }

        public StoreTemplatesViewModel(IEnumerable<StoreTemplateItemDto> items, string currentMode)
        {
            AllItems = items.ToList().AsReadOnly();
            MyItems = items.Where(item => item.SellerProductId.HasValue).ToList().AsReadOnly();
            AvailableItems = items.Where(item => item.SellerProductId == null).ToList().AsReadOnly();

            CurrentMode = DecideOnBestMode(currentMode);
        }

        public ReadOnlyCollection<StoreTemplateItemDto> AllItems = new List<StoreTemplateItemDto>().AsReadOnly();
        public ReadOnlyCollection<StoreTemplateItemDto> MyItems { get; }
        public ReadOnlyCollection<StoreTemplateItemDto> AvailableItems { get; }

        public string CurrentMode { get; private set; }
        public ReadOnlyCollection<StoreTemplateItemDto> CurrentList => CurrentMode == "mine" ? MyItems : CurrentMode == "available" ? AvailableItems : AllItems;

        public string DecideOnBestMode(string requestedMode) =>
            "all,mine,available".Split(',').Contains(requestedMode) ? requestedMode :
            MyItems.Count > 0 ? "mine" :
            AvailableItems.Count > 0 ? "available" : "all";
    }
}