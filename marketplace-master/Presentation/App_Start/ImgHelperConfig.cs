using Application.Config;
using Application.Helpers;
using System;

namespace Presentation
{
    public static class ImgHelperConfig
    {
        public static void Configure()
        {
            var config = MaaSConfig.Current;

            ImgHelper.Configure(
                storageBaseUrl: config.Environment.CloudStorage.Endpoint,
                imageResizerBaseUrl: config.Environment.AppUrl + ConfigHelper.GetSetting("ImageInterceptor:Prefix", "cloudimage"),
                cdnEndpoint: config.Cdn.Enabled ? config.Cdn.Endpoint : null,
                defaultUserAvatar: config.Layout.Images.DefaultAvatar,
                defaultProductImage: config.Layout.Images.ProductDummyImage);
        }
    }
}