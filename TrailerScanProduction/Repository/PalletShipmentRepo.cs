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
        /// Creates a new shipment for pallets to be scanned into
        /// </summary>
        /// <param name="shipment">shipment info for the shipment thats about to be created</param>
        /// <returns></returns>
        public bool CreateShipment(Shipment shipment)
        {
            List<ParmStruct> parms = new List<ParmStruct>
            {
                new ParmStruct("@TrailerNo", SqlDbType.VarChar, 20, shipment.TrailerNo, ParameterDirection.Input),
                new ParmStruct("@StartedAt", SqlDbType.DateTime, 0, shipment.StartedAt, ParameterDirection.Input),
                new ParmStruct("@CustomerID", SqlDbType.Int, 0, shipment.customerID, ParameterDirection.Input),
                new ParmStruct("@ShipmentID", SqlDbType.Int, 0, shipment.ShipmentID, ParameterDirection.Output)
            };

            if (db.SendData("spCreateShipment2", parms) > 0)
            {
                shipment.ShipmentID = (int)parms.Where(x => x.Name == "@ShipmentID").FirstOrDefault().Value;
                return true;
            }

            return false;
        }

        public int GetCustomerID(int id)
        {
            return (int)db.GetValue($"SELECT CustomerID FROM Shipment2 WHERE ShipmentID = {id}", null, CommandType.Text);
        }

        public List<Customer> GetAllCustomers()
        {
            List<Customer> customers = new List<Customer>();

            DataTable dt = db.GetData("SELECT * FROM Customer", null, CommandType.Text);

            foreach(DataRow row in dt.Rows)
            {
                customers.Add(
                        new Customer
                        {
                            CustomerID= Convert.ToInt32(row["CustomerID"]),
                            Name = row["Name"].ToString(),
                            Address = row["Address"].ToString()
                        }
                    );
            }

            return customers;
        }

        /// <summary>
        /// Creates a pallet record for the pallet is being scanned
        /// </summary>
        /// <param name="pallet">the pallet info of the pallet that was just scanned</param>
        /// <returns></returns>
        public bool CreatePallet(Pallet pallet)
        {
            List<ParmStruct> parms = new List<ParmStruct>
            {
                new ParmStruct("@PalletNo", SqlDbType.VarChar, 20, pallet.PalletNo, ParameterDirection.Input),
                new ParmStruct("@ShipmentID", SqlDbType.Int, 0, pallet.ShipmentID, ParameterDirection.Input),
                new ParmStruct("@ScannedAt", SqlDbType.DateTime, 0, pallet.ScannedAt, ParameterDirection.Input),
                new ParmStruct("@IsPalletized", SqlDbType.VarChar, 1, pallet.IsPalletized, ParameterDirection.Input)
            };

            return db.SendData("spCreatePallet2", parms) > 0;
        }

        /// <summary>
        /// marks a shipment as complete and inserts record into the appropriate tables for futute use
        /// </summary>
        /// <param name="shipmentID">ID of the shipment to mark as complete</param>
        /// <returns></returns>
        public bool CompleteShipment(int shipmentID)
        {
            List<ParmStruct> parms = new List<ParmStruct>
            {
                new ParmStruct("@ShipmentID", SqlDbType.VarChar, 20, shipmentID, ParameterDirection.Input),
                new ParmStruct("@CompletedAt", SqlDbType.DateTime, 0, DateTime.Now, ParameterDirection.Input)
            };

            List<Pallet> shipmentPallets = GetAllPallets(shipmentID);

            string trailerNo = (string)db.GetValue($"SELECT TrailerNo FROM Shipment2 WHERE ShipmentID = {shipmentID}", null, CommandType.Text);

            foreach(Pallet pallet in shipmentPallets)
            {
                db.SendData($"UPDATE ProductionMillen SET StateCode = 3, CustomerID = {GetCustomerID(shipmentID)} WHERE FullSkidNo = '{pallet.PalletNo}'", null, CommandType.Text);
                db.SendData($"DELETE FROM Inventory WHERE FullSkidNo = '{pallet.PalletNo}'", null, CommandType.Text);
                db.SendData($"UPDATE ProductionMillen SET StateCode = 3, CustomerID = {GetCustomerID(shipmentID)} WHERE SerialNo = '{pallet.PalletNo}'", null, CommandType.Text);
                db.SendData($"INSERT INTO PrintItems VALUES ('Y', '{shipmentID}', '{pallet.PalletNo}', '{trailerNo}', '{DateTime.Now}', '{pallet.IsPalletized}')", null, CommandType.Text);
            }

            db.SendData($"INSERT INTO PrintTable2(ReadyToPrint, TransNo, PrintType, insDateTime, Flag, PrinterOffline) VALUES ('Y', '{shipmentID}', 5, '{DateTime.Now}', 0, 0)", null, CommandType.Text);
            return db.SendData("spCompleteShipment2", parms) != 0;
        }

        /// <summary>
        /// gets all the shipments that are currently in progress
        /// </summary>
        /// <returns></returns>
        public List<Shipment> GetAllShipments()
        {
            List<Shipment> shipments = new List<Shipment>();
            DataTable dt = db.GetData("spGetAllShipments2");

            foreach(DataRow row in dt.Rows)
            {
                shipments.Add(
                        PopulateShipmentObj(row)
                    );
            }

            return shipments;
        }

        /// <summary>
        /// Gets all the associated pallets for the shipment that was selected
        /// </summary>
        /// <param name="shipmentID">ID of the shipment to get the pallets for</param>
        /// <returns></returns>
        public List<Pallet> GetAllPallets(int shipmentID)
        {
            List<Pallet> pallets = new List<Pallet>();

            List<ParmStruct> parms = new List<ParmStruct>
            {
                new ParmStruct("@ShipmentID", SqlDbType.Int, 0, shipmentID, ParameterDirection.Input)
            };

            DataTable dt = db.GetData("spGetAllPallets2", parms);

            foreach(DataRow row in dt.Rows)
            {
                pallets.Add(
                        PopulatePalletObj(row)
                    );
            }

            return pallets;
        }

        /// <summary>
        /// deletes a shipment in case of user input error
        /// </summary>
        /// <param name="shipmentID">ID of the shipment to delete</param>
        /// <returns></returns>
        public bool DeleteShipment(int shipmentID)
        {
            if (GetShipmentPalletCount(shipmentID) == 0)
            {
                List<ParmStruct> parms = new List<ParmStruct>
                {
                    new ParmStruct("@ShipmentID", SqlDbType.Int, 0, shipmentID, ParameterDirection.Input)
                };
                return db.SendData("spDeleteShipment2", parms) != 0;
            }
            else
            {
                return false;
            }

        }

        /// <summary>
        /// Deletes a pallet from a shipment in case of user input error
        /// </summary>
        /// <param name="palletID">ID of the pallet to delete</param>
        public void DeletePallet(int palletID)
        {
            List<ParmStruct> parms = new List<ParmStruct>
            {
                new ParmStruct("@PalletID", SqlDbType.Int, 0, palletID, ParameterDirection.Input)
            };
            db.SendData("spDeletePallet2", parms);
        }

        /// <summary>
        /// makes the item scanned into the current trailer a pallet instead of a case
        /// </summary>
        /// <param name="palletID"></param>
        public void MakePallet(int palletID)
        {
            List<ParmStruct> parms = new List<ParmStruct>
            {
                new ParmStruct("@PalletID", SqlDbType.Int, 0, palletID, ParameterDirection.Input)
            };
            db.SendData("spMakePallet", parms);
        }

        /// <summary>
        /// makes the item scanned into the current trailer a case instead of a pallet
        /// </summary>
        /// <param name="palletID"></param>
        public void MakeCase(int palletID)
        {
            List<ParmStruct> parms = new List<ParmStruct>
            {
                new ParmStruct("@PalletID", SqlDbType.Int, 0, palletID, ParameterDirection.Input)
            };
            db.SendData("spMakeCase", parms);
        }

        /// <summary>
        /// checks the database too see if the case is already a part of a pallet or not
        /// </summary>
        /// <param name="pallet">object which holds the data</param>
        /// <returns></returns>
        public int IsCasePalletized(Pallet pallet)
        {
            List<ParmStruct> parms = new List<ParmStruct>
            {
                new ParmStruct("@PalletNo", SqlDbType.VarChar, 20, pallet.PalletNo, ParameterDirection.Input)
            };

            DataTable dt = db.GetData("spIsCasePalletized", parms);

            return dt.Rows.Count;
        }

        public int IsPallet(string palletNo)
        {
            List<ParmStruct> parms = new List<ParmStruct>
            {
                new ParmStruct("@PalletNo", SqlDbType.VarChar, 20, palletNo, ParameterDirection.Input)
            };

            DataTable dt = db.GetData("spIsPallet", parms);

            return dt.Rows.Count;
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
                ReadyToPrint = row["ReadyToPrint"].ToString(),
                IsPalletized = row["IsPalletized"].ToString()
            };
        }

        /// <summary>
        /// check to see if shipment to delete has any pallets already scanned into it
        /// </summary>
        /// <param name="shipmentID"></param>
        /// <returns></returns>
        private int GetShipmentPalletCount(int shipmentID)
        {
            List<ParmStruct> parms = new List<ParmStruct>();
            parms.Add(new ParmStruct("@ShipmentID", SqlDbType.Int, 0, shipmentID, ParameterDirection.Input));

            return (int)db.GetValue("spGetShipmentPalletCount2", parms);
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
