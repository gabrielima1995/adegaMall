using Application.Config;
using Domain.Interfaces.Repositories;
using Infra.Crosscutting.Globalization;
using Owin;
using Presentation.Helpers.Modules;
using System;
using System.IO;
using System.Web.Hosting;

namespace Presentation.Configuration
{
    public static class MaasConfigConfigurationExtensions
    {
        private static bool firstTimeCall = true;

        public static IAppBuilder UseMaasConfig(this IAppBuilder app)
        {
            MaaSConfig.Loaded += MaaSConfig_Loaded;
            return app;
        }

        private static void MaaSConfig_Loaded(object sender, EventArgs e)
        {
            var config = MaaSConfig.Current;

            if (firstTimeCall)
            {
                firstTimeCall = false;
                GlobalizationHelper.LoadStrings(UnityConfig.Resolve<IStringRepository>());
            }

            // Creates the SASS file.
            var filePath = HostingEnvironment.MapPath("~/assets/css/variables.sass");
            File.WriteAllText(filePath, config.CreateSassFileContent());

            EmbedHelper.Config.DefaultImageThumbnail = config.Layout.Images.DefaultImageThumbUrl;
            EmbedHelper.Config.DefaultVideoThumbnail = config.Layout.Images.DefaultVideoThumbUrl;

            ImgHelperConfig.Configure();
            ImageInterceptorModule.CloudBaseUrl = config.Environment.CloudStorage.Endpoint;
        }
    }
}