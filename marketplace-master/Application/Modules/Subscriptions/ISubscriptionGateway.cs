using Application.CreditCards;
using Domain.Entities.SubscriptionModule;
using Framework;

namespace Application.Modules.Subscriptions
{
    /// <summary>
    /// Representa operações de assinatura para um gateway de pagamento.
    /// </summary>
    public interface ISubscriptionGateway
    {
        /// <summary>
        /// Cria uma nova assinatura para um usuário, no gateway de pagamento.
        /// </summary>
        /// <param name="personId">Identificador da pessoa assinante (ex vendedor).</param>
        /// <param name="plan">Dados do plano.</param>
        /// <param name="creditCard">Dados de pagamento.</param>
        /// <returns></returns>
        OperationResult<Subscription> Subscribe(string personId, Plan plan, CreditCardModel creditCard);

        /// <summary>
        /// Cancela a assinatura de um usuário, no gateway de pagamento.
        /// </summary>
        /// <param name="personId">Identificador da pessoa assinante (ex vendedor).</param>
        /// <returns></returns>
        OperationResult<Subscription> Unsubscribe(string personId);

        /// <summary>
        /// Url da aplicação que irá receber os dados do gateway de pagamento (postback), a cada mudança de status da assinatura.
        /// </summary>
        string NotifyUrl { get; }

        /// <summary>
        /// Recebimento de dados por postback quando há alguma alteração no status da assinatura da pagarme e que devem ser refletidos na nossa plataforma.
        /// </summary>
        /// <param name="postbackQuery">String dos dados retornados por postback pela pagarme</param>
        /// <returns></returns>
        OperationResult MapToProcessPostback(string postbackQuery);
    }
}