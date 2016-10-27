using System;
using System.Data;
using System.Xml;
using System.Data.SqlClient;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;
using AutoComplete.Comm;

namespace AutoComplete
{
    /// <summary>
    /// Abstract class that act as a basic for other classes. Contains a couple of support functions, as well
    /// as automatic instantiation and configuration of the SqlConnectin object and/or SqlTransaction object.
    /// </summary>
    public abstract class DataUtil
    {

        #region protected SqlConnection Connection

        /// <summary>
        /// Protected SqlConnection, to hold SQL connection.
        /// </summary>
        protected SqlConnection Connection;

        #endregion

        #region private SqlTransaction Transaction

        /// <summary>
        /// private sql transaction object, to hold the transaction when rollback is needed.
        /// </summary>
        private SqlTransaction Transaction;

        #endregion

        #region private string connectionString

        /// <summary>
        /// Private sql connection string, this is completely hide from child classes.
        /// </summary>
        private string connectionString;

        #endregion

        #region protected string ConnectionString (property to access private member connectionString)

        /// <summary>
        /// Protected propery that exposes the connection string to inheriting classes.
        /// </summary>
        protected string ConnectionString
        {
            get { return connectionString; }
            /* 
                  set	{this.connectionString = value; }
                  */
        }

        #endregion

        //constructor

        #region public DataUtil( string newConnectionString )

        /// <summary>
        /// Constructor takes a connection string and instantiates a new connection based on that string. 
        /// We don't open the connection here. The connection is opened whenever necessary.
        /// </summary>
        /// <param name="newConnectionString"></param>
        public DataUtil(string newConnectionString)
        {
            connectionString = newConnectionString;
            Connection = new SqlConnection(connectionString);
            //Tool.Debug("connection timeout: " + Connection.ConnectionTimeout);
        } //end of DataUtil

        #endregion

        //private helper methods

        #region private void LogSPError(string storedProcName, IDataParameter [] parameters,Exception ex)

        private void LogSPError(string storedProcName, IDataParameter[] parameters, Exception ex)
        {
            try
            {
                Tool.LogFile("sql to test: " + BuildSqlStatement(storedProcName, parameters) + ex);
            }
            catch { }

        }

        #endregion private void LogSPError(string storedProcName, IDataParameter [] parameters,Exception ex)

        #region private string BuildSqlStatement( string storedProcName, IDataParameter [] parameters )

        private string BuildSqlStatement(string storedProcName, IDataParameter[] parameters)
        {
            string s = "exec " + storedProcName + " ";
            if (parameters == null)
            {
                Tool.Warn("parameters is null.");
            }
            foreach (IDataParameter dp in parameters)
            {
                //Tool.Fatal("dp value is :" + dp.Value);
                if (dp.Direction == ParameterDirection.Output)
                {
                    s += "null, ";
                }
                else if (dp.Direction == ParameterDirection.Input || dp.Direction == ParameterDirection.InputOutput)
                {
                    if (dp.Value == DBNull.Value)
                    {
                        s += "null, ";
                    }
                    else if (dp.DbType == DbType.Byte
                               || dp.DbType == DbType.Currency
                               || dp.DbType == DbType.Decimal
                               || dp.DbType == DbType.Double
                               || dp.DbType == DbType.Int16
                               || dp.DbType == DbType.Int32
                               || dp.DbType == DbType.Int64
                               || dp.DbType == DbType.SByte
                               || dp.DbType == DbType.Single
                               || dp.DbType == DbType.UInt16
                               || dp.DbType == DbType.UInt32
                               || dp.DbType == DbType.UInt64
                      )
                    {
                        s += dp.Value + ", ";
                    }
                    else if (dp.DbType == DbType.Boolean)
                    {
                        s += ((bool)dp.Value ? "1, " : "0, ");
                    }
                    else
                    {
                        s += "'" + dp.Value + "', ";
                    }
                }
            } //endof foreach( IDataParameter dp in parameters)

            s = s.Trim();
            if (s.EndsWith(","))
            {
                s = s.Substring(0, s.Length - 1);
            }

            return s;
        }

        #endregion private string BuildSqlStatement( string storedProcName, IDataParameter [] parameters )

        #region private SqlCommand BuildQueryCommand( string storedProcName, IDataParameter [] parameters )

