using Dapper;
using Domain.Modules.StoreModule.CompleteProfile;
using Newtonsoft.Json;
using System.Linq;
using V2.Application.Modules.Sellers;
using V2.Infrastructure.Data;

namespace V2.Infrastructure.Modules.SellerProfileModule
{
    public class SellerCompleteProfileQueries : ISellerCompleteProfileQueries
    {
        private readonly SqlDbConnection dbConnection;

        public SellerCompleteProfileQueries(SqlDbConnection dbConnection)
        {
            this.dbConnection = dbConnection;
        }

        public DataForSellerCompleteProfileEvaluationDto GetDataForSeller(string sellerId)
        {
            using (var conn = dbConnection.Connect())
            {
                var sql = @"
                    -- DECLARE @sellerId VARCHAR(128) SET @sellerId = '5591023b-f76d-435f-a91e-739bc52a73b7'

                   SELECT
	                    Avatar,
	                    WhatsAppNumber,
	                    SupplierGatewayId,
						DocumentNumber,
						DateOfBirth,
						Address_Id,
	                    CASE WHEN ISNULL(RequireModeration, 0) = 0 THEN 1 ELSE 0 END IsApprovedInModeration,
	                    CASE WHEN EXISTS (SELECT TOP 1 1 FROM Subscription WHERE SellerId = @sellerId AND Status IN (1, 2)) THEN 1 ELSE 0 END HasActiveSubscription,
	                    ISNULL((SELECT COUNT(1) FROM Product WHERE Store_SellerId = @sellerId AND Deleted = 0), 0) ProductsCount,
	                    ISNULL((SELECT COUNT(1) FROM PortfolioItem WHERE Portfolio_SellerId = @sellerId), 0) PortfolioItemsCount,
	                    (
		                    ISNULL((SELECT COUNT(1) FROM SubCategoryPortfolio WHERE Portfolio_SellerId = @sellerId), 0) +
		                    ISNULL((SELECT COUNT(1) FROM PortfolioItemSubcategories
			                    INNER JOIN PortfolioItem ON PortfolioItemSubcategories.PortfolioItem_Id = PortfolioItem.Id
			                    AND PortfolioItem.Portfolio_SellerId = @sellerId), 0)) PortfolioCategoriesCount,
	                    SellerCompleteProfile CompleteProfileJson
                    FROM Person
                    WHERE Id = @sellerId

					SELECT CustomField.Id, CustomField.Name, CustomFieldValue.Value
                    FROM CustomField
                    LEFT JOIN CustomFieldValue ON CustomField.Id = CustomFieldValue.CustomField_Id
					AND CustomFieldValue.EntityId = @sellerId
                    WHERE CustomField.IsMandatory = 1 AND CustomField.EntityType = 2

					SELECT 
						StartDate AS PlanStartDate,
						Days as PlanDays
					FROM Subscription s 
					JOIN [Plan] p ON p.Id = s.Plan_Id
					WHERE SellerId = @sellerId";

                var grid = conn.QueryMultiple(sql, new { sellerId });

                var sellerAux = grid.ReadSingle<SellerCompleteProfileDtoAux>();
                var pendingCustomFields = grid.Read<DataForSellerCompleteProfileEvaluationDto.PendingCustomField>().ToList();
                var subscription = grid.ReadSingleOrDefault<DataForSellerCompleteProfileEvaluationDto.Subscription>();
                var seller = sellerAux as DataForSellerCompleteProfileEvaluationDto;
                seller.PendingCustomFields = pendingCustomFields;
                if (subscription != null)
                {
                    seller.PlanDays = subscription.PlanDays;
                    seller.PlanStartDate = subscription.PlanStartDate.GetValueOrDefault().ToUniversalTime();
                }
                return seller;
            }
        }

        private class SellerCompleteProfileDtoAux : DataForSellerCompleteProfileEvaluationDto
        {
            public string CompleteProfileJson
            {
                set => CompleteProfile = JsonConvert.DeserializeObject<SellerCompleteProfile>(value);
            }
        }
    }
}
