using Microsoft.Data.SqlClient;

namespace SV21T1020484.DataLayers.Sql
{
    /// <summary>
    /// lớp cha
    /// </summary>
    public abstract class BaseDAL
    {
        /// <summary>
        /// chuỗi kết nối đến sql
        /// </summary>
        protected string connectString="";
        protected BaseDAL(string connectString) {
            this.connectString = connectString;
        }
        protected SqlConnection OpenConnection()
        {
            SqlConnection conn = new SqlConnection(connectString);
            conn.Open();
            return conn;
        }
        
    }
}
