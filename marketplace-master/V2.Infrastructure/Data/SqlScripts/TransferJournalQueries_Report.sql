/*
DECLARE @sellerId VARCHAR(128)
DECLARE @startIndex INT SET @startIndex = 0
DECLARE @limit INT SET @limit = 100
DECLARE @type INT
DECLARE @showOnlyCancelled BIT SET @showOnlyCancelled = 0
DECLARE @showOnlyCompleted BIT SET @showOnlyCompleted = 0
DECLARE @startDate DATETIME2
DECLARE @endDate DATETIME2
*/

SELECT *
FROM TransferJournal
INNER JOIN Person Buyers ON TransferJournal.BuyerId = Buyers.Id
INNER JOIN Person Sellers ON TransferJournal.SellerId = Sellers.Id
WHERE
	(@startDate IS NULL OR DueDate >= @startDate) AND
	(@endDate IS NULL OR DueDate <= @endDate)
ORDER BY DueDate DESC
