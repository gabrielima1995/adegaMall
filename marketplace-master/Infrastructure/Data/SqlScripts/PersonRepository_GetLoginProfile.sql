--DECLARE @userId VARCHAR(128) SELECT @userId = 'b2f7c54c-68bc-43f9-8e73-00e970cbfee7'
--DECLARE @personId VARCHAR(128) SELECT @personId = '53502825-814f-4310-8b5d-fd22f52f7742'

DECLARE @subscriptionStatus INT SET @subscriptionStatus = 0
DECLARE @hasCategories BIT SET @hasCategories = 0
DECLARE @hasPortfolio BIT SET @hasPortfolio = 0

DECLARE @personType VARCHAR(128) SELECT @personType = Discriminator FROM Person WHERE Id = @personId
DECLARE @currentPersonType VARCHAR(30) SET @currentPersonType = 'Unset'

IF EXISTS(SELECT 1 FROM AspNetUserRoles JOIN AspNetRoles ON AspNetUserRoles.RoleId = AspNetRoles.Id AND AspNetRoles.Name = 'Admin' AND @personType = 'AdminPerson' WHERE UserId = @userId)
	SET @currentPersonType = 'Admin'

IF EXISTS(SELECT 1 FROM AspNetUserRoles JOIN AspNetRoles ON AspNetUserRoles.RoleId = AspNetRoles.Id AND AspNetRoles.Name = 'GestaoINA' AND @personType = 'AdminPerson' WHERE UserId = @userId)
	SET @currentPersonType = 'MasterAdmin'

IF EXISTS(SELECT 1 FROM AspNetUserRoles JOIN AspNetRoles ON AspNetUserRoles.RoleId = AspNetRoles.Id AND AspNetRoles.Name = 'Buyer' AND @personType = 'Buyer' WHERE UserId = @userId)
	SET @currentPersonType = 'Buyer'

IF EXISTS(SELECT 1 FROM AspNetUserRoles JOIN AspNetRoles ON AspNetUserRoles.RoleId = AspNetRoles.Id AND AspNetRoles.Name = 'Seller' AND @personType = 'Seller' WHERE UserId = @userId)
	SET @currentPersonType = 'Seller'

IF EXISTS(SELECT 1 FROM AspNetUserRoles JOIN AspNetRoles ON AspNetUserRoles.RoleId = AspNetRoles.Id AND AspNetRoles.Name = 'SellerAssistant' AND @personType = 'SellerAssistant' WHERE UserId = @userId)
	SET @currentPersonType = 'SellerAssistant'


DECLARE @assistantUserId VARCHAR(128)
DECLARE @assistantPersonId VARCHAR(128)

IF (@currentPersonType = 'SellerAssistant') BEGIN
	SET @assistantUserId = @userId
	SET @assistantPersonId = @personId

	SELECT @personId = AssistantToSellerId FROM Person WHERE Id = @assistantPersonId
	SELECT @userId = UserId FROM Person WHERE Id = @personId
END


SELECT @subscriptionStatus = ISNULL([Status], 0)
FROM Subscription
WHERE SellerId = @personId


IF EXISTS(
	SELECT 1
	FROM SubCategoryPortfolio
		INNER JOIN SubCategory ON SubCategoryPortfolio.SubCategory_Id = SubCategory.Id AND SubCategory.Deleted = 0
		INNER JOIN Category ON SubCategory.Category_Id = Category.Id AND Category.Deleted = 0
	WHERE SubCategoryPortfolio.Portfolio_SellerId = @personId
	UNION
	SELECT 1
	FROM PortfolioItemSubcategories
		INNER JOIN PortfolioItem ON PortfolioItemSubcategories.PortfolioItem_Id = PortfolioItem.Id AND PortfolioItem.Portfolio_SellerId = @personId
		INNER JOIN SubCategory ON PortfolioItemSubcategories.SubCategory_Id = SubCategory.Id AND SubCategory.Deleted = 0
		INNER JOIN Category ON SubCategory.Category_Id = Category.Id AND Category.Deleted = 0)
	SET @hasCategories = 1


IF EXISTS(
	SELECT 1
	FROM PortfolioItem
	WHERE PortfolioItem.Portfolio_SellerId = @personId)
	SET @hasPortfolio = 1


SELECT
	@userId 'UserId',
	@personId 'PersonId',
	@currentPersonType 'CurrentPersonType',
	ISNULL(Person.Email, Users.Email) 'Email',
	Person.Avatar,
	COALESCE(Person.AlternativeName, Person.FullName, Users.UserName) 'Name',
	Person.PhoneNumber,
	Person.WhatsAppNumber,
	Person.FriendlyUrl AS 'FriendlyUrlSlug',
	Users.LoginMode,
	Person.Enabled AS 'IsEnabled',
	Person.RequireModeration,
	Person.PreferredTimeZoneId, -- Deve passar para AspNetUsers
	Person.CustomerGatewayId,
	@hasCategories as 'HasCategories',
    @hasPortfolio AS 'HasPortfolio',
	Person.SupplierGatewayId,
	Person.SellerCompleteProfile AS 'SellerCompleteProfileJson',
	@subscriptionStatus AS 'SubscriptionStatus',
	@assistantUserId 'AssistantUserId',
	@assistantPersonId 'AssistantPersonId'
FROM AspNetUsers AS Users
LEFT JOIN Person ON Users.Id = Person.UserId AND Person.Id = @personId
WHERE Users.Id = @userId


SELECT ClaimType
FROM AspNetUserClaims
JOIN AspNetUsers AssistantUser ON AspNetUserClaims.UserId = AssistantUser.Id
JOIN Person AssistantPerson ON AssistantUser.Id = AssistantPerson.UserId
WHERE
	@currentPersonType = 'SellerAssistant' AND
	AspNetUserClaims.UserId = @assistantUserId AND
	AspNetUserClaims.ClaimValue = @personId
