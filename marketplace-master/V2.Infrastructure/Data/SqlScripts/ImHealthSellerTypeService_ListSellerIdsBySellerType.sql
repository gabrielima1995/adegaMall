SELECT DISTINCT CONVERT(VARCHAR(36), FieldValue.EntityId)
FROM CustomField Field
INNER JOIN CustomFieldValue FieldValue ON Field.Id = FieldValue.CustomField_Id
WHERE FieldValue.[Value] = 'true' AND Field.[Name] = @sellerType