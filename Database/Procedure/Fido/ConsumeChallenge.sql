CREATE OR ALTER PROCEDURE dbo.sp_Fido_ConsumeChallenge
    @UserId INT,
    @Challenge NVARCHAR(500),
    @Now DATETIME2 = NULL
AS
BEGIN
    SET NOCOUNT ON;

    IF @Now IS NULL SET @Now = SYSUTCDATETIME();

    ;WITH c AS (
        SELECT TOP 1 Id
        FROM dbo.AuthChallenges
        WHERE UserId = @UserId
          AND Challenge = @Challenge
          AND UsedAt IS NULL
          AND ExpiresAt > @Now
        ORDER BY CreatedAt DESC
    )
    UPDATE ac
    SET UsedAt = @Now
    FROM dbo.AuthChallenges ac
    INNER JOIN c ON c.Id = ac.Id;

    SELECT CASE WHEN EXISTS (
        SELECT 1
        FROM dbo.AuthChallenges
        WHERE UserId = @UserId
          AND Challenge = @Challenge
          AND UsedAt IS NOT NULL
          AND ExpiresAt > @Now
    ) THEN CAST(1 AS BIT) ELSE CAST(0 AS BIT) END AS Consumed;
END;
GO
