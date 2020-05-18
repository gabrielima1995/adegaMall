-- DECLARE @cartId UNIQUEIDENTIFIER SET @cartId = '4ca525db-07ad-4f75-a633-a25d9d42b075'

/* A quote billing can have multiple orders in case an order failed advancing from 'Processing' stage, e.g. when payment fails.
 * Row number ensures that the latest OrderId, which is bound to be the successful one, is retrieved for each billing.
 */

;WITH cteBillingOrders AS (
	SELECT
		[Order].Id OrderId,
		QuoteBilling.Id QuoteBillingId,
		ROW_NUMBER() OVER(PARTITION BY QuoteBilling.Id ORDER BY [Order].CreatedAt DESC) RowNumber
	FROM [Order]
	INNER JOIN OrderProduct ON [Order].Id = OrderProduct.Order_Id
	INNER JOIN QuoteBilling ON OrderProduct.Product_Id = QuoteBilling.ProductId
	WHERE [Order].CartId = @cartId)

SELECT OrderId, QuoteBillingId
FROM cteBillingOrders
WHERE RowNumber = 1
