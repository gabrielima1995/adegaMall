namespace Domain.Entities.StoreModule
{
    /// <summary>
    /// Enumera os possíveis tipos de transação de locação de produtos.
    /// </summary>
    public enum RentalTransactionType
    {
        /// <summary>
        /// Quando o produto de locação é entregue para o cliente.
        /// </summary>
        Delivery = 0,

        /// <summary>
        /// Quando o produto de locação é retornado para a loja.
        /// </summary>
        Returned = 1
    }
}