--DECLARE @sellerId varchar(36) = '420f4d76-e30e-4424-9b53-15e876fb08c0'
--DECLARE @productId varchar(36) = null

SELECT
    Products.Id,
    Products.Title
FROM Product Products 
WHERE
    (@productId IS NULL OR Products.Id = @productId) AND
    Products.Store_SellerId = @sellerId AND
    Products.Deleted = 0 AND
    (Products.IsBillingProduct IS NULL OR Products.IsBillingProduct = 0)
ORDER BY Products.Title