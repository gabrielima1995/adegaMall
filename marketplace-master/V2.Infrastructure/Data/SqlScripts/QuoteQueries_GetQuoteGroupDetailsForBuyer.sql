--DECLARE @groupId UNIQUEIDENTIFIER SET @groupId = 'fbfd9267-58a2-480b-aa09-fe86a0df649a'

SELECT TOP 1 *
FROM Quote
LEFT JOIN Category ON Quote.Category_id = Category.Id
LEFT JOIN SubCategory ON Quote.SubCategory_Id = SubCategory.Id
LEFT JOIN PortfolioItem ON Quote.PortfolioItem_Id = PortfolioItem.Id
INNER JOIN [Address] Addr ON Quote.Address_Id = Addr.Id
INNER JOIN Person Buyer ON Quote.Buyer_Id = Buyer.Id
LEFT JOIN Person Seller ON Quote.Seller_Id = Seller.Id
WHERE Quote.GroupId = @groupId

;WITH cteQuotes AS (
	SELECT Id, Seller_Id, LatestAnswer_Id
	FROM Quote
	WHERE GroupId = @groupId)
SELECT
	Seller.Id SellerId,
	cteQuotes.Id QuoteId,
	ISNULL(Seller.AlternativeName, Seller.FullName) SellerName,
	Seller.Nickname SellerNickname,
	Seller.Avatar SellerAvatarUrl,
	CONVERT(BIT, CASE WHEN Answer.Id IS NULL THEN 0 ELSE 1 END) HasInteraction,
	Answer.[Text] InteractionText,
	Answer.Price InteractionPrice,
    Answer.DeliveryDeadline InteractionDeadline,
	Answer.CreatedAt InteractionDate,
	(SELECT TOP 1 AttachmentUrl FROM QuoteAttachment WHERE QuoteAnswerId = Answer.Id) AttachmentUrl,
	CONVERT(BIT, CASE WHEN Answer.AuthorId = Answer.Seller_Id THEN 1 ELSE 0 END) IsInteractionBySeller
FROM Person Seller
INNER JOIN cteQuotes ON Seller.Id = cteQuotes.Seller_Id
LEFT JOIN QuoteAnswer Answer ON cteQuotes.LatestAnswer_Id = Answer.Id
ORDER BY Answer.CreatedAt DESC

;WITH cteLatestProposals AS (
	SELECT
		QuoteAnswer.Seller_Id SellerId,
		QuoteAnswer.Price,
		QuoteAnswer.DeliveryDeadline,
		ROW_NUMBER() OVER (PARTITION BY QuoteAnswer.Seller_Id ORDER BY QuoteAnswer.CreatedAt) RowNumber
	FROM QuoteAnswer
	INNER JOIN Quote ON QuoteAnswer.Quote_Id = Quote.Id AND Quote.GroupId = @groupId
	WHERE
		Price IS NOT NULL AND DeliveryDeadline IS NOT NULL)
SELECT *
FROM cteLatestProposals

SELECT CONVERT(BIT, CASE WHEN COUNT(1) > 0 THEN 1 ELSE 0 END) HasOpenQuotes
FROM Quote
WHERE GroupId = @groupId AND IsClosed = 0