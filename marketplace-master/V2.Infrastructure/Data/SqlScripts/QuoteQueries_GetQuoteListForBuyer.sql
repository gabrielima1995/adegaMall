-- DECLARE @buyerId VARCHAR(128) SET @buyerId = '04d116d5-9a0e-408a-b0a0-8ea92bed6b64'

/* Quote groups
 * Selects one quote ID from each group
 * The quote latest interacted with is selected
 */

DROP TABLE IF EXISTS #quoteGroups
CREATE TABLE #quoteGroups (Id UNIQUEIDENTIFIER, GroupId UNIQUEIDENTIFIER)

;WITH
	cteQuoteGroupsWithDate AS (
		SELECT Quote.Id, Quote.GroupId, ISNULL(QuoteAnswer.CreatedAt, Quote.CreatedAt) LatestInteractionDate
		FROM Quote
		LEFT JOIN QuoteAnswer ON Quote.LatestAnswer_Id = QuoteAnswer.Id
		WHERE
			Quote.GroupId IS NOT NULL AND
			Quote.Buyer_Id = @buyerId),

	cteQuoteGroups AS (
		SELECT Id, GroupId, ROW_NUMBER() OVER(PARTITION BY GroupId ORDER BY LatestInteractionDate DESC) AS RowNumber
		FROM cteQuoteGroupsWithDate)

INSERT INTO #quoteGroups
SELECT Id, GroupId
FROM cteQuoteGroups
WHERE RowNumber = 1

/* Groupped + individual quotes
 * Gather core data
 */