        /// <summary>
        /// Create and return a new SQL Command object.
        /// </summary>
        /// <param name="storedProcName"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        private SqlCommand BuildQueryCommand(string storedProcName, IDataParameter[] parameters)
        {
            SqlCommand command = new SqlCommand(storedProcName, Connection);

            command.CommandType = CommandType.StoredProcedure;
            command.CommandTimeout = 3600;

            foreach (SqlParameter parameter in parameters)
            {
                command.Parameters.Add(parameter);
            }

            return command;
        }

        #endregion

        #region private SqlCommand BuildIntCommand( string storedProcName, IDataParameter [] parameters )

        /// <summary>
        /// Create and return a new SQL Command object. This object contains an additional parameter which returns condition code.
        /// </summary>
        /// <param name="storedProcName"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        private SqlCommand BuildIntCommand(string storedProcName, IDataParameter[] parameters)
        {
            SqlCommand command = BuildQueryCommand(storedProcName, parameters);
            command.Parameters.Add
              (new SqlParameter
                 ("ReturnValue", SqlDbType.Int, 4, ParameterDirection.ReturnValue, false, 0, 0, string.Empty, DataRowVersion.Default,
                  null));
            return command;
        }

        #endregion

        //protected methods that can be used in the sub-class

        #region NeedRetry

        private bool NeedRetry(Exception ex)
        {
            List<string> retryCases = new List<string>();
            retryCases.Add("A transport-level error has occurred when sending the request to the server.");
            retryCases.Add("A transport-level error has occurred when receiving results from the server.");
            retryCases.Add("The timeout period elapsed prior to obtaining a connection from the pool.");
            //retryCases.Add("was deadlocked on lock resources with another process and has been chosen as the deadlock");

            bool needTry = false;

            foreach (string msg in retryCases)
            {
                if (ex.Message.ToLower().IndexOf(msg.ToLower()) >= 0)
                {
                    needTry = true;
                    break;
                }
            }
            return needTry;
        }
        #endregion

        #region RunProcedure


        protected string RunProcedureForParameter(string storedProcName, IDataParameter[] parameters, string outputname)
        {
            string result = string.Empty;
            int triesMax = 2;
            int tries = 0;

            while (tries++ < triesMax)
            {
                try
                {
                    result = RunProcedure1(storedProcName, parameters, outputname);
                }
                catch (Exception ex)
                {
                    if (NeedRetry(ex) && tries < triesMax)
                    {
                        Tool.Warn("Retry " + tries);
                        SqlConnection.ClearPool(Connection);
                        System.Threading.Thread.Sleep(100);
                        continue;
                    }
                    else
                    {
                        tries = triesMax;
                        LogSPError(storedProcName, parameters, ex);
                        throw;
                    }
                }
                break;
            }

            return result;
        }

        protected int RunProcedure(string storedProcName, IDataParameter[] parameters, out int rowsAffected)
        {
            rowsAffected = -1;
            int result = -1;
            int triesMax = 2;
            int tries = 0;
            while (tries++ < triesMax)
            {
                try
                {
                    result = RunProcedure0(storedProcName, parameters, out rowsAffected);
                }
                catch (Exception ex)
                {
                    if (NeedRetry(ex) && tries < triesMax)
                    {
                        Tool.Warn("Retry " + tries);
                        SqlConnection.ClearPool(Connection);
                        System.Threading.Thread.Sleep(100);
                        continue;
                    }
                    else
                    {
                        tries = triesMax;
                        LogSPError(storedProcName, parameters, ex);
                        throw;
                    }
                }
                break;
            }
            return result;
        }

        protected DataSet RunProcedure(string storedProcName, IDataParameter[] parameters, string tableName)
        {
            DataSet ds = new DataSet();
            int triesMax = 2;
            int tries = 0;
            while (tries++ < triesMax)
            {
                try
                {
                    ds = RunProcedure0(storedProcName, parameters, tableName);
                }
                catch (Exception ex)
                {
                    if (NeedRetry(ex) && tries < triesMax)
                    {
                        Tool.Warn("Retry " + tries);
                        SqlConnection.ClearPool(Connection);
                        System.Threading.Thread.Sleep(100);
                        continue;
                    }
                    else
                    {
                        tries = triesMax;
                        LogSPError(storedProcName, parameters, ex);
                        throw;
                    }
                }
                break;
            }
            return ds;
        }
        #endregion

        #region protected string RunProcedure( string storedProcName, IDataParameter [] parameters, out int rowsAffected )

