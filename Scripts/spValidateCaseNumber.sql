ALTER PROCEDURE spValidateCaseNumber
	@CaseNumber VARCHAR(20)
AS
BEGIN
	BEGIN TRY
		SELECT *
			FROM 
		ProductionMillen
			WHERE
		SerialNo = @CaseNumber
			AND
		StateCode = 1;
	END TRY	
	BEGIN CATCH
		;THROW
	END CATCH
END
