/*
DECLARE @startDate DATETIME2 SET @startDate = DATEADD(DAY, -30, GETDATE())
DECLARE @endDate DATETIME2 SET @endDate = GETDATE()

DECLARE @requireModerationSeller BIT SET @requireModerationSeller = 1
DECLARE @requireModerationBuyer BIT SET @requireModerationBuyer = 1
DECLARE @productEnabled BIT SET @productEnabled = 1
DECLARE @quoteEnabled BIT SET @quoteEnabled = 1
DECLARE @subscriptionEnabled BIT SET @subscriptionEnabled = 1
*/

SET @endDate = DATEADD(DAY, 1, @endDate)

IF (@requireModerationSeller = 1)
BEGIN
	SELECT *
	FROM Person
	WHERE Discriminator = 'Seller'
		AND [Enabled] = 1
		AND RequireModeration = 0
		AND CreatedAt >= @startDate
		AND CreatedAt < @endDate
END
ElSE
BEGIN
    SELECT *
    FROM Person
    WHERE Discriminator = 'Seller'
		AND [Enabled] = 1
		AND CreatedAt >= @startDate
		AND CreatedAt < @endDate
END

IF (@requireModerationBuyer = 1)
BEGIN
    SELECT *
	FROM Person
	WHERE Discriminator = 'Buyer'
		AND [Enabled] = 1
		AND RequireModeration = 0
		AND CreatedAt >= @startDate
		AND CreatedAt < @endDate
END
ELSE
BEGIN
    SELECT *
	FROM Person
	WHERE Discriminator = 'Buyer'
	    AND [Enabled] = 1
	    AND CreatedAt >= @startDate
	    AND CreatedAt < @endDate
END


IF (@productEnabled = 1)
BEGIN
	;WITH
		cteOrdersMetrics AS (
			SELECT
				Orders.Id,
				SUM(OrderProduct.Quantity * OrderProduct.Price) SubTotal,
				Orders.Shipping,
				Orders.InstallmentFees,
				Orders.Discount,
				Orders.StartupFee
			FROM [Order] Orders
			INNER JOIN OrderProduct ON Orders.Id = OrderProduct.Order_Id
			WHERE
				Orders.Id IN (
					SELECT DISTINCT Order_Id
					FROM OrderHistory
					WHERE OrderHistory.[Status] IN (
					1,  /*CompletedPayment*/
					2,  /*AwaitingDelivery*/
					3,	/*DeliveryOnTheWay*/
					4,	/*Delivered*/
					7,	/*AwaitingPickup*/
					12,	/*Returned*/
					14))/*Completed*/
				AND Orders.CreatedAt >= @startDate
				AND Orders.CreatedAt < @endDate
			GROUP BY
				Orders.Id,
				Orders.Shipping,
				Orders.InstallmentFees,
				Orders.Discount,
				Orders.StartupFee),
		cteAggregatedMetrics AS (
			SELECT
				Id,
				(SubTotal + Shipping + InstallmentFees - Discount) Amount,
				StartupFee
			FROM cteOrdersMetrics)
	SELECT
		COUNT(1) [Count],
		SUM(Amount) Amount,
		SUM(StartupFee) StartupFees
	FROM cteAggregatedMetrics
END

IF (@quoteEnabled = 1)
BEGIN
	SELECT *
	FROM Quote NewQuotes
	WHERE CreatedAt >= @startDate
		AND CreatedAt < @endDate

    SELECT *
    FROM Quote ClosedQuotes
    WHERE ClosedAt >= @startDate
        AND ClosedAt < @endDate
END

IF (@subscriptionEnabled = 1)
BEGIN
	SELECT *
	FROM Subscription
	WHERE Subscription.StartDate >= @startDate
		AND Subscription.StartDate < @endDate

	SELECT *
	FROM SubscriptionTransaction
	WHERE Subscription_SellerId IN (SELECT DISTINCT SellerId FROM Subscription WHERE StartDate >= @startDate AND StartDate < @endDate)
END

IF (@creditsPacksEnabled = 1)
BEGIN
	SELECT SUM(Amount - GatewayFee) as Total, COUNT(Id) as [Count] 
	FROM CreditsPackTransaction
	WHERE CreatedAt >= @startDate AND CreatedAt < @endDate;
END
