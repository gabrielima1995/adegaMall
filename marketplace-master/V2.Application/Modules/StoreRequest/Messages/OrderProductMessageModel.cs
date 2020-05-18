using V2.Application.Messages;

namespace V2.Application.Modules.StoreRequest.Messages
{
    public class OrderProductMessageModel : MessageModel
    {
        public string Item { get; set; }

        public string Quantity { get; set; }

        public string Amount { get; set; }
    }
}