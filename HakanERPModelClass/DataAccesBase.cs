using DPM_DAL;
using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

namespace NZF_DAL
{

    public class DataAccesBase : IDisposable    
    {

        //Private gblSqlConn As System.Data.SqlClient.SqlConnection
        private System.Data.SqlClient.SqlConnection gblSqlConn;
        private System.Data.SqlClient.SqlDataAdapter gblSqlAdap;

        public DataAccesBase()
        {
            globalDegiskenler.gblConStrDPM = "Persist Security Info=False;User ID=;Password=;Initial Catalog=;Data Source=";
            gblSqlConn = new System.Data.SqlClient.SqlConnection(globalDegiskenler.gblConStrDPM);
            gblSqlAdap = new System.Data.SqlClient.SqlDataAdapter();
            gblSqlAdap.SelectCommand = new System.Data.SqlClient.SqlCommand();
            gblSqlAdap.SelectCommand.Connection = gblSqlConn;
            gblSqlAdap.SelectCommand.CommandTimeout = 300;
        }


        public DataAccesBase(string ConnStr)
        {
            gblSqlConn = new System.Data.SqlClient.SqlConnection(ConnStr);
            gblSqlAdap = new System.Data.SqlClient.SqlDataAdapter();
            gblSqlAdap.SelectCommand = new System.Data.SqlClient.SqlCommand();
            gblSqlAdap.SelectCommand.Connection = gblSqlConn;
            gblSqlAdap.SelectCommand.CommandTimeout = 300;
        }




        public System.Data.DataTable ReturnDataTable(string sSQL)
        {

            try
            {
                System.Data.DataSet DS = new System.Data.DataSet();
                gblSqlAdap.SelectCommand.CommandText = sSQL;
                FillDataSet(DS, sSQL);
                return DS.Tables[0];
            }
            catch (Exception)
            {
                return null;
            }
        }



        public string ReturnString(string sSQL)
        {
            DataSet DS = new DataSet();
            gblSqlAdap.SelectCommand.CommandText = sSQL;
            FillDataSet(DS, sSQL);
            if (DS.Tables.Count > 0 && DS.Tables[0].Rows.Count > 0)
            {
                return DS.Tables[0].Rows[0][0].ToString();
            }
            else
            {
                return "";
            }
        }

        public double ReturnDouble(string sSQL)
        {
            //DataSet DS = new DataSet();
            //gblSqlAdap.SelectCommand.CommandText = sSQL;
            //FillDataSet(DS, sSQL);
            //if (DS.Tables.Count > 0 && DS.Tables[0].Rows.Count > 0 && Information.IsNumeric(DS.Tables[0].Rows[0][0].ToString()))
            //{
            //    return Convert.ToDouble(DS.Tables[0].Rows[0][0].ToString());
            //}
            //else
            //{
            //    return 0;
            //}
            return 0;
        }


        public string ExecuteFunction(string sFunctionName)
        {
           
            SqlCommand cmd = new SqlCommand("Select " + sFunctionName, gblSqlConn);
            gblSqlConn.Open();
            string temp= (string)cmd.ExecuteScalar();
            gblSqlConn.Close();
            return temp;
            
        }


        public void ExecuteSQL(string sSQL)
        {
            try
            {
                if (gblSqlAdap.SelectCommand.Connection.State == ConnectionState.Closed)
                {
                    gblSqlAdap.SelectCommand.Connection.Open();
                }
                gblSqlAdap.SelectCommand.CommandText = sSQL;
                gblSqlAdap.SelectCommand.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message + " sSQL:  " + sSQL);
            }
        }




        public void FillDataSet(System.Data.DataSet DS, string sSQL)
        {
            try
            {
                gblSqlAdap.SelectCommand.CommandText = sSQL;
                gblSqlAdap.Fill(DS);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message + " sSQL: " + sSQL);
            }
        }




        #region IDisposable Support
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }


        private bool disposedValue = false; // To detect redundant calls
        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    gblSqlAdap.SelectCommand.Connection.Close();
                    gblSqlAdap.SelectCommand.Connection.Dispose();
                    gblSqlAdap.SelectCommand.Dispose();
                    gblSqlAdap.Dispose();
                    gblSqlConn.Close();
                    gblSqlConn.Dispose();
                }
                disposedValue = true;
            }
        }


        #endregion


    }




}
