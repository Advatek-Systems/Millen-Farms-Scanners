CREATE PROCEDURE spGetAssociatedCases
	@PalletID BIGINT
AS
BEGIN
	BEGIN TRY
		SELECT *
			FROM 
		ProductionMillen
			WHERE
		FullSkidNo = @PalletID
		ORDER BY InsDateTime DESC;
	END TRY	
	BEGIN CATCH
		;THROW
	END CATCH
END
