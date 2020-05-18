using CSharpFunctionalExtensions;
using Dapper;
using NewContext.Core;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace NewContext.Queries.Sellers
{
    public class ListSellersForApi : IQuery<ListSellersForApi.ResultDto>
    {
        public ListSellersForApi(Paging paging, bool? enabled, bool? pendingModeration, string documentNumber, string gatewayId)
        {
            Paging = paging;
            Enabled = enabled;
            PendingModeration = pendingModeration;
            DocumentNumber = documentNumber;
            GatewayId = gatewayId;
        }

        public Paging Paging { get; }
        public bool? Enabled { get; }
        public bool? PendingModeration { get; }
        public string DocumentNumber { get; }
        public string GatewayId { get; }

        public class ResultDto
        {
            public ResultDto(IReadOnlyCollection<SellerDto> sellers, int totalCount)
            {
                Sellers = sellers;
                TotalCount = totalCount;
            }

            public IReadOnlyCollection<SellerDto> Sellers { get; }
            public int TotalCount { get; }
        }

        public class SellerDto
        {
            internal string IdString { get; set; }

            public Guid Id { get; internal set; }
            public string FullName { get; internal set; }
            public string DocumentNumber { get; internal set; }
            public string PublicProfileName { get; internal set; }
            public string Email { get; internal set; }
            public string ProfileImage { get; internal set; }
            public string ProfilePath { get; internal set; }
            public DateTime? BirthDate { get; internal set; }
            public string Genre { get; internal set; }
            public string AdditionalInfo1 { get; internal set; }
            public string AdditionalInfo2 { get; internal set; }
            public string AboutMe { get; internal set; }
            public DateTime RegisteredAt { get; internal set; }
            public bool Enabled { get; internal set; }
            public bool PendingModeration { get; internal set; }
            public string GatewayId { get; internal set; }
            public AddressDto Address { get; internal set; }
            public IReadOnlyCollection<CustomFieldDto> CustomFields { get; internal set; } = new List<CustomFieldDto>(0);
        }

        public class AddressDto
        {
            public string Street { get; internal set; }
            public string Number { get; internal set; }
            public string Complement { get; internal set; }
            public string Neighborhood { get; internal set; }
            public string CityState { get; internal set; }
            public string ZipCode { get; internal set; }
        }

        public class CustomFieldDto
        {
            internal Guid SellerId { get; set; }

            public Guid Id { get; internal set; }
            public string Value { get; internal set; }
        }
    }

    public class ListSellersForApiHandler : IQueryHandler<ListSellersForApi, ListSellersForApi.ResultDto>
    {
        private readonly QueriesConnectionString connectionString;

        public ListSellersForApiHandler(QueriesConnectionString connectionString)
        {
            this.connectionString = connectionString;
        }

        public async Task<ListSellersForApi.ResultDto> HandleAsync(ListSellersForApi query)
        {
            var queryParameters = new
            {
                start = query.Paging.Start,
                limit = query.Paging.Limit,
                enabled = query.Enabled,
                pendingModeration = query.PendingModeration,
                documentNumber = query.DocumentNumber,
                gatewayId = query.GatewayId
            };

            using (var connection = new SqlConnection(connectionString.Value))
            using (var grid = await connection.QueryMultipleAsync(sql, queryParameters))
            {
                var totalCount = await grid.ReadSingleAsync<int>();

                var sellers = grid.Read<ListSellersForApi.SellerDto, ListSellersForApi.AddressDto, ListSellersForApi.SellerDto>(
                    (seller, address) =>
                    {
                        seller.Id = Guid.Parse(seller.IdString);
                        seller.Address = address;
                        return seller;
                    },
                    splitOn: "Street");

                var customFields = await grid.ReadAsync<ListSellersForApi.CustomFieldDto>();

                foreach (var seller in sellers)
                    seller.CustomFields = customFields
                        .Where(field => field.SellerId == seller.Id)
                        .ToList().AsReadOnly();

                return new ListSellersForApi.ResultDto(sellers.ToList(), totalCount);
            }
        }

        private const string sql = @"
            /*
            DECLARE @start INT SET @start = 0
            DECLARE @limit INT SET @limit = 20
            DECLARE @enabled BIT
            DECLARE @pendingModeration BIT
            DECLARE @documentNumber VARCHAR(30)
            DECLARE @gatewayId VARCHAR(30)
            */

            DROP TABLE IF EXISTS #tmpSellerIds

            SELECT COUNT(1) TotalCount
            FROM Person
            WHERE Discriminator = 'Seller'
                AND (@enabled IS NULL OR [Enabled] = @enabled)
                AND (@pendingModeration IS NULL OR RequireModeration = @pendingModeration)
                AND (@documentNumber IS NULL OR DocumentNumber = @documentNumber)
                AND (@gatewayId IS NULL OR SupplierGatewayId = @gatewayId)

            SELECT Id
            INTO #tmpSellerIds
            FROM Person Seller
            WHERE Discriminator = 'Seller'
                AND (@enabled IS NULL OR [Enabled] = @enabled)
                AND (@pendingModeration IS NULL OR RequireModeration = @pendingModeration)
                AND (@documentNumber IS NULL OR DocumentNumber = @documentNumber)
                AND (@gatewayId IS NULL OR SupplierGatewayId = @gatewayId)
            ORDER BY Seller.CreatedAt
            OFFSET @start ROWS
            FETCH NEXT @limit ROWS ONLY

            SELECT
                Seller.Id IdString,
                Seller.FullName,
                Seller.DocumentNumber,
                Seller.AlternativeName PublicProfileName,
                Seller.Email,
                Seller.PhoneNumber,
                Seller.Avatar ProfileImage,
                CASE
                    WHEN Seller.FriendlyUrl IS NULL THEN NULL
                    ELSE '/p/' + Seller.FriendlyUrl
                    END ProfilePath,
                Seller.DateOfBirth BirthDate,
                CASE
                    WHEN Seller.Genre = 0 THEN 'male'
                    WHEN Seller.Genre = 1 THEN 'female'
                    WHEN Seller.Genre = 2 THEN 'other'
                    END Genre,
                Seller.AdditionalInfo1,
                Seller.AdditionalInfo2,
                Seller.AboutMe,
                Seller.CreatedAt RegisteredAt,
                Seller.[Enabled],
                ISNULL(Seller.RequireModeration, 0) PendingModeration,
                Seller.SupplierGatewayId GatewayId,
                Addr.Street Street,
                Addr.Number Number,
                Addr.AdditionalInfo Complement,
                Addr.Neighborhood Neighborhood,
                Addr.CityAndState CityState,
                Addr.ZipCode ZipCode
            FROM Person Seller
            INNER JOIN #tmpSellerIds ON Seller.Id = #tmpSellerIds.Id
            LEFT JOIN [Address] Addr ON Seller.Address_Id = Addr.Id

            SELECT
                CustomFieldValue.EntityId SellerId,
                CustomFieldValue.CustomField_Id Id,
                CustomFieldValue.[Value]
            FROM CustomFieldValue
            INNER JOIN #tmpSellerIds ON CustomFieldValue.EntityId = #tmpSellerIds.Id
            INNER JOIN CustomField ON CustomFieldValue.CustomField_Id = CustomField.Id

            DROP TABLE #tmpSellerIds";
    }
}
