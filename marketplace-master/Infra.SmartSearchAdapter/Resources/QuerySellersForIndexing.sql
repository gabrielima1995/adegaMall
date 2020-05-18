/*
DECLARE @sellerId VARCHAR(128) SET @sellerId = NULL --'680df253-acf9-400d-b84f-d47d2b825bb5'
DECLARE @checkHasCategories BIT SET @checkHasCategories = 1
DECLARE @checkHasPortfolio BIT SET @checkHasPortfolio = 1
DECLARE @checkHasServicesOrProducts BIT SET @checkHasServicesOrProducts = 1
DECLARE @calculateAvarageRating BIT SET @calculateAvarageRating = 1
*/

;WITH
	cteCategories AS (
		SELECT
			SubCategoryPortfolio.Portfolio_SellerId SellerId,
			Category.Id CategoryId,
			Category.[Name] CategoryName
		FROM SubCategoryPortfolio
			INNER JOIN SubCategory ON SubCategoryPortfolio.SubCategory_Id = SubCategory.Id
			INNER JOIN Category ON SubCategory.Category_Id = Category.Id
		WHERE Category.Deleted = 0 AND SubCategory.Deleted = 0
		UNION
		SELECT
			PortfolioItem.Portfolio_SellerId SellerId,
			Category.Id CategoryId,
			Category.[Name] CategoryName
		FROM PortfolioItemSubcategories
			INNER JOIN SubCategory ON PortfolioItemSubcategories.SubCategory_Id = SubCategory.Id
			INNER JOIN Category ON SubCategory.Category_Id = Category.Id
			INNER JOIN PortfolioItem ON PortfolioItemSubcategories.PortfolioItem_Id =  PortfolioItem.Id
		WHERE Category.Deleted = 0 AND SubCategory.Deleted = 0),

	cteSubCategories AS (
		SELECT
			SubCategoryPortfolio.Portfolio_SellerId SellerId,
			SubCategory.Id SubCategoryId,
			SubCategory.[Name] SubCategoryName
		FROM SubCategoryPortfolio
			INNER JOIN SubCategory ON SubCategoryPortfolio.SubCategory_Id = SubCategory.Id
			INNER JOIN Category ON SubCategory.Category_Id = Category.Id
		WHERE Category.Deleted = 0 AND SubCategory.Deleted = 0
		UNION
		SELECT
			PortfolioItem.Portfolio_SellerId SellerId,
			PortfolioItemSubcategories.SubCategory_Id SubCategoryId,
			SubCategory.[Name] SubCategoryName
		FROM PortfolioItemSubcategories
			INNER JOIN SubCategory ON PortfolioItemSubcategories.SubCategory_Id = SubCategory.Id
			INNER JOIN Category ON SubCategory.Category_Id = Category.Id
			INNER JOIN PortfolioItem ON PortfolioItemSubcategories.PortfolioItem_Id =  PortfolioItem.Id
		WHERE Category.Deleted = 0 AND SubCategory.Deleted = 0),

	cteSellersWithAggregatedData AS (
		SELECT
			Sellers.Id,
			Sellers.AlternativeName,
			Sellers.FullName,
			Sellers.Nickname,
			Sellers.AboutMe,
			Sellers.AdditionalInfo2,
			Sellers.Avatar,
			Sellers.CreatedAt,
			Sellers.[Enabled],
			Sellers.FriendlyUrl,
			Sellers.Email,
			Sellers.SupplierGatewayId,
			Sellers.SellerCompleteProfile,

			(SELECT STRING_AGG(CategoryId, '||')
				FROM cteCategories
				WHERE SellerId = Sellers.Id) AS CategoryIds,

			(SELECT STRING_AGG(CategoryName, '||')
				FROM cteCategories
				WHERE SellerId = Sellers.Id) AS CategoryNames,

			(SELECT STRING_AGG(SubCategoryId, '||')
				FROM cteSubCategories
				WHERE SellerId = Sellers.Id) AS SubCategoryIds,

			(SELECT STRING_AGG(SubCategoryName, '||')
				FROM cteSubCategories
				WHERE SellerId = Sellers.Id) AS SubCategoryNames,

			Subscription.[Status] SubscriptionStatus,
			Subscription.StartDate SubscriptionStartDate,
			Subscription.FinishDate SubscriptionFinishDate,
			Subscription.Plan_Id SubscriptionPlanId,
			Subscription.Amount SubscriptionAmount,
			Subscription.Currency SubscriptionCurrency,
			Subscription.PostbackUrl SubscriptionPostbackUrl,

			Addr.Id AddressId,
			Addr.Street AddressStreet,
			Addr.Number AddressNumber,
			Addr.AdditionalInfo AddressAdditionalInfo,
			Addr.Neighborhood AddressNeighborhood,
			Addr.CityAndState AddressCityAndState,
			Addr.ZipCode AddressZipCode,
			CASE
				WHEN Addr.[Location] IS NULL THEN NULL
				ELSE FORMAT(Addr.[Location].Lat, 'N8', 'en-US') + '|' + FORMAT(Addr.[Location].Long, 'N8', 'en-US')
				END AddressLatLng,

			CONVERT(BIT, (CASE
				WHEN Sellers.Avatar IS NOT NULL THEN 1
				ELSE 0 END)) HasAvatar,
			CONVERT(BIT, (CASE
				WHEN
					Subscription.SellerId IS NOT NULL AND
					Subscription.[Status] IN (1 /*trial*/, 2 /*paid*/, 3 /*pendingpayment*/) THEN 1
				ELSE 0 END)) HasActiveSubscription,

			(SELECT COUNT(1)
				FROM Quote
				WHERE Seller_Id = Sellers.Id) QuotesCount,
			(SELECT COUNT(1)
				FROM [Order]
				WHERE Seller_Id = Sellers.Id) OrdersCount,
			CASE
				WHEN @checkHasCategories = 0 THEN 0
				ELSE (
					SELECT COUNT(1)
					FROM cteCategories
					WHERE SellerId = Sellers.Id)
				END CategoriesCount,
			CASE
				WHEN @checkHasPortfolio = 0 THEN 0
				ELSE (
					SELECT COUNT(1)
					FROM PortfolioItem
					WHERE PortfolioItem.Portfolio_SellerId = Sellers.Id)
				END PortfolioItemsCount,
			CASE
				WHEN @checkHasServicesOrProducts = 0 THEN 0
				ELSE (
					SELECT COUNT(1)
					FROM Product
					WHERE Product.Store_SellerId = Sellers.Id
						AND Product.Deleted = 0
						AND Product.CurrentStock > 0)
				END ServicesOrProductsCount,
			CONVERT(BIT, (CASE
				WHEN Sellers.SupplierGatewayId IS NOT NULL THEN 1 ELSE 0
				END)) HasPaymentGatewayAccount,
			CONVERT(BIT, (CASE
				WHEN ISNULL(Sellers.RequireModeration, 0) = 1 THEN 0 ELSE 1
				END)) HasBeenApprovedInModeration,
			CASE
				WHEN @calculateAvarageRating = 0 THEN 0.0
				ELSE ISNULL((
					SELECT AVG(CONVERT(FLOAT, Rating))
					FROM UserRating
					WHERE UserRating.RatingTargetId = Sellers.Id), 0.0)
				END AverageRating
	
		FROM Person Sellers
			LEFT JOIN [Address] Addr ON Sellers.Address_Id = Addr.Id
			LEFT JOIN Subscription ON Sellers.Id = Subscription.SellerId
		WHERE
			Sellers.Discriminator = 'Seller' AND
			(@sellerId IS NULL OR Sellers.Id = @sellerId))

