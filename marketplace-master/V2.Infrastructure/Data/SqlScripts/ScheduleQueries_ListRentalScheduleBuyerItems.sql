--DECLARE @canceled INT SET @canceled = 5
--DECLARE @rejected INT SET @rejected = 6
--DECLARE @rental INT SET @rental = 1
--DECLARE @defaultType INT SET @defaultType = 0
--DECLARE @packageType INT SET @packageType = 1
--DECLARE @sellerId VARCHAR(128) SET @sellerId = 'b1221077-2058-4358-a5ea-9d07ac5a7681'
--DECLARE @startDate DATETIME SET @startDate = '2018-05-01'
--DECLARE @endDate DATETIME SET @endDate = '2018-05-30'
--DECLARE @productId UNIQUEIDENTIFIER SET @productId = '8fe05003-78a7-4ce0-b1bd-c848c1fc5b31'
--DECLARE @startUTC DATETIME SET @startUTC = @startDate
--DECLARE @endUTC DATETIME SET @endUTC = @endDate

SELECT 
    O.Id,
    O.[Status],
    S.[FullName] AS SellerName,
    CASE WHEN OP.[ProductType] = @defaultType THEN OP.[RentalInfo_StartDate] ELSE RI.[StartDate] END AS StartDateTime,
    CASE WHEN OP.[ProductType] = @defaultType THEN OP.[RentalInfo_EndDate] ELSE RI.[EndDate] END AS EndDateTime,
    CASE WHEN OP.[ProductType] = @defaultType THEN OP.[Quantity] ELSE 1 END AS Quantity,
    OP.[Item] AS ProductTitle,
    OP.[RentalInfo_IsReturned] AS IsReturned
FROM [dbo].[Order] O
INNER JOIN [dbo].[Person] S ON S.Id = O.Seller_Id
INNER JOIN [dbo].[OrderProduct] OP ON OP.Order_Id = O.Id AND (@productId IS NULL OR OP.Product_Id = @productId)
LEFT  JOIN [dbo].[OrderProductRentalsInfo] RI ON RI.OrderProduct_Id = OP.Id
WHERE 
    O.[Status] <> @canceled AND
    O.[Status] <> @rejected AND
    O.[Buyer_Id] = @buyerId AND
    ((OP.[ProductType] = @defaultType AND 
        (OP.[RentalInfo_StartDate] BETWEEN @startUTC AND @endUTC 
            OR OP.[RentalInfo_EndDate] BETWEEN @startUTC AND @endUTC 
            OR (OP.[RentalInfo_StartDate] <= @startUTC AND OP.[RentalInfo_EndDate] >= @endUTC))) OR
     (OP.[ProductType] = @packageType AND 
        (RI.[StartDate] BETWEEN @startUTC AND @endUTC
            OR RI.[EndDate] BETWEEN @startUTC AND @endUTC
            OR (RI.[StartDate] <= @startUTC AND RI.[EndDate] >= @endUTC))))
