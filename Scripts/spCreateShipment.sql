USE TerraBeata;
GO
CREATE PROCEDURE spCreateShipment
	@TrailerNo VARCHAR(20),
	@StartedAt DATETIME
AS
BEGIN
	BEGIN TRY
		INSERT INTO
			Shipment(TrailerNo, Completed, StartedAt)
			VALUES
			(@TrailerNo, 0, @StartedAt);
	END TRY	
	BEGIN CATCH
		;THROW
	END CATCH
END
GO