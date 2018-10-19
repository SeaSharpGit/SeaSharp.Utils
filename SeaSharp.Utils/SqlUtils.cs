using System.Data;
using System.Data.SqlClient;

namespace Sea.Utils
{
    public class SqlUtils
    {
        #region PROC(UnFinished)
        public static string PROC(string procName, params SqlParameter[] sps)
        {
            var str = string.Empty;//需替换
            using (var con = new SqlConnection(str))
            {
                using (var cmd = new SqlCommand()
                {
                    Connection = con,
                    CommandText = procName,
                    CommandType = CommandType.StoredProcedure
                })
                {
                    if (sps != null)
                    {
                        cmd.Parameters.AddRange(sps);
                    }
                    cmd.Parameters.Add(new SqlParameter()
                    {
                        ParameterName = "@msg",
                        SqlDbType = SqlDbType.NVarChar,
                        Size = 2000,
                        Direction = ParameterDirection.Output,
                        Value = ""
                    });
                    con.Open();
                    cmd.ExecuteNonQuery();
                    return cmd.Parameters["@msg"].Value.ToString();
                }
            }



            //System.Data.SqlClient.SqlConnection con = new System.Data.SqlClient.SqlConnection(Connections.af.ConnectionString);
            //con.Open();
            //var cmd = new SqlCommand();
            //cmd.Connection = con;
            //cmd.CommandText = "sp_bi_TelImport";
            //cmd.CommandType = CommandType.StoredProcedure;
            //cmd.Parameters.Add("@TelTable", SqlDbType.Structured);
            //cmd.Parameters.Add("@errorMessage", SqlDbType.NVarChar, 100);
            //cmd.Parameters["@TelTable"].Value = dt;//传入参数
            //cmd.Parameters["@errorMessage"].Direction = ParameterDirection.Output;//传出参数
            //cmd.ExecuteNonQuery();
            //var msg = new string[2];
            //msg[0] = cmd.Parameters["@errorMessage"].Value.ToString();
            //msg[1] = cmd.Parameters["@sameID"].Value.ToString();
            //con.Close();
        } 
        #endregion
    }
}
