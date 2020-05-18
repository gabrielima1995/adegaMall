using System;

namespace PaymentIntegrator.Core.Customers
{
    /// <summary>
    /// Representa um comprador/cliente.
    /// </summary>
    public interface ICustomer
    {
        /// <summary>
        /// Endereço de email.
        /// </summary>
        string Email { get; }

        /// <summary>
        /// Nome do comprador/cliente.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Número do documento.
        /// </summary>
        string DocumentNumber { get; }

        /// <summary>
        /// Número do documento do responsável pela conta bancária
        /// </summary>
        string AccountableDocument { get; }

        /// <summary>
        /// Número do telefone, com DDD.
        /// </summary>
        string PhoneNumber { get; }

        /// <summary>
        /// Identificador do comprador/cliente no gateway de pagamento.
        /// </summary>
        string GatewayId { get; }

        /// <summary>
        /// Identificador do assinante
        /// </summary>
        string SubscriberId { get; }

        /// <summary>
        /// Atribui o identificador do comprador/cliente no gateway de pagamento.
        /// </summary>
        /// <param name="id">Identificador do comprador/cliente no gateway de pagamento.</param>
        void SetGatewayId(string id);

        /// <summary>
        /// Atribui o identificador do usuário como assinante no gateway de pagamento
        /// </summary>
        /// <param name="id"></param>
        void SetSubscriberId(string id);

        /// <summary>
        /// Data de nascimento do comprador/cliente
        /// </summary>
        DateTime? DateOfBirth { get; }
    }
}