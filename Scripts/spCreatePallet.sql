USE TerraBeata;
GO
CREATE PROCEDURE spCreatePallet
	@PalletNo VARCHAR(20),
	@ShipmentID INT,
	@ScannedAt DATETIME
AS
BEGIN
	BEGIN TRY
		INSERT INTO 
			Pallet(PalletNo, ShipmentID, ScannedAt, ReadyToPrint)
			VALUES
			(@PalletNo, @ShipmentID, @ScannedAt, 'Y');
	END TRY	
	BEGIN CATCH
		;THROW
	END CATCH
END
GO