using Framework;
using PaymentIntegrator.Core.Common;

namespace PaymentIntegrator.Core.Suppliers
{
    /// <summary>
    /// Operação de fornecedor/recebedor no gateway de pagamento.
    /// </summary>
    public interface ISupplierOperations
    {
        /// <summary>
        /// Cadastro um fornecedor/recebedor no gateway de pagamento.
        /// </summary>
        /// <param name="supplier">Dados do fornecedor/recebedor.</param>
        /// <param name="address">Dados do endereço do fornecedor/recebedor.</param>
        /// <param name="bankAccount">Dados bancários do fornecedor/recebedor.</param>
        /// <param name="accountable">Dados do administrador responsável pelo fornecedor/recebedor.</param>
        /// <returns></returns>
        OperationResult<SupplierWithBankAccountResult> Create(ISupplier supplier, IAddress address, IBankAccount bankAccount, IAccountable accountable = null, IAddress accountableAddress = null);

        /// <summary>
        /// Obtem a situação do fornecedor/recebedor no gateway de pagamento.
        /// </summary>
        /// <param name="supplier">Dados do fornecedor/recebedor.</param>
        /// <returns></returns>
        OperationResult<ISupplierStatus> GetStatus(ISupplier supplier);

        /// <summary>
        /// Atualiza os dados do fornecedor/recebedor no gateway de pagamento.
        /// </summary>
        /// <param name="supplier">Dados do fornecedor/recebedor.</param>
        /// <returns></returns>
        OperationResult Update(ISupplier supplier);

        /// <summary>
        /// Atualiza os dados bancários do fornecedor/recebedor no gateway de pagamento.
        /// </summary>
        /// <param name="supplier">Dados do fornecedor/recebedor.</param>
        /// <param name="bankAccount">Dados bancários do fornecedor/recebedor.</param>
        /// <param name="accountable">Dados do administrador responsável pelo fornecedor/recebedor.</param>
        /// <returns></returns>
        OperationResult ChangeBankAccount(ISupplier supplier, IBankAccount bankAccount, IAccountable accountable = null);

        /// <summary>
        /// Altera o status da Transferência Automática do fornecedor/recebedor
        /// </summary>
        /// <param name="supplier"></param>
        /// <param name="enabled"></param>
        /// <returns></returns>
        OperationResult SetAutoTransferEnabled(ISupplier supplier, bool enabled);


        /// <summary>
        /// Retorna o saldo disponível para saque de um vendedor no gateway
        /// </summary>
        /// <param name="supplier"></param>
        /// <returns>Retorna o valor em centavos</returns>
        OperationResult<AccountBalance> GetAvailableFunds(ISupplier supplier);


        /// <summary>
        /// Solicita a transferência de fundos do gateway para a conta bancária informada
        /// </summary>
        /// <param name="supplier"></param>
        /// <param name="accountable"></param>
        /// <param name="bankAccount"></param>
        /// <returns></returns>
        OperationResult TransferBalance(ISupplier supplier, IAccountable accountable, IBankAccount bankAccount);

        OperationResult<SupplierWithBankAccountResult> UpdateBankAccountOrCreateNewSupplier(ISupplier supplier, IAddress address, IBankAccount bankAccount, IAccountable accountable = null, IAddress accountableAddress = null);
    }
}