--DECLARE @quoteEnabled BIT = 1;
--DECLARE @productEnabled BIT = 0;
--DECLARE @buyerDecision BIT = 1;
--DECLARE @startDate DATETIME2 = DATEADD(DAY, -30, GETUTCDATE())
--DECLARE @endDate DATETIME2 = GETUTCDATE()

SET @endDate = DATEADD(DAY, 1, @endDate)

--++++++++++++++++++++++++
--+ QUERIES DE ORÇAMENTO +
--++++++++++++++++++++++++

IF(@quoteEnabled = 1)
BEGIN
	SELECT COUNT(1) AS IndividualQuotes FROM dbo.Quote WHERE GroupId IS NULL 
	AND (@startDate IS NULL OR Quote.CreatedAt >= @startDate)
	AND (@endDate IS NULL OR Quote.CreatedAt < @endDate);

	SELECT COUNT(DISTINCT GroupId) AS GroupQuotes FROM dbo.Quote WHERE GroupId IS NOT NULL
	AND (@startDate IS NULL OR Quote.CreatedAt >= @startDate)
	AND (@endDate IS NULL OR Quote.CreatedAt < @endDate);

	--======================================================================================

	SELECT COUNT(Id) AS Quotes_Won FROM dbo.Quote
	WHERE IsClosed = 1 AND DealResult = 0
	AND (@startDate IS NULL OR Quote.ClosedAt >= @startDate)
	AND (@endDate IS NULL OR Quote.ClosedAt < @endDate)

	--======================================================================================

	SELECT COUNT(Id) AS Quotes_Lost FROM dbo.Quote
	WHERE IsClosed = 1 AND DealResult = 1
	AND (@startDate IS NULL OR Quote.ClosedAt >= @startDate)
	AND (@endDate IS NULL OR Quote.ClosedAt < @endDate)

	--======================================================================================

	IF (@buyerDecision = 1)
	BEGIN
		SELECT AVG([Quote].[AcceptedPrice]) AS AverageWon FROM dbo.Quote
		WHERE IsClosed = 1 AND DealResult = 0
		AND (@startDate IS NULL OR Quote.ClosedAt >= @startDate)
		AND (@endDate IS NULL OR Quote.ClosedAt < @endDate)
	END
	ELSE
	BEGIN
		SELECT AVG([Quote].Amount) AS AverageWon FROM dbo.Quote
		WHERE IsClosed = 1 AND DealResult = 0
		AND (@startDate IS NULL OR Quote.ClosedAt >= @startDate)
		AND (@endDate IS NULL OR Quote.ClosedAt < @endDate)
	END

	--======================================================================================

	IF (@buyerDecision = 1)
	BEGIN
		SELECT Person.Id, Person.FullName, Person.AlternativeName, SUM(Quote.AcceptedPrice) AS QuotesTotal, COUNT(Quote.Id) AS QuotesCount
		FROM Quote
        INNER JOIN Person ON Person.Id = Quote.Seller_Id
		WHERE Quote.DealResult = 0
			  AND (@startDate IS NULL OR Quote.ClosedAt >= @startDate)
			  AND (@endDate IS NULL OR Quote.ClosedAt < @endDate)
			  AND Quote.AcceptedPrice IS NOT NULL
		GROUP BY Person.Id, Person.FullName, Person.AlternativeName
		ORDER BY QuotesTotal DESC, QuotesCount DESC
	END
	ELSE
	BEGIN
		SELECT Person.Id, Person.FullName, Person.AlternativeName, SUM(Quote.Amount) AS QuotesTotal, COUNT(Quote.Id) AS QuotesCount
		FROM Quote
        INNER JOIN Person ON Person.Id = Quote.Seller_Id
		WHERE Quote.DealResult = 0
			  AND (@startDate IS NULL OR Quote.ClosedAt >= @startDate)
			  AND (@endDate IS NULL OR Quote.ClosedAt < @endDate)
		GROUP BY Person.Id, Person.FullName, Person.AlternativeName
		ORDER BY QuotesTotal DESC, QuotesCount DESC
	END

	--======================================================================================

	IF (@buyerDecision = 1)
	BEGIN
		SELECT TOP(3) Temp.Category_Id, Category.[Name], Temp.C_Count, Temp.Total FROM
		(
			SELECT Category_Id, COUNT(Category_Id) AS C_Count, SUM(AcceptedPrice) AS Total
			FROM Quote
			WHERE (@startDate IS NULL OR Quote.ClosedAt >= @startDate)
			  AND (@endDate IS NULL OR Quote.ClosedAt < @endDate)
			  AND DealResult = 0
			GROUP BY Category_Id
		) Temp LEFT JOIN Category ON Category.Id = Temp.Category_Id ORDER BY Temp.C_Count DESC;
	END
	ELSE
	BEGIN
		SELECT TOP(3) Temp.Category_Id, Category.[Name], Temp.C_Count, Temp.Total FROM
		(
			SELECT Category_Id, COUNT(Category_Id) AS C_Count, SUM(Amount) AS Total
			FROM Quote
			WHERE (@startDate IS NULL OR Quote.ClosedAt >= @startDate)
			  AND (@endDate IS NULL OR Quote.ClosedAt < @endDate)
			  AND DealResult = 0
			GROUP BY Category_Id
		) Temp LEFT JOIN Category ON Category.Id = Temp.Category_Id ORDER BY Temp.C_Count DESC;
	END
