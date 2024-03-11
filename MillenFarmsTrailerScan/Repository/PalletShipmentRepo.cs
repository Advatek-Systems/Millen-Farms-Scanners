using DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Model;
using System.Data;
using Types;

namespace Repository
{
    public class PalletShipmentRepo
    {
        private DataAccess db;

        public PalletShipmentRepo()
        {
            db = new DataAccess();
        }

        /// <summary>
        /// Creates new trailer shipment
        /// </summary>
        /// <param name="shipment">Shipment info of the shipment being created</param>
        /// <returns></returns>
        public bool CreateShipment(Shipment shipment)
        {
            List<ParmStruct> parms = new List<ParmStruct>();
            parms.Add(new ParmStruct("@TrailerNo", SqlDbType.VarChar, 20, shipment.TrailerNo, ParameterDirection.Input));
            parms.Add(new ParmStruct("@StartedAt", SqlDbType.DateTime, 0, shipment.StartedAt, ParameterDirection.Input));
            parms.Add(new ParmStruct("@StateCode", SqlDbType.Int, 0, shipment.statecode, ParameterDirection.Input));
            parms.Add(new ParmStruct("@ShipmentID", SqlDbType.Int, 0, shipment.ShipmentID, ParameterDirection.Output));

            if(db.SendData("spCreateShipment", parms) > 0)
            {
                shipment.ShipmentID = (int)parms.Where(x => x.Name == "@ShipmentID").FirstOrDefault().Value;
                return true;
            }

            return false;
        }

        public int GetStateCode(int i)
        {
            return (int)db.GetValue($"SELECT StateCode FROM Shipment WHERE ShipmentID = {i}", null, CommandType.Text);
        }

        /// <summary>
        /// Creates the pallet that is being scanned into the shipment
        /// </summary>
        /// <param name="pallet">pallet info of the pallet being scanned</param>
        /// <returns></returns>
        public bool CreatePallet(Pallet pallet)
        {
            List<ParmStruct> parms = new List<ParmStruct>();
            parms.Add(new ParmStruct("@PalletNo", SqlDbType.VarChar, 20, pallet.PalletNo, ParameterDirection.Input));
            parms.Add(new ParmStruct("@ShipmentID", SqlDbType.Int, 0, pallet.ShipmentID, ParameterDirection.Input));
            parms.Add(new ParmStruct("@ScannedAt", SqlDbType.DateTime, 0, pallet.ScannedAt, ParameterDirection.Input));

            return db.SendData("spCreatePallet", parms) > 0;
        }

        /// <summary>
        /// Marks the shipment as complete and makes record in the appropriate tables for future use
        /// </summary>
        /// <param name="shipmentID">id of the shipment to mark as complete</param>
        /// <returns></returns>
        public bool CompleteShipment(int shipmentID)
        {
            List<ParmStruct> parms = new List<ParmStruct>();
            parms.Add(new ParmStruct("@ShipmentID", SqlDbType.VarChar, 20, shipmentID, ParameterDirection.Input));
            parms.Add(new ParmStruct("@CompletedAt", SqlDbType.DateTime, 0, DateTime.Now, ParameterDirection.Input));

            List<Pallet> shipmentPallets = GetAllPallets(shipmentID);

            string trailerNo = (string)db.GetValue($"SELECT TrailerNo FROM Shipment WHERE ShipmentID = {shipmentID}", null, CommandType.Text);

            foreach(Pallet pallet in shipmentPallets)
            {
                db.SendData($"UPDATE ReceivingScaleMillen SET StateCode = {GetStateCode(shipmentID)} WHERE PalletNo = '{pallet.PalletNo}'", null, CommandType.Text);
                db.SendData($"INSERT INTO PrintItems VALUES ('Y', '{shipmentID}', '{pallet.PalletNo}', '{trailerNo}', '{DateTime.Now}', NULL)", null, CommandType.Text);
            }

            db.SendData($"INSERT INTO PrintTable2 VALUES ('Y', '{shipmentID}', 3, '{DateTime.Now}',0,0)", null, CommandType.Text);
            return db.SendData("spCompleteShipment", parms) != 0;
        }

