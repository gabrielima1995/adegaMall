SELECT COUNT(1)
FROM UserRating
WHERE @targetUserId IS NULL OR RatingTargetId = @targetUserId
    AND UserRating.Rating IS NOT NULL

SELECT
    UserRating.*,
	RatingSender.FullName SenderName, RatingSender.Discriminator SenderRole,
	RatingTarget.FullName TargetName, RatingTarget.Discriminator TargetRole
FROM dbo.UserRating
	INNER JOIN Person RatingSender ON UserRating.RatingSenderId = RatingSender.Id
	INNER JOIN Person RatingTarget on UserRating.RatingTargetId = RatingTarget.Id
WHERE @targetUserId IS NULL OR RatingTarget.Id = @targetUserId
    AND UserRating.Rating IS NOT NULL
ORDER BY CreatedAt DESC
OFFSET @startIndex ROWS
FETCH NEXT @pageSize ROWS ONLY
