using Application.Config;
using Application.Helpers;
using Domain.Entities.StoreModule;
using System.Linq;
using V2.Application.Modules.Search.Products;

namespace Application.Modules.Products
{
    public enum ProductImageSize
    {
        XS = 150,
        SM = 450,
        MD = 600,
        LG = 1200
    }

    public static class ProductsHelper
    {
        public static string GetImageUrl(string imageUrl, ProductImageSize imageSize = ProductImageSize.MD, string mode = "crop")
        {
            var widthHeight = (int)imageSize;
            imageUrl = imageUrl ?? MaaSConfig.Current.Layout.Images.DefaultImageThumbUrl;
            return ImgHelper.Get(imageUrl, widthHeight, widthHeight, mode: mode);
        }

        public static string GetCoverImageUrl(this Product product, ProductImageSize imageSize = ProductImageSize.MD, string mode = "crop")
        {
            var blobImageUrl = product.CoverImageUrl ?? (product.Images == null || !product.Images.Any()
                ? MaaSConfig.Current.Layout.Images.DefaultImageThumbUrl
                : product.Images.First().Url);

            return GetImageUrl(blobImageUrl, imageSize, mode);
        }

        public static string GetCoverImageUrl(this ProductSearchResultItem product, ProductImageSize imageSize = ProductImageSize.MD, string mode = "crop")
        {
            var blobImageUrl = product.CoverImageUrl ?? (product.Images == null || !product.Images.Any()
                                   ? MaaSConfig.Current.Layout.Images.DefaultImageThumbUrl
                                   : product.Images.First());

            return GetImageUrl(blobImageUrl, imageSize, mode);
        }

        public static string GetDimensions(this Product product)
        {
            var str = "";
            var flag = false;
            var config = MaaSConfig.Current;

            if (config.Product.Length.Enabled && product.Length.HasValue)
            {
                str += product.Length + " cm";
                flag = true;
            }

            if (config.Product.Width.Enabled && product.Width.HasValue)
            {
                if (flag)
                    str += " x ";

                str += product.Width + " cm";
                flag = true;
            }

            if (config.Product.Height.Enabled && product.Height.HasValue)
            {
                if (flag)
                    str += " x ";

                str += product.Height + " cm";
                flag = true;
            }

            return str;
        }
    }
}