;WITH cte AS (
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
        P.[Title] AS ProductTitle,
        CAST(RL.[StartDate] AS DATETIME) + CAST(RL.[StartTime] AS DATETIME) StartDateTime,
        CAST(RL.[EndDate] AS DATETIME) + CAST(RL.[EndTime] AS DATETIME) EndDateTime
    FROM [dbo].[RentalLock] RL
    LEFT JOIN [dbo].[Product] P ON RL.Product_Id = P.Id
    WHERE
        ((@onlyByProduct = 0 AND RL.Product_Id IS NULL) OR RL.Product_Id = @productId) AND
        RL.Seller_Id = @sellerId AND
        RL.Deleted = 0)

SELECT *
FROM cte
WHERE
    StartDate IS NULL OR EndDate IS NULL OR
        (Mode = 0 AND (
            (StartDateTime BETWEEN @startDate AND @endDate) OR
            (EndDateTime BETWEEN @startDate AND @endDate) OR
            (StartDateTime <= @startDate AND EndDateTime >= @endDate))) OR
        (Mode = 1 AND (
            (@startDate BETWEEN StartDateTime AND EndDateTime) OR
            (@endDate BETWEEN StartDateTime AND EndDateTime) OR
            (@startDate >= StartDateTime AND @endDate <= EndDateTime)))
