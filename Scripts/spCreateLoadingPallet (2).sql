ALTER PROCEDURE spCreateLoadingPallet
	@DateTime DATETIME,
	@PalletID BIGINT OUTPUT
AS
BEGIN
	BEGIN TRY
		INSERT INTO LoadingPallet(CreatedAt, Completed)
			VALUES
		(@DateTime, 0);

		SET @PalletID = SCOPE_IDENTITY();
	END TRY	
	BEGIN CATCH
		;THROW
	END CATCH
END
