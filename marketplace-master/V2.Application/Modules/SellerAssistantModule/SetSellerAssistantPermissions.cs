using Domain.Core;
using Domain.Entities;
using Domain.Modules.SellerAssistantModule;
using Framework;
using MediatR;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;

namespace V2.Application.Modules.SellerAssistantModule
{
    public class SetSellerAssistantPermissions : IRequest<OperationResult>
    {
        public SetSellerAssistantPermissions(string sellerId, IEnumerable<string> permissions)
        {
            SellerId = sellerId;
            Permissions = permissions;
        }

        public string SellerId { get; }
        public IEnumerable<string> Permissions { get; }
    }

    public class SetSellerAssistantPermissionsHandler : IRequestHandler<SetSellerAssistantPermissions, OperationResult>
    {
        private readonly IMediator mediator;
        private readonly UserManager<ApplicationUser> userManager;
        private readonly IDomainEventDispatcher domainEventDispatcher;

        public SetSellerAssistantPermissionsHandler(IMediator mediator, UserManager<ApplicationUser> userManager, IDomainEventDispatcher domainEventDispatcher)
        {
            this.mediator = mediator;
            this.userManager = userManager;
            this.domainEventDispatcher = domainEventDispatcher;
        }

        public async Task<OperationResult> Handle(SetSellerAssistantPermissions request, CancellationToken cancellationToken)
        {
            var addClaims = request.Permissions.Intersect(SellerAssistantPermissions.AllPermissions)
                .Select(permission => new Claim(permission, request.SellerId));

            var removeClaims = SellerAssistantPermissions.AllPermissions.Except(request.Permissions)
                .Select(permission => new Claim(permission, request.SellerId));

            if (addClaims.Any() || removeClaims.Any())
            {
                var (userId, personId) = await GetSellerAssistantUserAndPersonId(request.SellerId);

                var currentClaims = userManager.GetClaims(userId);
                var comparer = new ClaimComparer();

                foreach (var claim in addClaims)
                {
                    if (currentClaims.Contains(claim, comparer))
                        continue;
                    await userManager.AddClaimAsync(userId, claim);
                }

                foreach (var claim in removeClaims)
                {
                    if (!currentClaims.Contains(claim, comparer))
                        continue;
                    await userManager.RemoveClaimAsync(userId, claim);
                }

                var updatedEvent = new SellerAssistantPermissionsSet(userId, personId);
                domainEventDispatcher.Dispatch(updatedEvent);
            }

            return OperationResult.Success;
        }

        private async Task<(string, string)> GetSellerAssistantUserAndPersonId(string sellerId)
        {
            var assistant = await mediator.Send(new GetSellerAssistant(sellerId));
            return (assistant.UserId, assistant.PersonId);
        }

        private class ClaimComparer : IEqualityComparer<Claim>
        {
            public bool Equals(Claim x, Claim y) => x.Type == y.Type && x.Value == y.Value;
            public int GetHashCode(Claim obj) => (obj.Type, obj.Value).GetHashCode();
        }
    }
}
