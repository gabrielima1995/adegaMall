using Newtonsoft.Json;

namespace Application.Config.Common
{
    public class PublicRegisterConfig : RegisterConfig
    {
        /// <summary>
        /// Determina se o campo é público ou não. Um campo público é visível para os usuários Comprador e Vendedor (conforme o contexto).
        /// Um campo privado só pode ser visto pelos admininistradores, no painel administrativo.
        /// </summary>
        public bool IsPublic { get; set; } = true;

        /// <summary>
        /// Propriedade automática que representa o contrário de <see cref="IsPublic"/>. Apenas um "syntactic sugar".
        /// </summary>
        [JsonIgnore]
        public bool IsNotPublic => !IsPublic;
    }
}