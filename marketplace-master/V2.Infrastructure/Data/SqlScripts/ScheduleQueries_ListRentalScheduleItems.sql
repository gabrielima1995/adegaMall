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
    RL.[Id],
    RL.[Type],
    RL.[StartDate],
    RL.[EndDate],
    RL.[DayOfWeek],
    RL.[StartTime],
    RL.[EndTime],
    RL.[TimeZoneId],
    RL.[Mode],
    P.[Id] AS ProductId,
    P.[Title] AS ProductTitle
FROM [dbo].[RentalLock] RL
LEFT JOIN [dbo].[Product] P ON RL.Product_Id = P.Id
WHERE
    (@productId IS NULL OR RL.Product_Id = @productId) AND
    RL.Seller_Id = @sellerId AND
    RL.Deleted = 0 AND
    (RL.[StartDate] IS NULL OR RL.[EndDate] IS NULL OR
        ((RL.[StartDate] BETWEEN @startDate AND @endDate) OR
            (RL.[EndDate] BETWEEN @startDate AND @endDate) OR
            (RL.[StartDate] <= @startDate AND RL.[EndDate] >= @endDate)))

SELECT 
    O.Id,
    O.[Status],
    B.[FullName] AS BuyerName,
    CASE WHEN OP.[ProductType] = @defaultType THEN OP.[RentalInfo_StartDate] ELSE RI.[StartDate] END AS StartDateTime,
    CASE WHEN OP.[ProductType] = @defaultType THEN OP.[RentalInfo_EndDate] ELSE RI.[EndDate] END AS EndDateTime,
    CASE WHEN OP.[ProductType] = @defaultType THEN OP.[Quantity] ELSE 1 END AS Quantity,
    CASE WHEN OP.[ProductType] = @defaultType THEN OP.RentalInfo_ProductAvailability_Id ELSE RI.ProductAvailability_Id END AS ProductAvailabilityId,
    OP.[Product_Id] AS ProductId,
    OP.[Item] AS ProductTitle,
    OP.[RentalInfo_IsReturned] AS IsReturned
FROM [dbo].[Order] O
INNER JOIN [dbo].[Person] B ON B.Id = O.Buyer_Id
INNER JOIN [dbo].[OrderProduct] OP ON OP.Order_Id = O.Id AND (@productId IS NULL OR OP.Product_Id = @productId)
LEFT  JOIN [dbo].[OrderProductRentalsInfo] RI ON RI.OrderProduct_Id = OP.Id
WHERE 
    --O.[BusinessModel] = @rental AND
    O.[Status] <> @canceled AND
    O.[Status] <> @rejected AND
    O.[Seller_Id] = @sellerId AND
    -- obtem o agendamento do produto quando o produto for do tipo Padrão
    ((OP.[ProductType] = @defaultType AND 
        (OP.[RentalInfo_StartDate] BETWEEN @startUTC AND @endUTC 
            OR OP.[RentalInfo_EndDate] BETWEEN @startUTC AND @endUTC 
            OR (OP.[RentalInfo_StartDate] <= @startUTC AND OP.[RentalInfo_EndDate] >= @endUTC)) AND
        (@productAvailabilityId IS NULL OR OP.RentalInfo_ProductAvailability_Id = @productAvailabilityId)) OR
    -- obtem os agendamentos do produto quando o produto for do tipo Pacote de créditos
     (OP.[ProductType] = @packageType AND 
        (RI.[StartDate] BETWEEN @startUTC AND @endUTC
            OR RI.[EndDate] BETWEEN @startUTC AND @endUTC
            OR (RI.[StartDate] <= @startUTC AND RI.[EndDate] >= @endUTC)) AND
        (@productAvailabilityId IS NULL OR RI.ProductAvailability_Id = @productAvailabilityId)))
