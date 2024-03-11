CREATE PROCEDURE spUpdateStateCode
	@PalletNumber VARCHAR(20),
	@DateTime DATETIME
AS
BEGIN
	BEGIN TRY
		BEGIN TRANSACTION
			UPDATE ReceivingScaleMillen
				SET
			StateCode = 3
				WHERE
			PalletNo = @PalletNumber;

			DECLARE @LotNumber VARCHAR(20);

			SELECT @LotNumber = LotNo 
				FROM 
			ReceivingScaleMillen 
				WHERE 
			PalletNo = @PalletNumber;

			INSERT INTO LotLogMillen 
				VALUES
			(@LotNumber, @DateTime, 0);
		COMMIT TRANSACTION
	END TRY	
	BEGIN CATCH
		ROLLBACK TRANSACTION
		;THROW
	END CATCH
END
