--DECLARE @startIndex INT SET @startIndex = 0
--DECLARE @pageSize INT SET @pageSize = 100

DROP TABLE IF EXISTS #tmpQuotes 

;WITH

	cteResolved AS (
		SELECT DISTINCT GroupId
		FROM Quote
		WHERE GroupId IS NOT NULL
			AND Seller_Id IS NOT NULL
	),

	cteUnresolved AS (
		SELECT DISTINCT Quote.GroupId
		FROM Quote
		LEFT JOIN cteResolved ON Quote.GroupId = cteResolved.GroupId
		WHERE Quote.Seller_Id IS NULL
			AND cteResolved.GroupId IS NULL)

SELECT
	Quote.Id,
	Quote.Title,
	Category.Name CategoryName,
	SubCategory.Name SubCategoryName,
	PortfolioItem.Title PortfolioItemTitle,
	Quote.Quantity,
	Quote.CreatedAt,
	Buyer.Id BuyerId,
	Buyer.FullName BuyerName,
	Buyer.Email BuyerEmail,
	Address.CityAndState,
	CONVERT(BIT, CASE WHEN cteUnresolved.GroupId IS NULL THEN 1 ELSE 0 END) HasBeenSentToAnySellers
INTO #tmpQuotes
FROM Quote
LEFT JOIN cteUnresolved ON Quote.GroupId = cteUnresolved.GroupId
INNER JOIN [Address] ON Quote.Address_Id = [Address].Id
LEFT JOIN Category ON Quote.Category_Id = Category.Id
LEFT JOIN SubCategory ON Quote.SubCategory_Id = SubCategory.Id
LEFT JOIN PortfolioItem ON Quote.PortfolioItem_Id = PortfolioItem.Id
INNER JOIN Person Buyer ON Quote.Buyer_Id = Buyer.Id
WHERE Quote.Seller_Id IS NULL

SELECT COUNT(1) TotalCount FROM #tmpQuotes

SELECT * FROM #tmpQuotes
ORDER BY CreatedAt DESC
OFFSET @startIndex ROWS
FETCH NEXT @pageSize ROWS ONLY
