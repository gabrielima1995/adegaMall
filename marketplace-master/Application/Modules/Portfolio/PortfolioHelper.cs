using Application.Config;
using Application.Helpers;
using Framework;
using System.Linq;

namespace Application.Modules.Portfolio
{
    public static class PortfolioHelper
    {
        private const int Default = 380;

        public static string GetUrl(this IImage image)
        {
            return ImgHelper.Get(image.Url, Default, Default);
        }

        public static string GetCoverImageUrl(this PortfolioItemVM item)
        {
            if (!string.IsNullOrEmpty(item.CoverImageUrl))
                return ImgHelper.Get(item.CoverImageUrl, Default, Default);

            return item.IsVideo || item.Images == null || !item.Images.Any()
                ? MaaSConfig.Current.Layout.Images.DefaultVideoThumbUrl
                : ImgHelper.Get(item.Images.First().Url, Default, Default);
        }
    }
}