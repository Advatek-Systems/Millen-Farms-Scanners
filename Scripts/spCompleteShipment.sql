USE TerraBeata;
GO
CREATE PROCEDURE spCompleteShipment
	@ShipmentID INT,
	@CompletedAt DATETIME
AS
BEGIN
	BEGIN TRY
		UPDATE Shipment
			SET
			Completed = 1,
			CompletedAt = @CompletedAt
		WHERE ShipmentID = @ShipmentID;
	END TRY	
	BEGIN CATCH
		;THROW
	END CATCH
END
GO