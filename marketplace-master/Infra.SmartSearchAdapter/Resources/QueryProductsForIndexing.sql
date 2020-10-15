--DECLARE @shippingPerStore BIT = 1;
--DECLARE @sellerId varchar(40) = null
--DECLARE @productId varchar(40) = null

;WITH
    cteImages AS (
        SELECT 
            Product_Id [ProductId],
            [Url],
            [Position]
        FROM ProductImage
        WHERE (@productId IS NULL OR Product_Id = @productId) AND Product_Id IS NOT NULL
        ORDER BY Position
        OFFSET 0 ROWS
    ),

    ctePrivateFleet AS (
        SELECT 
            Product_Id [ProductId],
            DeliveryRange
        FROM PrivateFleetParameter
        WHERE (@productId IS NULL OR Product_Id = @productId) AND Product_Id IS NOT NULL
        ORDER BY DeliveryRange
        OFFSET 0 ROWS
    ),

    cteSingleFreight AS (
        SELECT 
            Store_SellerId [SellerId],
            DeliveryRange
        FROM SingleFreightParameter
        WHERE (@sellerId IS NULL OR Store_SellerId = @sellerId) AND Store_SellerId IS NOT NULL
        ORDER BY DeliveryRange
        OFFSET 0 ROWS
    ),

    cteVariants AS (
        SELECT 
            ProductVariantProduct.Product_Id [ProductId],
            ProductVariant.Id [VariantId],
            ProductVariant.Name [VariantName],
            ProductVariant.Slug [VariantSlug]
        FROM ProductVariantProduct
            INNER JOIN ProductVariant ON ProductVariant.Id = ProductVariantProduct.ProductVariant_Id
        WHERE (@productId IS NULL OR Product_Id = @productId) AND ProductVariant.Deleted = 0
        ORDER BY [Order], [Name]
        OFFSET 0 ROWS),

    cteVariantsType AS (
        SELECT DISTINCT
            ProductVariantProduct.Product_Id [ProductId],
            ProductVariantType.Id [VariantTypeId],
            ProductVariantType.Name [VariantTypeName],
            ProductVariantType.[Order]
        FROM ProductVariantProduct
            INNER JOIN ProductVariant ON ProductVariant.Id = ProductVariantProduct.ProductVariant_Id
            INNER JOIN ProductVariantType ON ProductVariantType.Id = ProductVariant.[Type_Id]
        WHERE (@productId IS NULL OR Product_Id = @productId) AND ProductVariant.Deleted = 0 AND ProductVariantType.Deleted = 0
        ORDER BY ProductVariantType.[Order], ProductVariantType.[Name]
        OFFSET 0 ROWS),

    cteBestPriceProductsTemplate AS(
        select Id,
            BasePrice,
            ProductTemplateId,
            row_number() over(partition by ProductTemplateId order by BasePrice) rn
        from Product
        where ProductTemplateId is not null
        group by ProductTemplateId, BasePrice, Id),

    cteProductsWithAggregatedData AS (
        SELECT
            Products.Id,
            Products.Title,
            Products.Description,
            Products.Specifications,
            Products.CustomId,
            Products.CoverImageUrl,
            Products.FriendlyUrl,
            Products.CreatedAt,
            Products.UpdatedAt,
            Products.ProductPrice,
            Products.BasePrice,
            Products.IsDisabled,

            CONVERT(BIT, (CASE
                WHEN Products.CurrentStock > 0 OR Products.NoShippingEnabled = 1 THEN 1 ELSE 0
                END)) HasStock,
            CONVERT(BIT, (CASE
                WHEN Products.ProductTemplateId IS NOT NULL THEN 1 ELSE 0
                END)) HasTemplate,
            CONVERT(BIT, (CASE
                WHEN cteBestPrice.Id IS NOT NULL THEN 1 ELSE 0
                END)) BestPrice,
            Products.ProductTemplateId TemplateId,

            Products.NoShippingEnabled,
            Products.ActualNoShippingEnabled,
            (CASE WHEN @shippingPerStore = 1
                THEN Store.OnSitePickupEnabled
                ELSE Products.OnSitePickupEnabled END) OnSitePickupEnabled,
            (CASE WHEN @shippingPerStore = 1
                THEN Store.SedexEnabled
                ELSE Products.SedexEnabled END) SedexEnabled,
            (CASE WHEN @shippingPerStore = 1
                THEN Store.PACEnabled
                ELSE Products.PACEnabled END) PACEnabled,
            (CASE WHEN @shippingPerStore = 1
                THEN Store.PrivateFleetEnabled
                ELSE Products.PrivateFleetEnabled END) PrivateFleetEnabled,

            Categ.Id [CategoryId],
            Categ.Name [CategoryName],				
            Categ.Slug [CategorySlug],		

            Seller.Id [SellerId],
            Seller.Nickname [SellerNickname],
            ISNULL(Seller.AlternativeName, Seller.FullName) [SellerProfileName],
            Seller.FriendlyUrl [SellerFriendlyUrl],
            Seller.CreatedAt [SellerCreatedAt],

            (SELECT STRING_AGG([Url], '||')
                FROM cteImages
                WHERE ProductId = Products.Id) AS ImageUrls,

            (SELECT STRING_AGG(DeliveryRange, '||')
                FROM ctePrivateFleet
                WHERE ProductId = Products.Id) AS PrivateFleetRanges,

            (SELECT STRING_AGG(DeliveryRange, '||')
                FROM cteSingleFreight
                WHERE SellerId = Seller.Id) AS SingleFreightRanges,

            (SELECT STRING_AGG(VariantTypeId, '||')
                FROM cteVariantsType
                WHERE ProductId = Products.Id) AS VariantTypeIds,

            (SELECT STRING_AGG(VariantTypeName, '||')
                FROM cteVariantsType
                WHERE ProductId = Products.Id) AS VariantTypeNames,

            (SELECT STRING_AGG(VariantId, '||')
                FROM cteVariants
                WHERE ProductId = Products.Id) AS VariantIds,

            (SELECT STRING_AGG(VariantName, '||')
                FROM cteVariants
                WHERE ProductId = Products.Id) AS VariantNames,

            (SELECT STRING_AGG(VariantSlug, '||')
                FROM cteVariants
                WHERE ProductId = Products.Id) AS VariantSlugs,

            ISNULL(
                (SELECT COUNT(1)
                FROM cteVariants
                WHERE ProductId = Products.Id), 0) VariantsCount,

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
                WHEN
                    Subscription.SellerId IS NOT NULL AND
                    Subscription.[Status] IN (1 /*trial*/, 2 /*paid*/) THEN 1
                ELSE 0 END)) HasActiveSubscription,
            
            CONVERT(BIT, (CASE
                WHEN Seller.SupplierGatewayId IS NOT NULL THEN 1 ELSE 0
                END)) HasPaymentGatewayAccount,

            CONVERT(BIT, (CASE
                WHEN ISNULL(Seller.RequireModeration, 0) = 1 THEN 0 ELSE 1
                END)) HasBeenApprovedInModeration,

            ISNULL(
                (SELECT AVG(CONVERT(FLOAT, Rating))
                FROM UserRating
                WHERE UserRating.RatingTargetId = Seller.Id), 0.0) AverageRating,
            
            CASE WHEN 
                    Seller.[Enabled] = 1 AND 
                    Products.Deleted = 0 AND
                    Products.IsDisabled = 0 AND
                    (Products.IsBillingProduct IS NULL OR Products.IsBillingProduct = 0)AND
                    (Products.ProductTemplateId IS NULL OR cteBestPrice.Id IS NOT NULL)
                THEN 'add' ELSE 'del' END operation
        FROM Product Products 
            INNER JOIN ProductCategory Categ ON Categ.Id = Products.Category_Id
            INNER JOIN Store ON Store.SellerId = Products.Store_SellerId
            INNER JOIN Person Seller ON Seller.Id = Store.SellerId
            LEFT JOIN [Address] Addr ON Seller.Address_Id = Addr.Id
            LEFT JOIN Subscription ON Seller.Id = Subscription.SellerId
            LEFT JOIN cteBestPriceProductsTemplate cteBestPrice ON 
                Products.ProductTemplateId IS NOT NULL AND 
                cteBestPrice.Id = Products.Id AND
                cteBestPrice.rn = 1
        WHERE
            (@productId IS NULL OR Products.Id = @productId) AND
            (@sellerId IS NULL OR Seller.Id = @sellerId))

