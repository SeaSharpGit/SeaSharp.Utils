using System;
using System.Collections.Generic;
using System.Data;
using System.Transactions;
using MySql.Data.MySqlClient;

namespace SeaSharp.Utils.DB
{
    public static class MySqlUtils
    {
        #region Public Methods
        /// <summary>
        /// 执行SQL语句，返回影响的记录数
        /// </summary>
        /// <param name="sql">SQL语句</param>
        /// <returns>影响的记录数</returns>
        public static int Execute(string db, string sql, params MySqlParameter[] parameters)
        {
            ParametersToDBNull(parameters);
            using (var connection = GetConnection(db))
            {
                using (var command = new MySqlCommand(sql, connection))
                {
                    if (parameters != null && parameters.Length > 0)
                    {
                        command.Parameters.AddRange(parameters);
                    }
                    connection.Open();
                    return command.ExecuteNonQuery();
                }
            }
        }

        /// <summary>
        /// 执行一条计算查询结果语句，返回查询结果（object）。
        /// </summary>
        /// <param name="sql">计算查询结果语句</param>
        /// <returns>查询结果（object）</returns>
        public static object Scalar(string db, string sql, params MySqlParameter[] parameters)
        {
            ParametersToDBNull(parameters);
            using (var connection = GetConnection(db))
            {
                using (var command = new MySqlCommand(sql, connection))
                {
                    if (parameters != null && parameters.Length > 0)
                    {
                        command.Parameters.AddRange(parameters);
                    }
                    connection.Open();
                    return command.ExecuteScalar();
                }
            }
        }

        public static DataTable GetDataTable(string db, string sql, params MySqlParameter[] parameters)
        {
            ParametersToDBNull(parameters);
            var result = new DataTable();
            using (var connection = GetConnection(db))
            {
                using (var command = new MySqlCommand(sql, connection))
                {
                    if (parameters != null && parameters.Length > 0)
                    {
                        command.Parameters.AddRange(parameters);
                    }
                    var data = new MySqlDataAdapter(command);
                    connection.Open();
                    data.Fill(result);
                }
            }
            return result;
        }

        public static DataSet GetDataSet(string db, string sql, params MySqlParameter[] parameters)
        {
            ParametersToDBNull(parameters);
            var result = new DataSet();
            using (var connection = GetConnection(db))
            {
                using (var command = new MySqlCommand(sql, connection))
                {
                    if (parameters != null && parameters.Length > 0)
                    {
                        command.Parameters.AddRange(parameters);
                    }
                    var data = new MySqlDataAdapter(command);
                    connection.Open();
                    data.Fill(result);
                }
            }
            return result;
        }

        public static List<T> GetList<T>(string db, string sql, params MySqlParameter[] parameters)
        {
            var dt = GetDataTable(db, sql, parameters);
            if (dt == null || dt.Rows.Count == 0)
            {
                return new List<T>();
            }
            var type = typeof(T);
            if (type.IsValueType)
            {
                return DataTableToList(dt, a => (T)a[0]);
            }
            else
            {
                if (type.Equals(typeof(string)))
                {
                    return DataTableToList(dt, a => (T)(object)a[0].ToString());
                }
                else if (type.Equals(typeof(DateTime)))
                {
                    return DataTableToList(dt, a => (T)(object)Convert.ToDateTime(a[0]));
                }
                else
                {
                    return DataTableToList(dt,
                        a =>
                        {
                            var t = Activator.CreateInstance<T>();
                            var propertys = type.GetProperties();
                            foreach (var pi in propertys)
                            {
                                var piName = pi.Name;
                                if (dt.Columns.Contains(piName))
                                {
                                    if (!pi.CanWrite)
                                    {
                                        continue;
                                    }
                                    var value = a[piName];
                                    if (value != DBNull.Value)
                                    {
                                        pi.SetValue(t, value, null);
                                    }
                                }
                            }
                            return t;
                        });
                }
            }
        }

        public static object Function(string db, string procedure, params MySqlParameter[] parameters)
        {
            ParametersToDBNull(parameters);
            using (var connection = GetConnection(db))
            {
                using (var command = new MySqlCommand(procedure, connection) { CommandType = CommandType.StoredProcedure })
                {
                    if (parameters != null && parameters.Length > 0)
                    {
                        command.Parameters.AddRange(parameters);
                    }
                    command.Parameters.Add(new MySqlParameter("Result", "") { Direction = ParameterDirection.ReturnValue });
                    connection.Open();
                    command.ExecuteNonQuery();
                    return command.Parameters["Result"].Value;
                }
            }
        }

        public static int Procedure(string db, string procedure, params MySqlParameter[] parameters)
        {
            ParametersToDBNull(parameters);
            using (var connection = GetConnection(db))
            {
                using (var command = new MySqlCommand(procedure, connection) { CommandType = CommandType.StoredProcedure })
                {
                    if (parameters != null && parameters.Length > 0)
                    {
                        command.Parameters.AddRange(parameters);
                    }
                    connection.Open();
                    return command.ExecuteNonQuery();
                }
            }
        }

        public static DataTable ProcedureDataTable(string db, string procedure, params MySqlParameter[] parameters)
        {
            ParametersToDBNull(parameters);
            var result = new DataTable();
            using (var connection = GetConnection(db))
            {
                using (var command = new MySqlCommand(procedure, connection) { CommandType = CommandType.StoredProcedure })
                {
                    if (parameters != null && parameters.Length > 0)
                    {
                        command.Parameters.AddRange(parameters);
                    }
                    var data = new MySqlDataAdapter(command);
                    connection.Open();
                    data.Fill(result);
                }
            }
            return result;
        }

        public static DataSet ProcedureDataSet(string db, string procedure, params MySqlParameter[] parameters)
        {
            ParametersToDBNull(parameters);
            var result = new DataSet();
            using (var connection = GetConnection(db))
            {
                using (var command = new MySqlCommand(procedure, connection) { CommandType = CommandType.StoredProcedure })
                {
                    if (parameters != null && parameters.Length > 0)
                    {
                        command.Parameters.AddRange(parameters);
                    }
                    var data = new MySqlDataAdapter(command);
                    connection.Open();
                    data.Fill(result);
                }
            }
            return result;
        }

        public static void Transaction(Action action)
        {
            using (var ts = new TransactionScope())
            {
                action?.Invoke();
                ts.Complete();
            }
        }
        #endregion

        #region Private Methods
        private static MySqlConnection GetConnection(string connectionString)
        {
            return new MySqlConnection(connectionString);
        }

        private static void ParametersToDBNull(IEnumerable<MySqlParameter> parameters)
        {
            if (parameters == null)
            {
                return;
            }
            foreach (var item in parameters)
            {
                if (item.Value == null && (item.Direction == ParameterDirection.Input || item.Direction == ParameterDirection.InputOutput))
                {
                    item.Value = DBNull.Value;
                }
            }
        }

        private static List<T> DataTableToList<T>(DataTable dt, Func<DataRow, T> convert)
        {
            var result = new List<T>();
            var type = typeof(T);
            foreach (DataRow row in dt.Rows)
            {
                result.Add(convert(row));
            }
            return result;
        }
        #endregion

    }
}
