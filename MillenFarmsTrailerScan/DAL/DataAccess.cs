using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Types;
using CM = System.Configuration.ConfigurationManager;

namespace DAL
{
    public class DataAccess
    {
        #region Methods

        public DataTable GetData(string cmdText, List<ParmStruct> parms = null, CommandType cmdType = CommandType.StoredProcedure)
        {
            DataTable dt = new DataTable();
            SqlCommand cmd = CreateCommand(cmdText, cmdType, parms);

            using (cmd.Connection)
            {
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da.Fill(dt);
            }
            return dt;
        }

        public int SendData(string cmdText, List<ParmStruct> parms = null, CommandType cmdType = CommandType.StoredProcedure)
        {
            int rowsAffected = 0;
            SqlCommand cmd = CreateCommand(cmdText, cmdType, parms);

            using (cmd.Connection)
            {
                cmd.Connection.Open();
                rowsAffected = cmd.ExecuteNonQuery();

                UnloadParms(parms, cmd);
            }

            return rowsAffected;
        }

        public object GetValue(string cmdText, List<ParmStruct> parms = null, CommandType cmdType = CommandType.StoredProcedure)
        {
            object retVal;
            SqlCommand cmd = CreateCommand(cmdText, cmdType, parms);

            using (cmd.Connection)
            {
                cmd.Connection.Open();
                retVal = cmd.ExecuteScalar();
            }

            return retVal;
        }

        #endregion

        #region Private Methods

        private SqlCommand CreateCommand(string cmdText, CommandType cmdType, List<ParmStruct> parms)
        {
            SqlConnection conn = new SqlConnection(CM.ConnectionStrings["ConnStr"].ConnectionString);

            SqlCommand cmd = new SqlCommand(cmdText, conn);
            cmd.CommandType = cmdType;

            if (parms != null)
            {
                foreach (ParmStruct p in parms)
                {
                    cmd.Parameters.Add(p.Name, p.DataType, p.Size).Value = p.Value;
                    cmd.Parameters[p.Name].Direction = p.Direction;
                }
            }

            return cmd;
        }

        private static void UnloadParms(List<ParmStruct> parms, SqlCommand cmd)
        {
            if (parms != null)
            {
                for (int i = 0; i <= parms.Count - 1; i++)
                {
                    ParmStruct p = parms[i];
                    p.Value = cmd.Parameters[i].Value;
                    parms[i] = p;
                }
            }
        }

        private string SQLCleaner(string sql)
        {
            while (sql.Contains("  "))
                sql = sql.Replace("  ", " ");

            return sql.Replace(Environment.NewLine, "");
        }

        #endregion 
    }
}
