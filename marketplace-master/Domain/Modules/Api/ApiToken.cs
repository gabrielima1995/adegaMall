using Domain.DTO;
using Framework;
using System;

namespace Domain.Entities
{
    public class ApiToken : SoftDeletableEntity<Guid>
    {
        public string Name { get; private set; }

        public string Token { get; private set; }

        public string Seller_Id { get; private set; }

        public Seller Seller { get; private set; }

        public bool IsRevoked => Deleted;

        public bool HasSeller => Seller_Id != null;

        private ApiToken()
        {
            Id = Guid.NewGuid();
        }

        private ApiToken(string name, string sellerId)
            : this()
        {
            Name = name;
            Seller_Id = sellerId;
        }

        private OperationResult<ApiToken> GenerateToken()
        {
            Token = SHA1.GenerateHash(Id.ToString());

            return this;
        }

        private OperationResult<ApiToken> Validate()
        {
            StringHelper.Sanitize(this);

            if (string.IsNullOrWhiteSpace(Name))
            {
                return OperationResult<ApiToken>.Fail("Name", "Name required.");
            }

            return this;
        }

        public static OperationResult<ApiToken> New(string name, string sellerId = null)
        {
            return new ApiToken(name, sellerId)
                .Validate();
        }

        public static OperationResult<ApiToken> Generate(string name, string sellerId = null)
        {
            return New(name, sellerId)
                .Then(i => i.GenerateToken());
        }

        public OperationResult<ApiToken> Update(ApiTokenEditing editing)
        {
            Name = editing.Name;
            Seller_Id = editing.SellerId;

            return Validate();
        }

        public OperationResult<ApiToken> Revoke()
        {
            Deleted = true;
            DeletedAt = DateTime.UtcNow;

            return this;
        }
    }
}