        /// <summary>
        /// Run stored procedure with parameters, return the success or failor code, out parameter contains rows affected.
        /// </summary>
        /// <param name="storedProcName"></param>
        /// <param name="parameters"></param>
        /// <param name="rowsAffected">the out parameter "rowsAffected" will<br/> contain the number of rows affected.</param>
        /// <returns>returns a numeric condition code that indicates success or some degree of failure.</returns>
        protected string RunProcedure1(string storedProcName, IDataParameter[] parameters, string outputparametername)
        {
            string result = string.Empty;
            SqlCommand command = null;

            try
            {
                if (Connection.State == ConnectionState.Closed)
                    Connection.Open();

                command = BuildQueryCommand(storedProcName, parameters);
                command.ExecuteNonQuery();
                if (!(command.Parameters[outputparametername].Value == DBNull.Value))
                    result = (string)command.Parameters[outputparametername].Value;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                // Here to clear the parameters, before next calling, to avoid exception like below:
                // The SqlParameter is already contained by another SqlParameterCollection... Diego Cheng, 2009-5-13.
                if (command != null && command.Parameters != null)
                {
                    command.Parameters.Clear();
                }
                if (Connection.State != ConnectionState.Closed)
                    Connection.Close();
            }

            return result;
        }


        #endregion

        #region protected int RunProcedure( string storedProcName, IDataParameter [] parameters, out int rowsAffected )

        /// <summary>
        /// Run stored procedure with parameters, return the success or failor code, out parameter contains rows affected.
        /// </summary>
        /// <param name="storedProcName"></param>
        /// <param name="parameters"></param>
        /// <param name="rowsAffected">the out parameter "rowsAffected" will<br/> contain the number of rows affected.</param>
        /// <returns>returns a numeric condition code that indicates success or some degree of failure.</returns>
        protected int RunProcedure0(string storedProcName, IDataParameter[] parameters, out int rowsAffected)
        {
            int result = -1;
            rowsAffected = -1;

            SqlCommand command = new SqlCommand();
            try
            {
                if (Connection.State == ConnectionState.Closed)
                    Connection.Open();

                command = BuildIntCommand(storedProcName, parameters);
                rowsAffected = command.ExecuteNonQuery();
                result = (int)command.Parameters["ReturnValue"].Value;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                // Here to clear the parameters, before next calling, to avoid exception like below:
                // The SqlParameter is already contained by another SqlParameterCollection... Diego Cheng, 2009-5-13.
                if (command.Parameters != null)
                {
                    command.Parameters.Clear();
                }
                if (Connection.State != ConnectionState.Closed)
                    Connection.Close();
            }
            return result;
        }

        #endregion

        #region protected DataSet RunProcedure( string storedProcName, IDataParameter [] parameters, string tableName )

        /// <summary>
        /// Return a DataSet which contain a table that holds records that are returned by running a stored procedure with parameters
        /// </summary>
        /// <param name="storedProcName"></param>
        /// <param name="parameters"></param>
        /// <param name="tableName"></param>
        /// <returns></returns>
        protected DataSet RunProcedure0(string storedProcName, IDataParameter[] parameters, string tableName)
        {
            DataSet ds = new DataSet();

            using (SqlDataAdapter sda = new SqlDataAdapter())
            {
                try
                {
                    if (Connection.State == ConnectionState.Closed)
                        Connection.Open();
                    sda.SelectCommand = BuildQueryCommand(storedProcName, parameters);

                    //There are some timeout in IA production, so increase the value here.
                    //Timeout for connection has been set to 0 (no limits) in configuration file
                    //Xin Ling, 08/26/2004
                    //sda.SelectCommand.CommandTimeout = 0;		// no limits, for testing

                    sda.Fill(ds, tableName);

                    //if no table at all, add a empty table
                    if (ds.Tables.Count == 0)
                    {
                        ds.Tables.Add(new DataTable(tableName));
                    }
                }
                catch (Exception ex)
                {
                    throw ex;
                }
                finally
                {
                    // Here to clear the parameters, before next calling, to avoid exception like below:
                    // The SqlParameter is already contained by another SqlParameterCollection... Diego Cheng, 2009-5-13.
                    if (sda.SelectCommand != null && sda.SelectCommand.Parameters != null)
                    {
                        sda.SelectCommand.Parameters.Clear();
                    }
                    if (Connection.State != ConnectionState.Closed)
                        Connection.Close();
                }
            }
            return ds;
        }

        #endregion

        #region protected SqlDataReader RunProcedure( string storedProcName, IDataParameter [] parameters)

