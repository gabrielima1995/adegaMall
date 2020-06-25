using Domain.DTO;
using Domain.Entities;
using System;
using System.Collections.Generic;

namespace Domain.Interfaces.Repositories
{
    public interface IApiTokenRepository : ICrudRepository<ApiToken>
    {
        ApiTokenEditing GetForEditing(Guid id);

        List<ApiTokenListing> ListByActive(string sellerId = null);

        ApiToken GetByToken(string token);
    }
}