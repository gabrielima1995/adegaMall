using Domain.Entities;
using Domain.Entities.PortfolioModule;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace Domain.Modules.Portfolio
{
    /// <summary>
    /// Item do portfólio
    /// </summary>
    public class PortfolioItem : SoftDeletableEntity<Guid>
    {
        private IList<PortfolioItemImage> _images;

        public string Title { get; set; }

        public string Description { get; set; }

        public string VideoUrl { get; set; }

        public decimal? Price { get; set; }

        public string CoverImageUrl { get; set; }

        [StringLength(1000)]
        public string FriendlyUrl { get; set; }

        public IList<PortfolioItemImage> Images
        {
            get { return _images = _images?.OrderBy(i => i.Position).ToList(); }
            set { _images = value; }
        }

        public IList<SubCategory> SubCategories { get; set; } = new List<SubCategory>();

        [NotMapped]
        public bool IsVideo => !string.IsNullOrEmpty(VideoUrl);

        [Required]
        public Portfolio Portfolio { get; set; }

        public PortfolioItem()
        {
            Id = Guid.NewGuid();
            Images = new List<PortfolioItemImage>();
        }

        public void GenerateFriendlyUrl(string suffix = null)
        {
            var value = !string.IsNullOrEmpty(suffix)
                ? Title + "-" + suffix
                : Title;

            FriendlyUrl = value
                .ReplaceDiacritics()
                .Hyphenize();
        }
    }
}