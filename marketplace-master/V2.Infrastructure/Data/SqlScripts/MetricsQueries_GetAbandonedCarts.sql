--DECLARE @startDate DATETIME2 = NULL --DATEADD(YEAR, -1, GETUTCDATE())
--DECLARE @endDate DATETIME2 = NULL --DATEADD(DAY, 1, GETUTCDATE())

SELECT
    Cart.Id,
    FullName,
    Email,
    Person.Id AS UserId,
    SUM(Price * Quantity) AS Value,
    Cart.CreatedAt
FROM dbo.Cart 
	INNER JOIN CartItem ON Cart.Id = CartItem.Cart_Id
	INNER JOIN Product ON CartItem.Product_Id = Product.Id
	INNER JOIN Person ON Cart.Buyer_Id = Person.Id
WHERE
    Completed = 0
    AND DATEADD(HOUR, 24, Cart.CreatedAt) < GETUTCDATE()
    AND (@startDate IS NULL OR Cart.CreatedAt >= @startDate)
    AND (@endDate IS NULL OR Cart.CreatedAt <= @endDate)
GROUP BY Cart.Id, FullName, Email, Person.Id, Cart.CreatedAt
ORDER BY Cart.CreatedAt DESC
