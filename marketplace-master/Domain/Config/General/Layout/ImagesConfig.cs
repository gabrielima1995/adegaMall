using System.Collections.Generic;

namespace Application.Config.General.Layout
{
    public class ImagesConfig
    {
        public string Favicon { get; set; } = "/favicon.ico";

        public string Logo { get; set; } = "/assets/images/logo.png";

        public string LogoHome { get; set; } = "/assets/images/logo.png";

        public string PoweredBy { get; set; } = "/assets/images/desenvolvido-por-ideia-no-ar.png";

        public string DefaultAvatar { get; set; } = "/assets/images/user.png";

        public string ModeratorAvatar { get; set; } = "/assets/images/user.png";

        public string DefaultImageThumbUrl { get; set; } = "/assets/images/default-image-thumbnail.png";

        public string DefaultVideoThumbUrl { get; set; } = "/assets/images/default-video-thumbnail.png";

        public string FacebookSharing { get; set; } = null;

        public string Background1 { get; set; } = "/assets/images/bg1.jpeg";

        public string Background2 { get; set; } = "/assets/images/bg2.jpg";

        public string Background3 { get; set; } = "/assets/images/bg3.jpg";

        public string HomeCallSellers { get; set; } = "/assets/images/call-sellers.svg";

        public string FooterBullet { get; set; } = "/assets/images/footer-bullet.png";

        public string ProductDummyImage { get; set; } = "/assets/images/product-img-placeholder.jpg";

        public string BillingDummyImage { get; set; } = "/assets/images/billing-img-placeholder.png";

        public ExtendedDictionary<string, string> CreateSassVariables()
        {
            return new ExtendedDictionary<string, string>
            {
                { "$bg_image_1", $"url('{Background1}') no-repeat center center" },
                { "$bg_image_2", $"url('{Background2}') no-repeat center center" },
                { "$bg_image_3", $"url('{Background3}') no-repeat center center" },
                { "$footer_bullet", $"'{FooterBullet}'" },
            };
        }
    }
}