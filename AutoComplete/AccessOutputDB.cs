using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoComplete.Comm;
using System.Configuration;

namespace AutoComplete
{
    class AccessOutputDB :DataUtil
    {
        #region constructors

        public AccessOutputDB(string newConnectionString) : base(newConnectionString) { }
        public AccessOutputDB() : this(("" + ConfigurationManager.AppSettings["OutputDB"]).Trim()) { }

        #endregion

        #region public DataSet getEquityXOICurrentShareClassInfo()
        public DataSet getEquityXOICurrentShareClassInfo(
          string p_ShareClassId
          )
        {
            //parameter count includes @RETURN_VALUE: 2
            SqlParameter[] parameters = {
                                     new SqlParameter("@p_ShareClassId", SqlDbType.Char, 10)
                                   };

            if (p_ShareClassId == null)
                parameters[0].Value = DBNull.Value;
            else
                parameters[0].Value = p_ShareClassId;

            return RunProcedure("getEquityXOICurrentShareClassInfo", parameters, "table1");
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
    }
}
