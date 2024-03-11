using DAL;
using Model;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Types;

namespace Repository
{
    public class ProductionRepo
    {
        private DataAccess db;

        public ProductionRepo()
        {
            db = new DataAccess();
        }

        public List<Pallet> GetAllPallets()
        {
            List<Pallet> pallets = new List<Pallet>();

            DataTable dt = db.GetData("spGetAllLoadingPallets");

            foreach(DataRow row in dt.Rows)
            {
                pallets.Add(
                        new Pallet
                        {
                            PalletID = Convert.ToInt64(row["PalletID"]),
                            CreatedAt = Convert.ToDateTime(row["CreatedAt"])
                        }
                    );
            }

            return pallets;
        }

        public long CreatePallet()
        {
            List<ParmStruct> parms = new List<ParmStruct>();

            parms.Add(new ParmStruct("@DateTime", SqlDbType.DateTime, 0, DateTime.Now, ParameterDirection.Input));
            parms.Add(new ParmStruct("@PalletID", SqlDbType.BigInt, 0, null, ParameterDirection.Output));

            db.SendData("spCreateLoadingPallet", parms);

            return (long)parms.Where(x => x.Name == "@PalletID").FirstOrDefault().Value;
        }

        public List<Case> GetCases(long palletID)
        {
            List<Case> cases = new List<Case>();

            List<ParmStruct> parms = new List<ParmStruct>();

            parms.Add(new ParmStruct("@PalletID", SqlDbType.BigInt, 0, palletID, ParameterDirection.Input));

            DataTable dt = db.GetData("spGetAssociatedCases", parms);

            foreach(DataRow row in dt.Rows)
            {
                cases.Add(
                        new Case
                        {
                            SerialNo = row["SerialNo"].ToString()
                        }
                    );
            }

            return cases;
        }

        public bool ValidateCaseNumber(string caseNumber)
        {
            List<ParmStruct> parms = new List<ParmStruct>();

            parms.Add(new ParmStruct("@CaseNumber", SqlDbType.VarChar, 20, caseNumber, ParameterDirection.Input));

            DataTable dt = db.GetData("spValidateCaseNumber", parms);

            if(dt.Rows.Count > 0)
                return true;
            return false;
        }

        public bool UpdateCaseStatus(long palletID, string caseNumber)
        {
            List<ParmStruct> parms = new List<ParmStruct>();

            parms.Add(new ParmStruct("@PalletID", SqlDbType.BigInt, 0, palletID, ParameterDirection.Input));
            parms.Add(new ParmStruct("@CaseNumber", SqlDbType.VarChar, 20, caseNumber, ParameterDirection.Input));

            int rowsAffected = db.SendData("spUpdateCaseStatus", parms);

            return rowsAffected > 0;
        }

        public bool MarkComplete(long palletID)
        {
            List<ParmStruct> parms = new List<ParmStruct>();

            parms.Add(new ParmStruct("@PalletID", SqlDbType.BigInt, 0, palletID, ParameterDirection.Input));
            parms.Add(new ParmStruct("@DateTime", SqlDbType.DateTime, 0, DateTime.Now, ParameterDirection.Input));

            int rowsAffected = db.SendData("spMarkComplete", parms);

            return rowsAffected > 0;
        }

        public bool Delete(long id)
        {
            if(GetCases(id).Count == 0)
            {
                List<ParmStruct> parms = new List<ParmStruct>();
                parms.Add(new ParmStruct("@PalletID", SqlDbType.BigInt, 0, id, ParameterDirection.Input));
                return db.SendData("spDeleteLoadingPallet", parms) != 0;
            }
            return false;
        }

        public List<Freezer> GetFreezers()
        {
            List<Freezer> info = new List<Freezer>();

            DataTable dt = db.GetData("spGetFreezerList");

            foreach (DataRow row in dt.Rows)
            {
                info.Add(
                        new Freezer
                        {
                            FreezerID = Convert.ToInt32(row["FreezerID"]),
                            FreezerName = row["FreezerName"].ToString()
                        }
                    );
            }

            return info;
        }

        public bool PutInFreezer(long palletID, int freezerID)
        {
            List<ParmStruct> parms = new List<ParmStruct>()
            {
                new ParmStruct("@PalletID", SqlDbType.VarChar, 50, palletID.ToString(), ParameterDirection.Input),
                new ParmStruct("@FreezerID", SqlDbType.Int, 0, freezerID, ParameterDirection.Input),
            };

            return db.SendData("spPutPalletizedPalletIntoFreezer", parms) > 0;
        }
    }
}