        /// <summary>
        /// Return a SqlDataReader by running a stored procedure with parameters. 
        /// </summary>
        /// 
        /// <param name="storedProcName"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        /// <remarks></remarks>
        protected SqlDataReader RunProcedure(string storedProcName, IDataParameter[] parameters)
        {
            SqlDataReader returnReader = null;
            int triesMax = 2;
            int tries = 0;
            while (tries++ < triesMax)
            {
                try
                {
                    if (Connection.State == ConnectionState.Closed)
                        Connection.Open();

                    SqlCommand command = BuildQueryCommand(storedProcName, parameters);

                    returnReader = command.ExecuteReader(CommandBehavior.CloseConnection);
                }
                catch (Exception ex)
                {
                    if (NeedRetry(ex) && tries < triesMax)
                    {
                        Tool.Warn("Retry " + tries);
                        SqlConnection.ClearPool(Connection);
                        System.Threading.Thread.Sleep(100);
                        continue;
                    }
                    else
                    {
                        tries = triesMax;
                        LogSPError(storedProcName, parameters, ex);
                        throw;
                    }
                }
                break;
            }

            return returnReader;
        }

        #endregion

        #region Obsolete, protected void RunProcedure( string storedProcName, IDataParameter [] parameters, DataSet ds, string tableName )
        /*
        /// <summary>
        /// Add records that are returned by running a stored procedure with parameters as a table to the existing DataSet
        /// </summary>
        /// <param name="storedProcName"></param>
        /// <param name="parameters"></param>
        /// <param name="ds"></param>
        /// <param name="tableName"></param>
        protected void RunProcedure(string storedProcName, IDataParameter[] parameters, DataSet ds, string tableName)
        {
            try
            {
                Connection.Open();
                SqlDataAdapter sda = new SqlDataAdapter();
                sda.SelectCommand = BuildQueryCommand(storedProcName, parameters);
                sda.Fill(ds, tableName);
            }
            catch (SqlException ex)
            {
                LogSPError(storedProcName, parameters, ex);
                throw;
            }
            catch (Exception exx)
            {
                LogSPError(storedProcName, parameters, exx);
                throw;
            }
            finally
            {
                Connection.Close();
            }
        } //end of RunProcedure()
        */
        #endregion

        #region protected int RunProcedureWithoutClose( string storedProcName, IDataParameter [] parameters, out int rowsAffected )

        /// <summary>
        /// Run stored procedure with parameters, return the success or failor code, out parameter contains rows affected.
        /// </summary>
        /// <param name="storedProcName"></param>
        /// <param name="parameters"></param>
        /// <param name="rowsAffected">the out parameter "rowsAffected" will<br/> contain the number of rows affected.</param>
        /// <returns>returns a numeric condition code that indicates success or some degree of failure.</returns>
        protected int RunProcedureWithoutClose(string storedProcName, IDataParameter[] parameters, out int rowsAffected)
        {
            int result;

            try
            {
                if (Connection.State == ConnectionState.Closed)
                    Connection.Open();

                SqlCommand command = BuildIntCommand(storedProcName, parameters);
                rowsAffected = command.ExecuteNonQuery();
                result = (int)command.Parameters["ReturnValue"].Value;
            }
            catch (Exception exx)
            {
                if (Connection.State != ConnectionState.Closed)
                    Connection.Close();
                LogSPError(storedProcName, parameters, exx);
                throw;
            }
            return result;
        }

        #endregion

        #region protected XmlDocument RunProcedureXML( string storedProcName, IDataParameter [] parameters)

        protected XmlDocument RunProcedureXML(string storedProcName, IDataParameter[] parameters)
        {
            XmlDocument doc = new XmlDocument();

            try
            {
                Connection.Open();
                SqlCommand command = BuildIntCommand(storedProcName, parameters);
                SqlDataReader reader = command.ExecuteReader();
                StringBuilder sb = new StringBuilder();
                sb.Append("<AUTOROOT>");
                do
                {
                    while (reader.Read())
                    {
                        sb.Append(reader.GetSqlString(0).Value);
                    }
                } while (reader.NextResult());
                sb.Append("</AUTOROOT>");
                doc.LoadXml(sb.ToString());

                reader.Close();
                Connection.Close();
            }
            catch (Exception ex)
            {
                if (Connection.State != ConnectionState.Closed)
                    Connection.Close();
                LogSPError(storedProcName, parameters, ex);
                throw;
            }
            return doc;
        }

