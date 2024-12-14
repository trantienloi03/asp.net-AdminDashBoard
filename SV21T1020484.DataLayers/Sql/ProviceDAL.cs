
using Dapper;
using SV21T1020484.DomainModels;

namespace SV21T1020484.DataLayers.Sql
{
    public class ProviceDAL : BaseDAL, ISimpleQueryDAL<Province>
    {
        public ProviceDAL(string connectString) : base(connectString)
        {
        }

        public List<Province> ListNoParam()
        {
            List<Province> data = new List<Province>();
            using( var conn = OpenConnection())
            {
                var sql = "select * from Provinces";
                data = conn.Query<Province>(sql :sql, commandType: System.Data.CommandType.Text).ToList();
                conn.Close();
            }
            return data;
        }
    }
}
