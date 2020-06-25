namespace Application.Config.General.Layout
{
    public class TransactionWarrantyDisplayConfig
    {
        private bool showReassuringMessages = true;

        public bool ShowReassuringMessages
        {
            get
            {
                if (MaaSConfig.Current != null && !MaaSConfig.Current.Payment.Common.EnableTransactionWarranty)
                    return false;

                return showReassuringMessages;
            }
            set
            {
                showReassuringMessages = value;
            }
        }
    }
}