        #endregion protected XmlDocument RunProcedureXML( string storedProcName, IDataParameter [] parameters)

        #region public SqlDataReader RunQuery( string queryText )

        /// <summary>
        /// The calling method is responsible for closing the reader.
        /// </summary>
        /// <param name="queryText"></param>
        /// <returns></returns>
        public SqlDataReader RunQuery(string queryText)
        {
            try
            {
                Connection.Open();
                SqlCommand command = new SqlCommand(queryText, Connection);
                command.CommandType = CommandType.Text;
                return command.ExecuteReader(CommandBehavior.CloseConnection);
            }
            catch (Exception ex)
            {
                Connection.Close();

                Tool.LogFile("sql:" + queryText);
                Tool.LogFile(ex.ToString());
                throw;
            }
        }

        #endregion public SqlDataReader RunQuery( string queryText )

        #region Init SqlParameter

        /// <summary>
        /// BigInt
        /// </summary>
        /// <param name="parameterName"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        protected SqlParameter CreateSqlParameter(string parameterName, long value)
        {
            SqlParameter sp = new SqlParameter();
            sp.ParameterName = parameterName;
            sp.SqlDbType = SqlDbType.BigInt;

            if (value == long.MinValue) sp.Value = DBNull.Value;
            else sp.Value = value;

            return sp;
        }

        /// <summary>
        /// Char/VarChar/Text
        /// </summary>
        /// <param name="parameterName"></param>
        /// <param name="type"></param>
        /// <param name="size"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        protected SqlParameter CreateSqlParameter(string parameterName, SqlDbType type, int size, string value)
        {
            SqlParameter sp = new SqlParameter();
            sp.ParameterName = parameterName;
            sp.SqlDbType = type;
            if (size != int.MinValue) sp.Size = size;

            if (string.IsNullOrEmpty(value)) sp.Value = DBNull.Value;
            else sp.Value = value;

            return sp;
        }

        protected SqlParameter CreateSqlParameter(string parameterName, SqlDbType type, int size, string value, string defaultValue)
        {
            SqlParameter sp = new SqlParameter();
            sp.ParameterName = parameterName;
            sp.SqlDbType = type;
            if (size != int.MinValue) sp.Size = size;

            if (string.IsNullOrEmpty(value)) sp.Value = defaultValue;
            else sp.Value = value;

            return sp;
        }

        /// <summary>
        /// SmallDateTime
        /// </summary>
        /// <param name="parameterName"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        protected SqlParameter CreateSqlParameter(string parameterName, DateTime value)
        {
            SqlParameter sp = new SqlParameter();
            sp.ParameterName = parameterName;
            sp.SqlDbType = SqlDbType.SmallDateTime;

            if (value == DateTime.MinValue) sp.Value = DBNull.Value;
            else sp.Value = value;

            return sp;
        }

        protected SqlParameter CreateSqlParameter(string parameterName, SqlDbType type, DateTime value)
        {
            SqlParameter sp = new SqlParameter();
            sp.ParameterName = parameterName;
            sp.SqlDbType = type;

            if (value == DateTime.MinValue) sp.Value = DBNull.Value;
            else sp.Value = value;

            return sp;
        }

        /// <summary>
        /// Decimal
        /// </summary>
        /// <param name="parameterName"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        protected SqlParameter CreateSqlParameter(string parameterName, decimal value)
        {
            SqlParameter sp = new SqlParameter();
            sp.ParameterName = parameterName;
            sp.SqlDbType = SqlDbType.Decimal;

            if (value == decimal.MinValue) sp.Value = DBNull.Value;
            else sp.Value = value;

            return sp;
        }

        /// <summary>
        /// Int
        /// </summary>
        /// <param name="parameterName"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        protected SqlParameter CreateSqlParameter(string parameterName, SqlDbType type, int value)
        {
            SqlParameter sp = new SqlParameter();
            sp.ParameterName = parameterName;
            sp.SqlDbType = type;

            if (value == int.MinValue) sp.Value = DBNull.Value;
            else sp.Value = value;

            return sp;
        }

        /// <summary>
        /// Bool/Bit
        /// </summary>
        /// <param name="parameterName"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        protected SqlParameter CreateSqlParameter(string parameterName, bool? value)
        {
            SqlParameter sp = new SqlParameter();
            sp.ParameterName = parameterName;
            sp.SqlDbType = SqlDbType.Bit;

            if (value == null) sp.Value = DBNull.Value;
            else sp.Value = value;

            return sp;
        }