SELECT *
FROM Quote
LEFT JOIN Category ON Quote.Category_id = Category.Id
LEFT JOIN SubCategory ON Quote.SubCategory_Id = SubCategory.Id
LEFT JOIN PortfolioItem ON Quote.PortfolioItem_Id = PortfolioItem.Id
INNER JOIN [Address] Addr ON Quote.Address_Id = Addr.Id
INNER JOIN Person Buyer ON Quote.Buyer_Id = Buyer.Id
LEFT JOIN Person Seller ON Quote.Seller_Id = Seller.Id
WHERE
	Quote.Id IN (SELECT Id FROM #quoteGroups) OR
	(Quote.Buyer_Id = @buyerId AND (Quote.GroupId IS NULL OR Quote.GroupId NOT IN (SELECT GroupId FROM #quoteGroups)))
ORDER BY Quote.CreatedAt DESC

/* Checks if groups are entirely closed or still have open Quotes
 */

;WITH cteClosedGroups AS (
	SELECT GroupId, SUM(CASE WHEN IsClosed = 1 THEN 0 ELSE 1 END) AllClosed
	FROM Quote
	WHERE GroupId IS NOT NULL AND Buyer_Id = @buyerId
	GROUP BY GroupId)
	
SELECT GroupId, CONVERT(BIT, (CASE WHEN AllClosed >= 1 THEN 0 ELSE 1 END)) 'AllClosed'
FROM cteClosedGroups

/* Gather additional data for groupped quotes
 * Top sellers, sellers count, latest interaction in the group, etc
 */

;WITH cteSellers AS (
	SELECT
		Seller.Id,
		ISNULL(Seller.AlternativeName, Seller.FullName) [Name],
		Seller.Nickname,
		Seller.Avatar AvatarUrl,
		Quote.GroupId,
		CONVERT(BIT, CASE WHEN QuoteAnswer.Id IS NULL THEN 0 ELSE 1 END) HasInteraction,
		ROW_NUMBER() OVER (PARTITION BY Seller.Id, Quote.GroupId ORDER BY QuoteAnswer.CreatedAt) RowNumber
	FROM Person Seller
	INNER JOIN Quote ON Seller.Id = Quote.Seller_Id
	LEFT JOIN QuoteAnswer ON Quote.Id = QuoteAnswer.Quote_Id AND QuoteAnswer.AuthorId = Seller.Id
	WHERE Quote.GroupId IN (SELECT GroupId FROM #quoteGroups))

SELECT Id, [Name], Nickname, AvatarUrl, GroupId, HasInteraction
FROM cteSellers
WHERE RowNumber = 1

DROP TABLE IF EXISTS #latest
CREATE TABLE #latest (
	UserId VARCHAR(128),
	[Name] VARCHAR(500),
	AvatarUrl VARCHAR(1000),
	IsSeller BIT,
	SellerNickname VARCHAR(8000),
	[Text] VARCHAR(4000),
	OccurredAt DATETIME,
	Price DECIMAL(18, 2),
	DeliveryDeadline DATETIME,
	QuoteId UNIQUEIDENTIFIER,
	GroupId UNIQUEIDENTIFIER,
	RowNumber INT)

;WITH cteLatestForGroup AS (
	SELECT
		Author.Id UserId,
		ISNULL(Author.AlternativeName, Author.FullName) [Name],
		Author.Avatar AvatarUrl,
		CONVERT(BIT, CASE WHEN QuoteAnswer.AuthorId = QuoteAnswer.Seller_Id THEN 1 ELSE 0 END) IsSeller,
        Author.Nickname SellerNickname,
		QuoteAnswer.[Text],
		QuoteAnswer.CreatedAt OccurredAt,
		QuoteAnswer.Price,
		QuoteAnswer.DeliveryDeadline,
		Quote.Id QuoteId,
		Quote.GroupId,
		ROW_NUMBER() OVER(PARTITION BY Quote.GroupId ORDER BY QuoteAnswer.CreatedAt DESC) RowNumber
	FROM QuoteAnswer
	INNER JOIN Quote ON QuoteAnswer.Quote_Id = Quote.Id
	INNER JOIN Person Author ON QuoteAnswer.AuthorId = Author.Id
	WHERE Quote.GroupId IN (SELECT GroupId FROM #quoteGroups))

INSERT INTO #latest
SELECT *
FROM cteLatestForGroup
WHERE RowNumber = 1

;WITH cteLatest AS (
	SELECT
		Author.Id UserId,
		ISNULL(Author.AlternativeName, Author.FullName) [Name],
		Author.Avatar AvatarUrl,
		CONVERT(BIT, CASE WHEN QuoteAnswer.AuthorId = QuoteAnswer.Seller_Id THEN 1 ELSE 0 END) IsSeller,
        Author.Nickname SellerNickname,
		QuoteAnswer.[Text],
		QuoteAnswer.CreatedAt OccurredAt,
		QuoteAnswer.Price,
		QuoteAnswer.DeliveryDeadline,
		Quote.Id QuoteId,
		NULL GroupId,
		ROW_NUMBER() OVER(PARTITION BY Quote.Id ORDER BY QuoteAnswer.CreatedAt DESC) RowNumber
	FROM QuoteAnswer
	INNER JOIN Quote ON QuoteAnswer.Quote_Id = Quote.Id
	INNER JOIN Person Author ON QuoteAnswer.AuthorId = Author.Id
	WHERE
		Quote.Buyer_Id = @buyerId AND GroupId IS NULL)

INSERT INTO #latest
SELECT *
FROM cteLatest
WHERE RowNumber = 1

SELECT * FROM #latest

SELECT
	QuoteBilling.Id,
	QuoteBilling.QuoteId,
	QuoteBilling.Amount,
	QuoteBilling.Deadline,
	QuoteBilling.SellerId,
	QuoteBilling.BuyerId,
	QuoteBilling.ProductId,
	QuoteBilling.OrderId,
	QuoteBilling.IsCancelled,
	CONVERT(BIT, CASE
		WHEN QuoteBilling.IsPaid = 1 THEN 1
		WHEN OrderTransaction.BoletoUrl IS NOT NULL THEN 1
		ELSE 0 END) IsPaid,
	QuoteBilling.IsRefunded,
	QuoteBilling.CreatedAt,
	Quote.GroupId
FROM QuoteBilling
INNER JOIN Quote ON QuoteBilling.BuyerId = @buyerId AND QuoteBilling.QuoteId = Quote.Id
LEFT JOIN [Order] ON QuoteBilling.OrderId = [Order].Id
LEFT JOIN OrderTransaction ON [Order].Transaction_Id = OrderTransaction.Id

-- Clean-up
DROP TABLE IF EXISTS #quoteGroups
DROP TABLE IF EXISTS #latest
