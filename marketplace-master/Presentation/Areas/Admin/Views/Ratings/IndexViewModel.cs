using Domain.Modules.Ratings;
using System.Collections.Generic;
using System.Linq;

namespace Presentation.Areas.Admin.Views.Ratings
{
    public class IndexViewModel
    {
        private readonly IEnumerable<UserRatingModel> ratingItems;

        public IndexViewModel(IEnumerable<UserRatingModel> ratingItems, ViewMode mode)
        {
            this.ratingItems = ratingItems;
            Mode = mode;
        }

        public ViewMode Mode { get; }
        public IReadOnlyCollection<UserRatingModel> RatingsForCurrentMode => Mode == ViewMode.Visible ? VisibleRatings : HiddenRatings;
        public IReadOnlyCollection<UserRatingModel> VisibleRatings => ratingItems.Where(rating => rating.IsVisible).ToList();
        public IReadOnlyCollection<UserRatingModel> HiddenRatings => ratingItems.Where(rating => rating.Hidden).ToList();
        public string Title => "Avaliações " + (Mode == ViewMode.Visible ? "Visíveis" : "Ocultas");

        public enum ViewMode
        {
            Visible,
            Hidden
        }
    }
}