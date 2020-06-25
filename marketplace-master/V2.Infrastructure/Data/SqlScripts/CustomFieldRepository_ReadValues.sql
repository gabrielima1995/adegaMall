SELECT * FROM CustomFields
WHERE 
	EntityId = @EntityId AND
	EntityType = @EntityType;
