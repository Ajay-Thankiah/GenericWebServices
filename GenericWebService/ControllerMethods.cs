using System;
using System.Collections;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Web;

namespace GenericWebService.Helpers
{
    public class ControllerMethods
    {
        private static readonly ConnectionStringSettings connectionStringSettingLocalhost;
        private static readonly ConnectionStringSettings connectionStringSettingDev;
        private static readonly ConnectionStringSettings connectionStringSettingProd;
        private static readonly String connectionString;
        private static readonly SqlConnection sqlConnection;
        private static readonly string[] validIdArray = Convert.ToString(ConfigurationManager.AppSettings["validIds"]).Split(',');
        private static readonly ArrayList validIdArrayList = new ArrayList(validIdArray);
        private static readonly string host;

        static ControllerMethods()
        {
            if(sqlConnection == null)
            {
                host = HttpContext.Current.Request.Url.Host;

                foreach (ConnectionStringSettings connectionStringSetting in ConfigurationManager.ConnectionStrings)
                {
                    if (connectionStringSetting.Name.Contains("Localhost")) connectionStringSettingLocalhost = connectionStringSetting;
                    if (connectionStringSetting.Name.Contains("Dev")) connectionStringSettingDev = connectionStringSetting;
                    if (connectionStringSetting.Name.Contains("Prod")) connectionStringSettingProd = connectionStringSetting;
                }
                
                if (host.Equals("localhost"))
                {
                    connectionString = (connectionStringSettingLocalhost != null) ? connectionStringSettingLocalhost.ConnectionString : "";
                }
                else if (host.Contains("dev"))
                {
                    connectionString = (connectionStringSettingDev != null) ? connectionStringSettingDev.ConnectionString : "";
                    
                }else
                {
                    connectionString = (connectionStringSettingProd != null) ? connectionStringSettingProd.ConnectionString : "";
                }
                if(connectionString == "" && ConfigurationManager.ConnectionStrings[0] != null) sqlConnection = new SqlConnection(ConfigurationManager.ConnectionStrings[0].ConnectionString);
            } 
        }

        public static string OpenSqlConnection()
        {
            try
            {
                if(sqlConnection == null) return "Database connection failed! Notification email has been sent to the development team. Please try after some time.";
                sqlConnection.Open();
                return "";
            }
            catch (Exception)
            {
                CloseSql();
                return "Database connection failed! Notification email has been sent to the development team. Please try after some time.";
            }
        }

        public static void CloseSql()
        {
            if (sqlConnection != null)
            {
                sqlConnection.Close();
            }
        }


        public static SqlDataReader RunProcedure(String procedureName, string[] parameterName, int?[] paramValue)
        {
            try
            {
                SqlCommand sqlCommand = new SqlCommand();
                sqlCommand.Connection = sqlConnection;
                sqlCommand.CommandType = CommandType.StoredProcedure;
                sqlCommand.CommandText = procedureName;
                for (int i = 0; i < parameterName.Length; i++)
                {
                    sqlCommand.Parameters.Add(new SqlParameter(parameterName[i], paramValue[i]));
                }
                return sqlCommand.ExecuteReader();
            }
            catch (Exception)
            {
                CloseSql();
                return null;
            }
        }

        public static SqlDataReader RunProcedure(String procedureName, string parameterName, int? paramValue)
        {
            try
            {
                SqlCommand sqlCommand = new SqlCommand();
                sqlCommand.Connection = sqlConnection;
                sqlCommand.CommandType = CommandType.StoredProcedure;
                sqlCommand.CommandText = procedureName;
                sqlCommand.Parameters.Add(new SqlParameter(parameterName, paramValue));
                return sqlCommand.ExecuteReader();
            }
            catch (Exception)
            {
                CloseSql();
                return null;
            }
        }

        public static SqlDataReader RunProcedure(String procedureName)
        {
            try
            {
                SqlCommand sqlCommand = new SqlCommand();
                sqlCommand.Connection = sqlConnection;
                sqlCommand.CommandType = CommandType.StoredProcedure;
                sqlCommand.CommandText = procedureName;
                return sqlCommand.ExecuteReader();
            }
            catch (Exception)
            {
                CloseSql();
                return null;
            }
        }

        public static bool IsValidId(ref int? id)
        {
            if (!id.HasValue)
                id = Convert.ToInt32(ConfigurationManager.AppSettings["DefaultId"]);
            if (!validIdArrayList.Contains(id.ToString()))
            {
                id = Convert.ToInt32(ConfigurationManager.AppSettings["DefaultId"]);
                return false;
            }
            return true;
        }
    }
}