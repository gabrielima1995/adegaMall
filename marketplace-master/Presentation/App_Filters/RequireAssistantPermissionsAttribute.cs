using Domain.DTO;
using Domain.Entities;
using Domain.Interfaces;
using Microsoft.AspNet.Identity;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;

namespace System.Web.Mvc
{
    /// <summary>
    /// No caso de um usuário SellerAssistant, verifica se atende as permissões necessárias.
    /// Deve ser usado em conjunto com outros atributos de autorização.
    /// </summary>
    public sealed class RequireAssistantPermissionsAttribute : AuthorizeEnabledAttribute
    {
        /// <summary>
        /// No caso de um usuário SellerAssistant, verifica se atende as permissões necessárias.
        /// Deve ser usado em conjunto com outros atributos de autorização.
        /// </summary>
        /// <param name="requiredAssistantClaims">Permissões que habilitam o acesso em caso de Role = SellerAssistant, separadas por vírgula. Basta que o assistente tenha uma das permissões indicadas. Veja mais em <see cref="V2.Application.Modules.SellerAssistantModule.SellerAssistantPermissions"/>.</param>
        public RequireAssistantPermissionsAttribute(string requiredAssistantClaims)
        {
            RequiredAssistantClaims = new List<string>(requiredAssistantClaims.Split(',').Select(str => str.Trim()));
        }

        /// <summary>
        /// A permissão necessária, no caso de Role = SellerAssistant. Veja mais em <see cref="V2.Application.Modules.SellerAssistantModule.SellerAssistantPermissions"/>.
        /// </summary>
        public List<string> RequiredAssistantClaims { get; set; }

        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            if (!base.AuthorizeCore(httpContext))
                return false;

            var authProvider = DependencyResolver.Current.GetService<IUserAuthProvider>();
            var loginProfile = authProvider.GetLoginProfile();

            if (loginProfile.IsSellerAssistant())
            {
                var userId = loginProfile.AssistantUserId;
                var userManager = DependencyResolver.Current.GetService<UserManager<ApplicationUser>>();
                var claimsTheUserHas = userManager.GetClaims(userId);

                var requiredClaims = RequiredAssistantClaims.Select(c => new Claim(c, loginProfile.PersonId));
                return claimsTheUserHas.Intersect(requiredClaims, new ClaimComparer()).Any();
            }

            return true;
        }

        private class ClaimComparer : IEqualityComparer<Claim>
        {
            public bool Equals(Claim x, Claim y) => x.Type == y.Type && x.Value == y.Value;
            public int GetHashCode(Claim obj) => (obj.Type, obj.Value).GetHashCode();
        }
    }
}