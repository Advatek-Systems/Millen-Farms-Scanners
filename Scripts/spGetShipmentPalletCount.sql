USE TerraBeata;
GO
CREATE PROCEDURE spGetShipmentPalletCount
	@ShipmentID INT
AS
BEGIN
	BEGIN TRY
		SELECT COUNT(*)
			FROM Pallet
			WHERE Pallet.ShipmentID = @ShipmentID;
	END TRY	
	BEGIN CATCH
		;THROW
	END CATCH
END
GO