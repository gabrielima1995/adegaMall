--DECLARE @entityType INT SET @entityType = 2
--DECLARE @lang CHAR(2) SET @lang = 'EN'

SELECT
	Id,
	ISNULL(CASE WHEN @lang = 'EN' THEN Name_EN ELSE [Name] END, [Name]) [Name],
	ISNULL(CASE WHEN @lang = 'EN' THEN Placeholder_EN ELSE Placeholder END, Placeholder) Placeholder,
	[Order],
	DataType,
	InputType,
	IsMandatory
FROM CustomField
WHERE EntityType = @entityType
ORDER BY [Order]

SELECT
	CustomFieldOption.Id,
	CustomFieldOption.CustomField_Id FieldId,
	CustomFieldOption.[Name]
FROM CustomFieldOption
INNER JOIN CustomField ON CustomFieldOption.CustomField_Id = CustomField.Id
WHERE EntityType = @entityType
ORDER BY CustomFieldOption.[Name]