SELECT
    Id,
    Title,
    Description,
    Specifications,
    CustomId,
    CoverImageUrl,
    FriendlyUrl,
    CreatedAt,
    UpdatedAt,
    ProductPrice,
    BasePrice,

    HasStock,
    IsDisabled,
    HasTemplate,
    BestPrice,
    TemplateId,
    NoShippingEnabled,
    ActualNoShippingEnabled,
    OnSitePickupEnabled,
    SedexEnabled,
    PACEnabled,
    PrivateFleetEnabled,

    [CategoryId],
    [CategoryName],
    [CategorySlug],

    [SellerId],
    [SellerNickname],
    [SellerProfileName],
    [SellerFriendlyUrl],
    [SellerCreatedAt],

    ImageUrls,
    (CASE WHEN @shippingPerStore = 1
        THEN SingleFreightRanges 
        ELSE PrivateFleetRanges END) [DeliveryRanges],	
    VariantTypeIds,
    VariantTypeNames,
    VariantIds,
    VariantNames,
    VariantSlugs,
    VariantsCount,

    AddressId,
    AddressStreet,
    AddressNumber,
    AddressAdditionalInfo,
    AddressNeighborhood,
    AddressCityAndState,
    AddressZipCode,
    AddressLatLng,

    HasActiveSubscription,
    HasPaymentGatewayAccount,
    HasBeenApprovedInModeration,
    AverageRating,
    
    operation
FROM cteProductsWithAggregatedData