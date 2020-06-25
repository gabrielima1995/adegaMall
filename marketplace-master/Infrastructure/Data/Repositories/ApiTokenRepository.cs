using Domain.DTO;
using Domain.Entities;
using Domain.Interfaces.Repositories;
using Infrastructure.Data.Context;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace Infrastructure.Data.Repositories
{
    public class ApiTokenRepository : EfCrudRepository<ApiToken>, IApiTokenRepository
    {
        private readonly ISellerRepository _sellerRepository;

        public ApiTokenRepository(
            ScaffoldContext context,
            ISellerRepository sellerRepository)
            : base(context)
        {
            _sellerRepository = sellerRepository;
        }

        public ApiTokenEditing GetForEditing(Guid id)
        {
            var entity = Context.Set<ApiToken>()
                .Include(x => x.Seller)
                .Where(x =>
                    !x.Deleted &&
                    x.Id == id)
                .Select(x => new ApiTokenEditing
                {
                    Name = x.Name,
                    Token = x.Token,
                    SellerId = x.Seller.Id
                })
                .FirstOrDefault();

            if (entity != null)
            {
                entity.Sellers = _sellerRepository.ListForSelector();
            }

            return entity;
        }

        public List<ApiTokenListing> ListByActive(string sellerId = null)
        {
            return Context.Set<ApiToken>()
                .Include(x => x.Seller)
                .Where(x => !x.Deleted)
                .Where(x => sellerId == null || x.Seller.Id == sellerId)
                .Select(x => new ApiTokenListing
                {
                    Id = x.Id,
                    Name = x.Name,
                    Token = x.Token,
                    SellerName = x.Seller.FullName
                })
                .OrderBy(i => i.Name)
                .ToList();
        }

        public ApiToken GetByToken(string token)
        {
            return Context.Set<ApiToken>()
                .FirstOrDefault(x => x.Token == token);
        }
    }
}