SELECT
	Id,
	AlternativeName,
	FullName,
	Nickname,
    AboutMe,
    AdditionalInfo2,
    Avatar,
    CreatedAt,
    [Enabled],
    FriendlyUrl,
    Email,
    SupplierGatewayId,
	SellerCompleteProfile,

	CategoryIds,
	CategoryNames,
	SubCategoryIds,
	SubCategoryNames,

	SubscriptionStatus,
	SubscriptionStartDate,
	SubscriptionFinishDate,
	SubscriptionPlanId,
	SubscriptionAmount,
	SubscriptionCurrency,
	SubscriptionPostbackUrl,

	AddressId,
	AddressStreet,
	AddressNumber,
	AddressAdditionalInfo,
	AddressNeighborhood,
	AddressCityAndState,
	AddressZipCode,
	AddressLatLng,

	AverageRating,
	CONVERT(INT, ServicesOrProductsCount + PortfolioItemsCount + AverageRating * 1000) Score,
	
	HasAvatar,
	HasActiveSubscription,
	CONVERT(BIT, (CASE WHEN CategoriesCount > 0 THEN 1 ELSE 0 END)) HasAnyCategories,
	CONVERT(BIT, (CASE WHEN PortfolioItemsCount > 0 THEN 1 ELSE 0 END)) HasPortfolio,
	CONVERT(BIT, (CASE WHEN ServicesOrProductsCount > 0 THEN 1 ELSE 0 END)) HasServicesOrProducts,
	HasPaymentGatewayAccount,
	HasBeenApprovedInModeration
	
FROM cteSellersWithAggregatedData