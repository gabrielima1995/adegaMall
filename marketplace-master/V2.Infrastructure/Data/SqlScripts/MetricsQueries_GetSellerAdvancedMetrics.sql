--DECLARE @startDate DATETIME2 = NULL;
--DECLARE @endDate DATETIME2 = NULL;
--DECLARE @sellerId VARCHAR(200) = '0a1a6e1f-7854-41f1-a137-4b0389a4a0c3';
--DECLARE @buyerDecision BIT = 1;
--DECLARE @comissionFixed DECIMAL(5,2) = 2;
--DECLARE @comissionMultiplier DECIMAL(5,2) = 0.15;

--========================================================

SELECT Seller_Id, COUNT(DISTINCT Id) AS NumQuotes, 
	COUNT(CASE DealResult WHEN 1 THEN 1 ELSE NULL END) AS ClosedQuotes
FROM dbo.Quote 
WHERE Seller_Id = @sellerId
	AND (@startDate IS NULL OR CreatedAt > @startDate)
	AND (@endDate IS NULL OR CreatedAt < @endDate)
GROUP BY Seller_Id;


--========================================================

IF(@buyerDecision = 0)
BEGIN
	SELECT COUNT(Id) AS NumQuotes, SUM(Amount) AS Total FROM dbo.Quote
	WHERE Seller_Id = @sellerId
		AND (@startDate IS NULL OR CreatedAt > @startDate)
		AND (@endDate IS NULL OR CreatedAt < @endDate) 
		AND DealResult = 0
		AND IsClosed = 1
	GROUP BY Seller_Id;
END
ELSE
BEGIN
	SELECT COUNT(Id) AS NumQuotes, 
		   SUM(AcceptedPrice - @fixedFee - (AcceptedPrice * @percentFee)) AS Total 
	FROM dbo.Quote
	WHERE Seller_Id = @sellerId
		AND (@startDate IS NULL OR CreatedAt > @startDate)
		AND (@endDate IS NULL OR CreatedAt < @endDate) 
		AND DealResult = 0
		AND IsClosed = 1
		AND AcceptedPrice IS NOT NULL
	GROUP BY Seller_Id;

	SELECT COUNT(DISTINCT QuoteAnswer.Quote_Id) AS Proposals 
	FROM dbo.QuoteAnswer
	WHERE DeliveryDeadline IS NOT NULL
		  AND Seller_Id = @sellerId
		  AND Price IS NOT NULL
		  AND (@startDate IS NULL OR CreatedAt > @startDate)
		  AND (@endDate IS NULL OR CreatedAt < @endDate);
	
END

--========================================================

SELECT SellerId, SUM(TransferAmount / 100) AS WarrantyAmount FROM dbo.ScheduledTransfer
WHERE TransferAuthorized = 0
	AND	SellerId = @sellerId
	AND (@startDate IS NULL OR CreatedAt > @startDate)
	AND (@endDate IS NULL OR CreatedAt < @endDate)
GROUP BY SellerId;

--========================================================

--PRODUCTS
DROP TABLE IF EXISTS TempTable;

SELECT [Order].Id, ([Order].StartupFee) as fee, ((SUM(Price * Quantity) + [Order].Shipping) - [Order].StartupFee) AS Revenue,((SUM(Price * Quantity)) - [Order].StartupFee) as LiquidRevenue, SUM(Quantity) AS ProductsSold
INTO dbo.TempTable
FROM dbo.[Order] INNER JOIN dbo.OrderProduct ON OrderProduct.Order_Id = [Order].Id LEFT JOIN OrderTransaction transac on transac.id = [Order].Transaction_Id
WHERE [Order].Seller_Id = @sellerId
	  AND  [Order].[Status] IN (1,2,3,4,7,12,14)
	  AND (transac.Id IS NULL OR transac.Status IN ('AUTHORIZED', 'Paid'))
GROUP BY [Order].Id,[Order].StartupFee,[Order].Shipping;  

SELECT COUNT(Id) AS NumSales, SUM(Revenue) AS TotalRevenue,Sum(LiquidRevenue) as TotalLiquidRevenue, SUM(ProductsSold) AS ProductsSold FROM dbo.TempTable;

DROP TABLE IF EXISTS dbo.TempTable;

--Count credits
SELECT ExpirationDate, CurrentCredits FROM LeadSalesProfile 
	INNER JOIN CreditsBundle ON CreditsBundle.LeadSalesProfile_Id = LeadSalesProfile.Id
	WHERE LeadSalesProfile.SellerId = @sellerId AND
	      ExpirationDate > SYSUTCDATETIME() AND
		  CurrentCredits > 0
	ORDER BY ExpirationDate ASC;

--Count unlocked quotes
SELECT Count(UnlockedQuote.UnlockedQuoteId) as UnlockedQuotes FROM UnlockedQuote 
	INNER JOIN LeadSalesProfile ON UnlockedQuote.LeadSalesProfileId = LeadSalesProfile.Id
	WHERE LeadSalesProfile.SellerId = @sellerId
	GROUP BY UnlockedQuote.LeadSalesProfileId;

--Get next credits date
SELECT NextCreditsAddDate FROM Subscription WHERE SellerId = @sellerId AND Status NOT IN (5,6,99);