END

--++++++++++++++++++++++
--+ QUERIES DE PRODUTO +
--++++++++++++++++++++++

IF(@productEnabled = 1)
BEGIN
	DROP TABLE IF EXISTS dbo.SumsTable;

	SELECT [Order].Id, SUM(Quantity * Price) AS Sums INTO dbo.SumsTable FROM dbo.[Order] 
				RIGHT JOIN dbo.OrderProduct ON OrderProduct.Order_Id = [Order].Id
		WHERE [Order].[Status] IN (1,2,3,4,7,12,13,14)
				AND (@startDate IS NULL OR [Order].CreatedAt >= @startDate)
				AND (@endDate IS NULL OR [Order].CreatedAt < @endDate)
		GROUP BY [Order].Id

	SELECT AVG(Sums) AS AverageOrder FROM dbo.SumsTable;

	DROP TABLE IF EXISTS dbo.SumsTable;

	--======================================================================================

	--Contagem de Carts Abandonados

	;WITH cteAbandonedCarts AS (
		SELECT Cart.Id
		FROM Cart
			INNER JOIN CartItem ON CartItem.Cart_Id = Cart.Id
			INNER JOIN Product ON Product.Id = CartItem.Product_Id
			INNER JOIN Person ON Person.Id = Cart.Buyer_Id
		WHERE
			Completed = 0
			AND DATEADD(HOUR, 24, Cart.CreatedAt) < GETUTCDATE()
			AND (@startDate IS NULL OR Cart.CreatedAt >= @startDate)
			AND (@endDate IS NULL OR Cart.CreatedAt <= @endDate)
		GROUP BY Cart.Id, FullName, Email, Person.Id, Cart.CreatedAt)

	SELECT COUNT(1) AbandonedCarts
	FROM cteAbandonedCarts

	--======================================================================================

	SELECT TOP(10) Person.FullName, Person.AlternativeName, Person.Id, 
		COUNT(DISTINCT Order_Id) AS Sales, SUM(Price * Quantity) AS TotalSales
	FROM Person 
		INNER JOIN [Order] ON [Order].Seller_Id = Person.Id 
		INNER JOIN OrderProduct ON OrderProduct.Order_Id = [Order].Id
	WHERE [Order].[Status] IN (1,2,3,4,7,12,13,14)
		AND (@startDate IS NULL OR [Order].CreatedAt >= @startDate)
		AND (@endDate IS NULL OR [Order].CreatedAt < @endDate)
	GROUP BY Person.Id, Person.FullName, Person.AlternativeName
	ORDER BY TotalSales DESC

	--======================================================================================

	SELECT TOP(10) Temp.*, Product.Title, Product.FriendlyUrl FROM
	(
		SELECT Product_Id, SUM(Quantity) AS C_Count, SUM(Price * Quantity) AS Total 
		FROM dbo.OrderProduct INNER JOIN [dbo].[Order] ON [Order].Id = OrderProduct.Order_Id
		WHERE (@startDate IS NULL OR OrderProduct.CreatedAt >= @startDate)
		    AND (@endDate IS NULL OR OrderProduct.CreatedAt < @endDate)
			AND [Order].[Status] IN (1,2,3,4,7,12,13,14)
		GROUP BY Product_Id
	) Temp LEFT JOIN dbo.Product ON Product.Id = Temp.Product_Id ORDER BY Temp.Total DESC;

	--======================================================================================

	SELECT Category, SUM(Quantity) AS C_Count, SUM(Price * Quantity) AS Total
		FROM [dbo].[OrderProduct] LEFT JOIN [dbo].[Order] ON [Order].Id = OrderProduct.Order_Id
		WHERE Category IS NOT NULL
			AND (@startDate IS NULL OR OrderProduct.CreatedAt >= @startDate)
		      AND (@endDate IS NULL OR OrderProduct.CreatedAt < @endDate)
			  AND dbo.[Order].Status IN (1,2,3,4,7,12,13,14)
		GROUP BY Category 
		ORDER BY Total DESC;
END