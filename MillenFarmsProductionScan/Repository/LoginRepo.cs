using DAL;
using Model;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Types;

namespace Repository
{
    public class LoginRepo
    {
        private DataAccess db;

        public LoginRepo()
        {
            db = new DataAccess();
        }

        public Login ValidateUser(string userName, string password)
        {
            Login user = null;

            List<ParmStruct> parms = new List<ParmStruct>();

            parms.Add(new ParmStruct("@Username", SqlDbType.VarChar, 255, userName, ParameterDirection.Input));
            parms.Add(new ParmStruct("@Password", SqlDbType.VarChar, 255, HashPassword(password), ParameterDirection.Input));

            DataTable dt = db.GetData("spGetUserLogin", parms);

            if (dt.Rows.Count > 0)
            {
                DataRow row = dt.Rows[0];
                user = PopulateLogin(row);
            }

            return user;
        }

        private string HashPassword(string password)
        {
            using (SHA256 sha256Hash = SHA256.Create())
            {
                byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(password));

                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < bytes.Length; i++)
                {
                    builder.Append(bytes[i].ToString("x2"));
                }
                return builder.ToString();
            }
        }

        private static Login PopulateLogin(DataRow row)
        {
            return new Login
            {
                UserID = Convert.ToInt32(row["UserID"]),
                Username = row["Username"].ToString(),
                Password = row["Password"].ToString()
            };
        }
    }
}
