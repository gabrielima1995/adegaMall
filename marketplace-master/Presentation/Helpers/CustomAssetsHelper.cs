using Application.Config;
using Application.Helpers;
using System;
using System.Linq;

namespace Presentation.Helpers
{
    public static class CustomAssetsHelper
    {
        public static string HomeBackground(int maxWidth = 1600, int maxHeight = 900)
        {
            return GetImage(MaaSConfig.Current.Layout.Images.Background1, maxWidth, maxHeight);
        }

        public static string HomeBenefitsIcon(int index, int maxWidth = 150, int maxHeight = 150)
        {
            return GetImage(MaaSConfig.Current.HowItWorks.Items[index].ImageUrl, maxWidth, maxHeight);
        }

        public static string Logo(int maxWidth = 500, int maxHeight = 150)
        {
            return GetImage(MaaSConfig.Current.Layout.Images.Logo, maxWidth, maxHeight);
        }

        public static string LogoHome(int maxWidth = 500, int maxHeight = 150)
        {
            return GetImage(MaaSConfig.Current.Layout.Images.LogoHome, maxWidth, maxHeight);
        }

        public static string SellerBenefitsIcon(int index, int maxWidth = 150, int maxHeight = 150)
        {
            return GetImage(MaaSConfig.Current.HelpQuestions.Items[index].ImageUrl, maxWidth, maxHeight);
        }

        #region Helpers

        private static string GetImage(string url, int maxWidth, int maxHeight)
        {
            return IsCloudResource(url)
                ? ImgHelper.BlobToCdn(url, new { maxWidth, maxHeight })
                : url;
        }

        private static bool IsCloudResource(string url)
        {
            return
                url.Contains(".s3.amazonaws.com", StringComparison.OrdinalIgnoreCase) ||
                url.Contains(".blob.core.windows.net", StringComparison.OrdinalIgnoreCase);
        }

        #endregion
    }
}