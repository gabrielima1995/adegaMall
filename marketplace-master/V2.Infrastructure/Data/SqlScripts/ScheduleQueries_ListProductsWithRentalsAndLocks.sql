--DECLARE @cancelled INT SET @cancelled = 5
--DECLARE @rejected INT SET @rejected = 6
--DECLARE @rental INT SET @rental = 1
--DECLARE @sellerId VARCHAR(128) SET @sellerId = 'bdcd5e1f-6970-48d6-a014-94c940915e9a'
--DECLARE @startDate DATETIME SET @startDate = DATEADD(DAY, -30, GETDATE())

select Distinct 	Product.Id,Deleted ,
	Product.Title From Product 
	left join Store on Product.Store_SellerId = Store.SellerId
	where 
	Deleted = 0
	and (IsBillingProduct = 0 or IsBillingProduct is null) and
	Product.Store_SellerId = @sellerId
	ORDER BY Product.Title

-- busca antiga
--	SELECT DISTINCT
--	Product.Id,
--	Product.Title
--FROM Product
--LEFT JOIN RentalLock ON Product.Id = RentalLock.Product_Id AND (RentalLock.StartDate IS NULL OR RentalLock.StartDate >= @startDate OR RentalLock.EndDate >= @startDate)
--LEFT JOIN OrderProduct ON Product.Id = OrderProduct.Product_Id AND (OrderProduct.RentalInfo_StartDate >= @startDate OR OrderProduct.RentalInfo_EndDate >= @startDate)
--LEFT JOIN [Order] ON OrderProduct.Order_Id = [Order].Id AND
--	[Order].BusinessModel = @rental AND 
--	[Order].Status != @cancelled AND
--	[Order].Status != @rejected
--WHERE
--	 Store_SellerId = @sellerId AND
--	(RentalLock.Id IS NOT NULL OR [Order].Id IS NOT NULL) AND
--	(IsBillingProduct = 0 or IsBillingProduct is null) 
--ORDER BY Product.Title

