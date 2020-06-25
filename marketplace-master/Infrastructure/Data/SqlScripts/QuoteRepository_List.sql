--DECLARE @buyerId VARCHAR(128) SET @buyerId = NULL
--DECLARE @sellerId VARCHAR(128) SET @sellerId = NULL
--DECLARE @status INT SET @status = NULL

/* Select the main information for the quotes and related data.
 */
SELECT *
FROM Quote
LEFT JOIN [Address] Addr ON Quote.Address_Id = Addr.Id
INNER JOIN Person Seller ON Quote.Seller_Id = Seller.Id
INNER JOIN Person Buyer ON Quote.Buyer_Id = Buyer.Id
LEFT JOIN Category ON Quote.Category_Id = Category.Id
LEFT JOIN SubCategory ON Quote.SubCategory_Id = SubCategory.Id
LEFT JOIN PortfolioItem ON Quote.PortfolioItem_Id = PortfolioItem.Id
LEFT JOIN QuoteAnswer ON Quote.LatestAnswer_Id = QuoteAnswer.Id
WHERE
	(@buyerId IS NULL OR Buyer.Id = @buyerId) AND
	(@sellerId IS NULL OR Seller.Id = @sellerId) AND
	(@status IS NULL OR Quote.[Status] = @status)
ORDER BY Quote.CreatedAt DESC

/* Select the attachments related to the quotes above.
 * The question is: why isn't it part of the first query?
 * Dapper could split it when mapping and that would be one less scan operation in the database.
 * Possibly a future refactoring.
 */
SELECT *
FROM QuoteAttachment
WHERE QuoteId IN (
	SELECT Id
	FROM Quote
	WHERE
		(@buyerId IS NULL OR Quote.Buyer_Id = @buyerId) AND
		(@sellerId IS NULL OR Quote.Seller_Id = @sellerId) AND
		(@status IS NULL OR Quote.[Status] = @status))

/* Select quote ids from quotes that have an answer from the seller.
 * This will help the application decide whether the seller should be shown to the buyer
 * depending on configuration settings.
 */
SELECT DISTINCT Quote.Id
FROM Quote
INNER JOIN QuoteAnswer ON Quote.Id = QuoteAnswer.Quote_Id
WHERE
	QuoteAnswer.AuthorId = QuoteAnswer.Seller_Id AND
	(@buyerId IS NULL OR Quote.Buyer_Id = @buyerId) AND
	(@sellerId IS NULL OR Quote.Seller_Id = @sellerId) AND
	(@status IS NULL OR Quote.[Status] = @status)

/* The first query above does not include user data for quote answers.
 * This is needed for showing the name of the author of the answer.
 */
SELECT
	QuoteAnswer.Id,
	ISNULL(Buyer.AlternativeName, Buyer.FullName) BuyerName,
	ISNULL(Seller.AlternativeName, Seller.FullName) SellerName
FROM QuoteAnswer
INNER JOIN Quote ON QuoteAnswer.Id = Quote.LatestAnswer_Id
	AND (@buyerId IS NULL OR Quote.Buyer_Id = @buyerId)
	AND (@sellerId IS NULL OR Quote.Seller_Id = @sellerId)
	AND (@status IS NULL OR Quote.[Status] = @status)
LEFT JOIN Person Buyer ON QuoteAnswer.Buyer_Id = Buyer.Id
LEFT JOIN Person Seller ON QuoteAnswer.Seller_Id = Seller.Id
ORDER BY Quote.CreatedAt DESC
