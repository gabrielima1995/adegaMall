namespace PaymentIntegrator.Core.Suppliers
{
    /// <summary>
    /// Representa um administrador responsável pelo fornecedor/recebedor.
    /// </summary>
    public interface IAccountable
    {
        /// <summary>
        /// Nome completo do administrador.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Número do documento do administrador.
        /// </summary>
        string DocumentNumber { get; }

        /// <summary>
        /// Número do telefone do administrador.
        /// </summary>
        string PhoneNumber { get; }
    }
}