        /// <summary>
        /// Gets all the shipments that are currently being scanned
        /// </summary>
        /// <returns></returns>
        public List<Shipment> GetAllShipments()
        {
            List<Shipment> shipments = new List<Shipment>();
            DataTable dt = db.GetData("spGetAllShipments");

            foreach(DataRow row in dt.Rows)
            {
                shipments.Add(
                        PopulateShipmentObj(row)
                    );
            }

            return shipments;
        }

        /// <summary>
        /// Gets all the associated pallets for the shipment that is in progress
        /// </summary>
        /// <param name="shipmentID">shipment id of the shipment to get the pallets for</param>
        /// <returns></returns>
        public List<Pallet> GetAllPallets(int shipmentID)
        {
            List<Pallet> pallets = new List<Pallet>();

            List<ParmStruct> parms = new List<ParmStruct>();
            parms.Add(new ParmStruct("@ShipmentID", SqlDbType.Int, 0, shipmentID, ParameterDirection.Input));

            DataTable dt = db.GetData("spGetAllPallets", parms);

            foreach(DataRow row in dt.Rows)
            {
                pallets.Add(
                        PopulatePalletObj(row)
                    );
            }

            return pallets;
        }

        /// <summary>
        /// deletes a shipment incase of user input error
        /// </summary>
        /// <param name="shipmentID">ID of the shipment to delete</param>
        /// <returns></returns>
        public bool DeleteShipment(int shipmentID)
        {
            if (GetShipmentPalletCount(shipmentID) == 0)
            {
                List<ParmStruct> parms = new List<ParmStruct>();
                parms.Add(new ParmStruct("@ShipmentID", SqlDbType.Int, 0, shipmentID, ParameterDirection.Input));
                return db.SendData("spDeleteShipment", parms) != 0;
            }
            else
            {
                return false;
            }

        }

        /// <summary>
        /// deletes a pallet from a shipment in case of user input error
        /// </summary>
        /// <param name="palletID">ID of the pallet to delete</param>
        public void DeletePallet(int palletID)
        {
            List<ParmStruct> parms = new List<ParmStruct>();
            parms.Add(new ParmStruct("@PalletID", SqlDbType.Int, 0, palletID, ParameterDirection.Input));
            db.SendData("spDeletePallet", parms);
        }

        /// <summary>
        /// populates the pallet obj
        /// </summary>
        /// <param name="row"></param>
        /// <returns></returns>
        private static Pallet PopulatePalletObj(DataRow row)
        {
            return new Pallet
            {
                PalletID = Convert.ToInt32(row["PalletID"]),
                PalletNo = row["PalletNo"].ToString(),
                ShipmentID = Convert.ToInt32(row["ShipmentID"]),
                ScannedAt = Convert.ToDateTime(row["ScannedAt"]),
                ReadyToPrint = row["ReadyToPrint"].ToString()
            };
        }

        /// <summary>
        /// Check to see if shipment has pallets scanned into it before deleting
        /// </summary>
        /// <param name="shipmentID">ID of the shipment to check</param>
        /// <returns></returns>
        private int GetShipmentPalletCount(int shipmentID)
        {
            List<ParmStruct> parms = new List<ParmStruct>();
            parms.Add(new ParmStruct("@ShipmentID", SqlDbType.Int, 0, shipmentID, ParameterDirection.Input));

            return (int)db.GetValue("spGetShipmentPalletCount", parms);
        }

        /// <summary>
        /// populates the shipment obj
        /// </summary>
        /// <param name="row"></param>
        /// <returns></returns>
        private static Shipment PopulateShipmentObj(DataRow row)
        {
            return new Shipment
            {
                ShipmentID = Convert.ToInt32(row["ShipmentID"]),
                TrailerNo = row["TrailerNo"].ToString(),
                Completed = Convert.ToBoolean(row["Completed"]),
                StartedAt = Convert.ToDateTime(row["StartedAt"])
            };
        }
    }
}
