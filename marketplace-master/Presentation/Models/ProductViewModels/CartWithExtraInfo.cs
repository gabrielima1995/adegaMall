using Application.Config;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Presentation.Models.ProductViewModels
{
    public class CartWithExtraInfo
    {
        public CartWithExtraInfo()
        {
        }

        public CartWithExtraInfo(string extraInfo)
        {
            ExtraInfo = extraInfo;
        }

        [DisplayNameLocalized("ORDER_EXTRA_INFO_TITLE")]
        [RequiredIfLocalized("ExtraInfoRequired")]
        public string ExtraInfo { get; set; }

        public bool ExtraInfoRequired => MaaSConfig.Current.Product.Order.ExtraInfoField.IsRequired;
    }
}