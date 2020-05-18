using Framework;
using PagarMe;
using PagarMe.Base;
using PaymentIntegrator.PagarMe.Common;
using System;

namespace PaymentIntegrator.PagarMe
{
    /// <summary>
    /// Representa o client de conexão com a api do Pagar.me.
    /// </summary>
    public class PagarMeClient
    {
        public PagarMeService GetService()
        {
            return new PagarMeService(PagarMeConfig.ApiKey, PagarMeConfig.EncryptionKey);
        }

        /// <summary>
        /// Envia o modelo para a api do Pagar.me.
        /// </summary>
        /// <param name="model">Dados do modelo.</param>
        /// <returns></returns>
        public OperationResult Save(Model model)
        {
            return Execute(model.Save);
        }

        /// <summary>
        /// Executa uma ação na api do Pagar.me.
        /// </summary>
        /// <param name="action">Ação a ser executada.</param>
        /// <returns></returns>
        public OperationResult Execute(Action action)
        {
            try
            {
                action.Invoke();
            }
            catch (PagarMeException ex)
            {
                return ex.Error.ToValidation();
            }
            catch (Exception ex)
            {
                return new OperationResult("", ex.Message);
            }

            return new OperationResult();
        }
    }
}