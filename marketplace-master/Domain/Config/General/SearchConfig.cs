using Application.Config;
using FluentValidation;
using System.ComponentModel.DataAnnotations;

namespace Domain.Config.General
{
    public class SearchConfig
    {
        public bool Enabled { get; set; } = true;
        public bool RequireLogin { get; set; } = false;
        public bool IgnorePaymentRequirements { get; set; } = false;
        public bool BrowserGeolocation { get; set; } = true;
        public SearchType SearchType { get; set; }
        public ResultType ResultType { get; set; }
        public SearchFieldsConfig SearchFields { get; set; } = new SearchFieldsConfig();
        public SearchMapConfig Map { get; set; } = new SearchMapConfig();
        public SearchListConfig List { get; set; } = new SearchListConfig();
        public bool ShowDescription { get; set; } = true;
        public int DescriptionMaxLength { get; set; } = 100;
        public bool RequireCategoryForProductSearch { get; set; } = false;
        public QuoteSearchConfig QuotesSearch { get; set; } = new QuoteSearchConfig();
    }

    public class QuoteSearchConfig
    {
        public bool Enabled { get; set; } = false;
        public bool RequiredLogin { get; set; } = true;
        public int ResultsPerPage = 8;

        public decimal MaxPrice { get; set; } = 2000M;

        public QuoteSearchResultFieldsConfig HiddenFields { get; set; } = new QuoteSearchResultFieldsConfig();
        public QuoteSearchFilterConfig Filters { get; set; } = new QuoteSearchFilterConfig();
    }

    public class QuoteSearchFilterConfig
    {
        public bool Category { get; set; } = true;
        public bool SubCategory { get; set; } = true;

        public bool MinDate { get; set; } = true;
        public bool MaxDate { get; set; } = true;
        public bool PriceRange { get; set; } = true;
    }

    public class QuoteSearchResultFieldsConfig
    {
        public bool Description { get; set; } = true;

        public bool Name { get; set; } = true;

        public bool Address { get; set; } = true;

        public bool Email { get; set; } = true;

        public bool Amount { get; set; } = false;

        public bool Phone { get; set; } = true;

        public bool Avatar { get; set; } = true;

        public bool Title { get; set; } = false;

        public bool Categories { get; set; } = false;

        public bool Date { get; set; } = false;

        //public bool CustomFields { get; set; } = true;
    }

    public class SearchFieldsConfig
    {
        public bool Category { get; set; } = true;
        public bool Location { get; set; } = true;
        public bool Query { get; set; } = true;
        public bool OnlyOnSitePickup { get; set; }
        public bool PeriodAvailability { get; set; }
        public bool LocationRange { get; set; }
    }

    public class SearchMapConfig
    {
        public bool Enabled { get; set; } = false;
        public int SearchRange { get; set; } = 5000;
    }

    public class SearchListConfig
    {
        public bool Enabled { get; set; } = true;
        public int PageSize => 24;
        public int SearchRange { get; set; } = 5000;

        public SearchRangeOption[] SearchRangeOptions { get; set; } =
        {
            new SearchRangeOption(500,   "500m"),
            new SearchRangeOption(1000,  "1km"),
            new SearchRangeOption(2000,  "2km"),
            new SearchRangeOption(3000,  "3km"),
            new SearchRangeOption(4000,  "4km"),
            new SearchRangeOption(5000,  "5km"),
            new SearchRangeOption(10000, "10km"),
            new SearchRangeOption(15000, "15km"),
            new SearchRangeOption(30000, "30km"),
        };
    }

    public class SearchRangeOption
    {
        public int Value { get; set; }
        public string Label { get; set; }

        public SearchRangeOption()
        {
        }

        public SearchRangeOption(int value, string label)
        {
            Value = value;
            Label = label;
        }
    }

    public enum SearchType
    {
        [Display(Name = "Vendedor")]
        Seller = 0,

        [Display(Name = "Produto")]
        Product = 1,

        [Display(Name = "Portfolio")]
        Portfolio = 2,
    }

    public enum ResultType
    {
        [Display(Name = "Vendedor")]
        Seller = 0,

        [Display(Name = "Produto")]
        Product = 1,

        [Display(Name = "Portfolio")]
        Portfolio = 2,
    }

    #region Validation

    public class SearchConfigValidator : AbstractValidator<SearchConfig>
    {
        public SearchConfigValidator(MaaSConfig config)
        {
            RuleFor(x => x.ResultType)
                .Equal(ResultType.Product)
                .Unless(x => x.SearchType != SearchType.Product);

            RuleFor(x => x.SearchFields)
                .SetValidator(x => new SearchFieldsConfigValidator(x));

            RuleFor(x => x.Map)
                .SetValidator(x => new SearchMapConfigValidator(x));

            RuleFor(x => x.List)
                .SetValidator(x => new SearchListConfigValidator(x));
        }
    }

    public class SearchFieldsConfigValidator : AbstractValidator<SearchFieldsConfig>
    {
        public SearchFieldsConfigValidator(SearchConfig searchConfig)
        {
            RuleFor(x => x.Category)
                .Equal(true)
                .Unless(x => SomeFieldEnabled(x));

            RuleFor(x => x.Location)
                .Equal(true)
                .Unless(x => SomeFieldEnabled(x));

            RuleFor(x => x.Query)
                .Equal(true)
                .Unless(x => SomeFieldEnabled(x));
        }

        private bool SomeFieldEnabled(SearchFieldsConfig config)
        {
            return config.Category || config.Location || config.Query;
        }
    }

    public class SearchMapConfigValidator : AbstractValidator<SearchMapConfig>
    {
        public SearchMapConfigValidator(SearchConfig searchConfig)
        {
            RuleFor(x => x.Enabled)
                .Equal(true)
                .Unless(x => !searchConfig.Enabled || searchConfig.List.Enabled);

            RuleFor(x => x.SearchRange)
                .GreaterThanOrEqualTo(500)
                .Unless(x => !x.Enabled);
        }
    }

    public class SearchListConfigValidator : AbstractValidator<SearchListConfig>
    {
        public SearchListConfigValidator(SearchConfig searchConfig)
        {
            RuleFor(x => x.Enabled)
                .Equal(true)
                .Unless(x => !searchConfig.Enabled || searchConfig.Map.Enabled);

            RuleFor(x => x.PageSize)
                .GreaterThanOrEqualTo(10)
                .Unless(x => !x.Enabled);
        }
    }

    #endregion Validation
}