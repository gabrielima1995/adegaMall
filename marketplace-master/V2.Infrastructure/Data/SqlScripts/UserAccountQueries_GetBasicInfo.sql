SELECT
	Id,
	FullName,
	AlternativeName,
	Email,
	Enabled AS IsEnabled,
	CONVERT(BIT, CASE WHEN Discriminator = 'Buyer' THEN 1 ELSE 0 END) AS IsBuyer,
	CONVERT(BIT, CASE WHEN Discriminator = 'Seller' THEN 1 ELSE 0 END) AS IsSeller,
	Avatar AS AvatarUrl
FROM Person
WHERE Id = @userId
