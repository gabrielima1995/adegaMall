using System;

namespace PaymentIntegrator.Core.Suppliers
{
    /// <summary>
    /// Representa um fornecedor/recebedor.
    /// </summary>
    public interface ISupplier
    {
        /// <summary>
        /// Nome completo do fornecedor/recebedor.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Nome alternativo do fornecedor/recebedor.
        /// </summary>
        string AlternativeName { get; }

        /// <summary>
        /// Endereço de email.
        /// </summary>
        string Email { get; }

        /// <summary>
        /// Descrição do negócio da empresa do fornecedor/recebedor.
        /// </summary>
        string BusinessDescription { get; }

        /// <summary>
        /// Número do documento.
        /// </summary>
        string DocumentNumber { get; }

        /// <summary>
        /// Telefone, com DDD.
        /// </summary>
        string Phone { get; }

        /// <summary>
        /// Identificador do fornecedor/recebedor no gateway de pagamento.
        /// </summary>
        string GatewayId { get; }

        /// <summary>
        /// Live token do fornecedor/recebedor (Iugu).
        /// </summary>
        string GatewayLiveApiToken { get; }

        /// <summary>
        /// Test token do fornecedor/recebedor (Iugu).
        /// </summary>
        string GatewayTestApiToken { get; }

        /// <summary>
        /// User token do fornecedor/recebedor (Iugu).
        /// </summary>
        string GatewayUserToken { get; }

        /// <summary>
        /// Data de nascimento do vendedor
        /// </summary>
        DateTime? DateOfBirth { get; }

        /// <summary>
        /// Indica se fornecedor/recebedor é pessoa jurídica.
        /// </summary>
        /// <returns>Verdadeiro se fornecedor/recebedor for pessoa jurídica.</returns>
        bool IsCompany();

        /// <summary>
        /// Atribui os valores de identificação do fornecedor/recebedor no gateway de pagamento.
        /// </summary>
        /// <param name="id">Identificador do fornecedor/recebedor no gateway de pagamento.</param>
        /// <param name="liveApiToken">Live token do fornecedor/recebedor (Iugu).</param>
        /// <param name="testApiToken">Test token do fornecedor/recebedor (Iugu).</param>
        /// <param name="userToken">User token do fornecedor/recebedor (Iugu).</param>
        void SetGatewayTokens(string id, string liveApiToken, string testApiToken, string userToken);
    }
}