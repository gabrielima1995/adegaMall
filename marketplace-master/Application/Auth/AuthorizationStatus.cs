namespace Application.Auth
{
    /// <summary>
    /// Status da autorização.
    /// </summary>
    public enum AuthorizationStatus
    {
        /// <summary>
        /// Autorização permitida.
        /// </summary>
        OK = 200,

        /// <summary>
        /// Autorização inexistente ou inválida.
        /// </summary>
        Unauthorized = 401,

        /// <summary>
        /// Sem autorização.
        /// </summary>
        Forbidden = 403
    }
}