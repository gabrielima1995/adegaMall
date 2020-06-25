using Domain.Entities;
using Domain.Entities.PortfolioModule;
using System.Collections.Generic;

namespace Application.Modules.Quotes
{
    public class QuoteViewModel_DEPRECATED : QuoteRequestViewModel_DEPRECATED, IWithCategory, IWithSubCategory
    {
        public IList<Category> Categories { get; set; }
        public bool HasBuyer => !string.IsNullOrEmpty(Name) && !string.IsNullOrEmpty(Email);
        public Seller Seller { get; set; }
        public IList<SubCategory> SubCategories { get; set; }

        public QuoteViewModel_DEPRECATED()
        {
        }

        public QuoteViewModel_DEPRECATED(QuoteRequestViewModel_DEPRECATED request)
        {
            SellerId = request.SellerId;
            Name = request.Name;
            Email = request.Email;
            Title = request.Title;
            Description = request.Description;
            Category = request.Category;
            SubCategory = request.SubCategory;
            Address = request.Address;
            AddressNumber = request.AddressNumber;
            AddressNeighborhood = request.AddressNeighborhood;
            AddressZipCode = request.AddressZipCode;
            CityAndState = request.CityAndState;
            Phone = request.Phone;
            Date = request.Date;
            Preference = request.Preference;
            Quote = request.Quote;
            NewPassword = request.NewPassword;
            ConfirmPassword = request.ConfirmPassword;
        }
    }
}