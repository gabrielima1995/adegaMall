-- DECLARE @quoteId UNIQUEIDENTIFIER SET @quoteId = '4A7A01A1-89C0-4F45-A936-17B1470F0585'

SELECT *
FROM Person
WHERE Id = (SELECT TOP 1 Buyer_Id FROM Quote WHERE Id = @quoteId)

SELECT *
FROM Person
WHERE Id = (SELECT TOP 1 Seller_Id FROM Quote WHERE Id = @quoteId)

SELECT *
FROM [Address]
WHERE Id = (SELECT TOP 1 Address_Id FROM Quote WHERE Id = @quoteId)

SELECT *
FROM Quote
WHERE Quote.Id = @quoteId

SELECT *
FROM Category
WHERE Id = (SELECT TOP 1 Category_Id FROM Quote WHERE Id = @quoteId)

SELECT *
FROM SubCategory
WHERE Id = (SELECT TOP 1 SubCategory_Id FROM Quote WHERE Id = @quoteId)

SELECT *
FROM QuoteAnswer
WHERE QuoteAnswer.Quote_Id = @quoteId
ORDER BY CreatedAt

SELECT *
FROM PortfolioItem
WHERE Id = (SELECT TOP 1 PortfolioItem_Id FROM Quote WHERE Id = @quoteId)

SELECT *
FROM QuoteAttachment
WHERE QuoteId = @quoteId