        #endregion

        //to support transaction/commit/rollback

        #region public void beginTransaction()

        /// <summary>
        /// open connection and create a database Transaction
        /// </summary>
        public void beginTransaction()
        {
            Connection.Open();
            Transaction = Connection.BeginTransaction();
        }

        #endregion

        #region protected int RunProcedureWithinTrans( string storedProcName, IDataParameter [] parameters, out int rowsAffected )

        protected int RunProcedureWithinTrans(string storedProcName, IDataParameter[] parameters, out int rowsAffected)
        {
            int result;

            SqlCommand command = BuildIntCommand(storedProcName, parameters);
            command.Transaction = Transaction;
            rowsAffected = command.ExecuteNonQuery();
            result = (int)command.Parameters["ReturnValue"].Value;

            return result;
        }

        #endregion

        #region public void commitTransaction()

        /// <summary>
        /// commit the database transaction and close the connection
        /// </summary>
        public void commitTransaction()
        {
            Transaction.Commit();
            Connection.Close();
        }

        #endregion

        #region public void rollbackTransaction()

        /// <summary>
        /// rollback the database transaction and close the connection
        /// </summary>
        public void rollbackTransaction()
        {
            Transaction.Rollback();
            Connection.Close();
        }

        #endregion

        #region public string GetConnectionString()

        public string GetConnectionString()
        {
            return connectionString;
        }

        #endregion

        //public helper

        #region public void CloseConnection()
        /// <summary>
        /// close the conncetion
        /// Due to connection pooling, it makes no difference whether using keep-connection-alive solution,
        /// which is an old way before ADO.Net. We can just open-call-close, and leave it to connection pool
        /// to handle. Diego Cheng.
        /// </summary>
        /// <Author>yonghua zeng</Author>
        /// <CreationTime>2008-5-80</CreationTime>
        public void CloseConnection()
        {
            if (Connection.State == ConnectionState.Closed)
                return;

            Connection.Close();
        }

        public void OpenConnection()
        {
            if (Connection.State == ConnectionState.Closed)
                Connection.Open();
        }

        #endregion

        #region public static DataTable GetDataTable( SqlDataReader reader)

        /// <summary>
        /// Converting a DataReader to a DataTable
        /// </summary>
        /// <param name="reader">DataReader that needs to be converted</param>
        /// <returns>result DataTable</returns>
        public static DataTable GetDataTable(SqlDataReader reader)
        {
            DataTable schemaTable = reader.GetSchemaTable();
            DataTable dt = new DataTable();
            DataColumn dc;
            DataRow row;
            ArrayList al = new ArrayList();

            for (int i = 0; i < schemaTable.Rows.Count; i++)
            {
                dc = new DataColumn();

                if (!dt.Columns.Contains(schemaTable.Rows[i]["ColumnName"].ToString()))
                {
                    dc.ColumnName = schemaTable.Rows[i]["ColumnName"].ToString();
                    dc.Unique = Convert.ToBoolean(schemaTable.Rows[i]["IsUnique"]);
                    dc.AllowDBNull = Convert.ToBoolean(schemaTable.Rows[i]["AllowDBNull"]);
                    dc.ReadOnly = Convert.ToBoolean(schemaTable.Rows[i]["IsReadOnly"]);
                    al.Add(dc.ColumnName);
                    dt.Columns.Add(dc);
                }
            }

            while (reader.Read())
            {
                row = dt.NewRow();
                for (int i = 0; i < al.Count; i++)
                {
                    row[((String)al[i])] = reader[(String)al[i]];
                }
                dt.Rows.Add(row);
            }
            reader.Close();
            return dt;
        }

        #endregion

        #region public static DataSet	GetDataSet( SqlDataReader reader)

        public static DataSet GetDataSet(SqlDataReader reader)
        {
            DataSet ds = new DataSet();
            ds.Tables.Add(GetDataTable(reader));
            return ds;
        }

        #endregion

        #region SetColumnMapping2Attribute

        public static void SetColumnMapping2Attribute(DataTable dt)
        {
            foreach (DataColumn column in dt.Columns)
                column.ColumnMapping = MappingType.Attribute;
        }

        public static void SetColumnMapping2Attribute(DataSet ds)
        {
            foreach (DataTable table in ds.Tables)
                SetColumnMapping2Attribute(table);
        }

        #endregion
    }
}