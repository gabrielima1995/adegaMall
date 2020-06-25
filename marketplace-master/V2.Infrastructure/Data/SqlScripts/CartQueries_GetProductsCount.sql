SELECT ISNULL(SUM(CartItem.Quantity), 0)
FROM CartItem
INNER JOIN Cart ON CartItem.Cart_Id = Cart.Id
INNER JOIN Product ON CartItem.Product_Id = Product.Id
	AND Product.Deleted = 0 AND Product.CurrentStock > 0
WHERE Cart.Buyer_Id = @buyerId
AND Cart.Completed = 0