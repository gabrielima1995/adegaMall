--DECLARE @sellerId VARCHAR(128) SET @sellerId = '88ac4cbf-5da7-4745-9b2d-b7b59d86177d'

SELECT *
FROM Person
WHERE Id = @sellerId

SELECT *
FROM [Address]
WHERE Id = (SELECT Address_Id FROM Person WHERE Id = @sellerId)

SELECT *
FROM BankAccount
WHERE Id = (SELECT BankAccount_Id FROM Person WHERE Id = @sellerId)

SELECT *
FROM Quote
LEFT JOIN Category ON Quote.Category_Id = Category.Id
LEFT JOIN SubCategory ON Quote.SubCategory_Id = SubCategory.Id
WHERE Seller_Id = @sellerId
ORDER BY Quote.CreatedAt DESC

SELECT *
FROM Subscription
WHERE SellerId = @sellerId

SELECT *
FROM Store
WHERE SellerId = @sellerId

SELECT *
FROM Product
LEFT JOIN ProductCategory ON Product.Category_Id = ProductCategory.Id
WHERE Product.Store_SellerId = @sellerId
	AND Product.Deleted = 0
ORDER BY Title

SELECT *
FROM [Order]
LEFT JOIN Person AS Buyer ON [Order].Buyer_Id = Buyer.Id
LEFT JOIN [Address] ON [Order].ShippingAddress_Id = [Address].Id
WHERE Seller_Id = @sellerId
ORDER BY [Order].CreatedAt DESC

SELECT *
FROM OrderProduct
WHERE Order_Id IN (SELECT Id FROM [Order] WHERE Seller_Id = @sellerId)
ORDER BY Item