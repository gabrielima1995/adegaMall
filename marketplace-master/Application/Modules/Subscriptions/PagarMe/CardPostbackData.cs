namespace Application.Modules.Subscriptions.PagarMe
{
    /// <summary>
    /// Dados do objeto Card no postback enviado pelo Pagar.me.
    /// </summary>
    public class CardPostbackData
    {
        public string id { get; set; }

        public string brand { get; set; }

        public string holder_name { get; set; }

        public string first_digits { get; set; }

        public string last_digits { get; set; }
    }
}