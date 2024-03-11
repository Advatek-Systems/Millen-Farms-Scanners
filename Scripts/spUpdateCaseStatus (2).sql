CREATE PROCEDURE spUpdateCaseStatus
	@PalletID BIGINT,
	@CaseNumber VARCHAR(20)
AS
BEGIN
	BEGIN TRY
		UPDATE
			ProductionMillen
		SET
			FullSkidNo = @PalletID,
			StateCode = 2
		WHERE
		SerialNo = @CaseNumber;
	END TRY	
	BEGIN CATCH
		;THROW
	END CATCH
END
