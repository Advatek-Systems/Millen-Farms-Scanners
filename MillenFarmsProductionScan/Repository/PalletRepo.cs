using DAL;
using Model;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Types;

namespace Repository
{
    public class PalletRepo
    {
        private DataAccess db;

        public PalletRepo()
        {
            db = new DataAccess();
        }

        public bool ValidatePalletNumber(string palletNumber)
        {
            DataTable dt = db.GetData($"SELECT * FROM ReceivingScaleMillen WHERE PalletNo = '{palletNumber}'", null, CommandType.Text);

            if (dt.Rows.Count == 0)
                return false;

            return true;
        }

        public string UpdatePalletStatus(string palletNumber)
        {
            List<ParmStruct> parms = new List<ParmStruct>()
            {
                new ParmStruct("@PalletNumber", SqlDbType.VarChar, 20, palletNumber, ParameterDirection.Input),
                new ParmStruct("@DateTime", SqlDbType.DateTime, 0, DateTime.Now, ParameterDirection.Input),
                new ParmStruct("@LotNumber", SqlDbType.VarChar, 20, "", ParameterDirection.InputOutput)
            };

            db.SendData("spUpdateStateCode", parms);

            return parms.Where(x => x.Name == "@LotNumber").FirstOrDefault().Value.ToString();
        }

        public List<Product> GetProductList()
        {
            List<Product> info = new List<Product>();

            DataTable dt = db.GetData("spGetProductList");

            foreach (DataRow row in dt.Rows)
            {
                info.Add(
                        new Product
                        {
                            ProductID = Convert.ToInt32(row["ProductID"]),
                            ProductName = row["ProductName"].ToString()
                        }
                    );
            }

            return info;
        }

        public List<BoxSize> GetBoxSizeList()
        {
            List<BoxSize> info = new List<BoxSize>();

            DataTable dt = db.GetData("spGetBoxSizeList");

            foreach(DataRow row in dt.Rows)
            {
                info.Add(
                        new BoxSize
                        {
                            BoxSizeID = Convert.ToInt32(row["BoxSizeID"]),
                            BoxSizeName = row["BoxSizeName"].ToString()
                        }
                    );
            }

            return info;
        }

        public void InsertProductionRecord(string lotNo, int prodID, int boxSizeID, int qty, string freshOrFrozen)
        {
            int currentCount = Convert.ToInt32(db.GetValue("spGetCount"));

            for (int i = 0; i < qty; i++)
            {
                if (currentCount == 99999)
                    currentCount = 1;
                else
                    currentCount++;

                string serialNo = GetLastTwoDigitsOfYear() + GetJulianDateOfYear() + "01" + currentCount.ToString().PadLeft(5, '0');

                List<ParmStruct> parms = new List<ParmStruct>()
                {
                    new ParmStruct("@LotNumber", SqlDbType.VarChar, 20, lotNo, ParameterDirection.Input),
                    new ParmStruct("@SerialNumber", SqlDbType.VarChar, 20, serialNo, ParameterDirection.Input),
                    new ParmStruct("@ProductID", SqlDbType.Int, 0, prodID, ParameterDirection.Input),
                    new ParmStruct("@BoxSizeID", SqlDbType.Int, 0, boxSizeID, ParameterDirection.Input),
                    new ParmStruct("@FreshOrFrozen", SqlDbType.VarChar, 50, freshOrFrozen, ParameterDirection.Input),
                };

                db.SendData("spInsertProduction", parms);

                Thread.Sleep(100);
            }
        }

        private static string GetLastTwoDigitsOfYear()
        {
            int year = DateTime.Now.Year;
            int lastTwoDigits = year % 100;
            return lastTwoDigits.ToString();
        }

        private static string GetJulianDateOfYear()
        {
            DateTime currentDate = DateTime.Now;
            int julianDate = currentDate.DayOfYear;

            return julianDate.ToString();
        }

        public bool DeleteCase(string serialNo)
        {
            List<ParmStruct> parms = new List<ParmStruct>()
            {
                new ParmStruct("@SerialNo", SqlDbType.VarChar, 50, serialNo, ParameterDirection.Input)
            };

            return db.SendData("spDeleteCase", parms) > 0;
        }
    }
}
