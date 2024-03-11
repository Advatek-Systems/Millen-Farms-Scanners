USE [TerraBeata]
GO
/****** Object:  StoredProcedure [dbo].[spDeleteShipment2]    Script Date: 2021-06-16 11:51:46 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE spDeleteLoadingPallet
	@PalletID BIGINT
AS
BEGIN
	BEGIN TRY
		DELETE FROM LoadingPallet WHERE PalletID = @PalletID;
	END TRY	
	BEGIN CATCH
		;THROW
	END CATCH
END
