SELECT
	SellerId,
	Seller.FullName AS SellerName,
	Subscription.StartDate,
	[Plan].Name AS PlanName,
	Subscription.Amount,
	Subscription.Status
FROM Subscription
INNER JOIN Person AS Seller ON Subscription.SellerId = Seller.Id
INNER JOIN [Plan] ON Subscription.Plan_Id = [Plan].Id
ORDER BY StartDate DESC
