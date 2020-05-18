--DECLARE @groupId nvarchar(200) = NULL

;WITH
	cteAugumentedQuotes AS (
		SELECT
			Quote.Id, Quote.Title, Quote.[Description], Quote.Phone, Quote.[Date], Quote.Preference, ISNULL(Quote.Amount, 0) Amount, Quote.[Status], Quote.Buyer_Id, Quote.Category_Id, Quote.SubCategory_Id, Quote.CreatedAt, Quote.GroupId, Quote.IsClosed, Quote.DealResult, Quote.PortfolioItem_Id, Quote.Quantity,
			ROW_NUMBER() OVER(PARTITION BY Quote.GroupId ORDER BY Quote.CreatedAt ASC) RowNumber,
			Person.FullName, Person.Email,
			Street, Number, AdditionalInfo, Neighborhood, ZipCode, CityAndState,
			Category.[Name] AS CategoryName, SubCategory.[Name] AS SubCategoryName,
			PortfolioItem.[Title] as PortfolioItemTitle
		FROM dbo.Quote
			INNER JOIN dbo.Person ON Quote.Buyer_Id = Person.Id
			INNER JOIN dbo.[Address] ON Quote.Address_Id = dbo.[Address].Id
			LEFT JOIN dbo.Category ON Category_Id = Category.Id
			LEFT JOIN dbo.SubCategory ON SubCategory_Id = SubCategory.Id AND SubCategory.Category_Id = Quote.Category_Id
			LEFT JOIN dbo.PortfolioItem ON PortfolioItem_Id = PortfolioItem.Id
		WHERE
			GroupId IS NOT NULL AND (@groupId IS NULL OR @groupId = GroupId)),

	cteOpenCount AS (
		SELECT DISTINCT
			GroupId,
			SUM(CASE WHEN IsClosed = 0 AND Status = 0 THEN 1 ELSE 0 END) AS OpenCount,
			SUM(CASE DealResult WHEN 0 THEN 1 ELSE 0 END) AS DealsWon
		FROM cteAugumentedQuotes
		GROUP BY GroupId)

SELECT
	*,
	CASE WHEN OpenCount > 0 AND DealsWon < 1 THEN 'add' ELSE 'del' END operation
FROM cteAugumentedQuotes
INNER JOIN cteOpenCount ON cteAugumentedQuotes.GroupId = cteOpenCount.GroupId
WHERE RowNumber = 1
