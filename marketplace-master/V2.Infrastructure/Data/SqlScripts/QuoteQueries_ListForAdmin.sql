--DECLARE @startIndex INT SET @startIndex = 0
--DECLARE @pageSize INT SET @pageSize = 100

DROP TABLE IF EXISTS #tmpQuotes 

;WITH cteQuoteAnswersMetadata AS (
	SELECT
		Quote.Id,
		COUNT(QuoteAnswer.Id) AS AnswersCount,
		MAX(QuoteAnswer.CreatedAt) AS LatestInteraction
	FROM Quote
	INNER JOIN QuoteAnswer ON Quote.Id = QuoteAnswer.Quote_Id
	GROUP BY Quote.Id)

SELECT
	Quote.Id,
	Quote.Title,
	Category.Name CategoryName,
	SubCategory.Name SubCategoryName,
	PortfolioItem.Title PortfolioItemTitle,
	Quote.Quantity,
	Quote.CreatedAt,
	Quote.IsClosed,
	Quote.ClosedAt,
	Quote.DealResult,
	ISNULL(cteQuoteAnswersMetadata.AnswersCount, 0) AnswersCount,
	cteQuoteAnswersMetadata.LatestInteraction,
	Buyer.Id BuyerId,
	Buyer.FullName BuyerName,
	Buyer.Email BuyerEmail,
	Seller.Id SellerId,
	ISNULL(Seller.AlternativeName, Seller.FullName) SellerName,
	Seller.Email SellerEmail,
	Address.CityAndState
INTO #tmpQuotes
FROM Quote
LEFT JOIN cteQuoteAnswersMetadata ON Quote.Id = cteQuoteAnswersMetadata.Id
INNER JOIN [Address] ON Quote.Address_Id = [Address].Id
LEFT JOIN Category ON Quote.Category_Id = Category.Id
LEFT JOIN SubCategory ON Quote.SubCategory_Id = SubCategory.Id
LEFT JOIN PortfolioItem ON Quote.PortfolioItem_Id = PortfolioItem.Id
INNER JOIN Person Buyer ON Quote.Buyer_Id = Buyer.Id
INNER JOIN Person Seller ON Quote.Seller_Id = Seller.Id

SELECT COUNT(1) TotalCount FROM #tmpQuotes

SELECT * FROM #tmpQuotes
ORDER BY CreatedAt DESC
OFFSET @startIndex ROWS
FETCH NEXT @pageSize ROWS ONLY
