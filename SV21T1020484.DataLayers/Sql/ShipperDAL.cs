using Dapper;
using SV21T1020484.DomainModels;
using System.Buffers;

namespace SV21T1020484.DataLayers.Sql
{
    public class ShipperDAL : BaseDAL, ICommonDAL<Shipper>
    {
        public ShipperDAL(string connectString) : base(connectString)
        {
        }

        public int Add(Shipper data)
        {
            int id = 0;
            using (var connection = OpenConnection())
            {
                var sql = @"if exists(select * from Shippers where Phone = @Phone)
                                select -1
                            else
                                begin
                                   insert into Shippers(ShipperName, Phone)
                                    values(@ShipperName, @Phone)
                                    select scope_identity();
                                end";
       
                var parameters = new {ShipperName  = data.ShipperName, Phone = data.Phone };
                id = connection.ExecuteScalar<int>(sql: sql, param: parameters, commandType: System.Data.CommandType.Text);
                connection.Close();
            }
            return id;
        }

        public int Count(string searchValue)
        {
            int count = 0;
            searchValue = $"%{searchValue}%";
            using (var connection = OpenConnection())
            {
                string sql = @" select count(*)
                        from Shippers
                        where (ShipperName like @searchvalue)";
                var parameters = new { searchValue = searchValue };
                count = connection.ExecuteScalar<int>(sql: sql, param: parameters, commandType: System.Data.CommandType.Text);
                connection.Close();
            }
            return count;
        }

        public bool Delete(int id)
        {
            bool result = false;
            using (var connection = OpenConnection())
            {
                string sql = @" delete from Shippers where ShipperID = @ShipperID";
                var parameters = new { ShipperID = id };
                result = connection.Execute(sql: sql, param: parameters, commandType: System.Data.CommandType.Text) > 0;
                connection.Close();
            }
            return result;
        }

        public List<Shipper> GetAll()
        {
            throw new NotImplementedException();
        }

        public Shipper? GetByID(int id)
        {
            Shipper? shipper = null;
            using (var connection = OpenConnection())
            {
                string sql = @" select * from Shippers
                        where (ShipperID = @ShipperID)";
                var parameters = new { ShipperID = id };
                shipper = connection.QueryFirstOrDefault<Shipper>(sql: sql, param: parameters, commandType: System.Data.CommandType.Text);
                connection.Close();
            }
            return shipper;
        }

        public bool InUse(int id)
        {
            bool result = false;
            using (var conn = OpenConnection())
            {
                var sql = @"if exists(select * from Orders where ShipperID = @ShipperID)
                                select 1
                            else
                                select 0;";
                var parameters = new { ShipperID = id };
                result = conn.ExecuteScalar<bool>(sql: sql, param: parameters, commandType: System.Data.CommandType.Text);
                conn.Close();
            }
            return result;
        }

        public List<Shipper> List(int page, int pageSize, string searchValue)
        {
            List<Shipper> list = new List<Shipper>();
            searchValue = $"%{searchValue}%";

            using (var connection = OpenConnection())
            {
                string sql = @"
                                select *
                                from (	select *, ROW_NUMBER() over(order by ShipperName) as RowNumber
		                                from Shippers
		                                where ShipperName like @searchvalue 
		                                ) as s
                                where (@pageSize=0) or
		                                (s.RowNumber between (@page -1)*@pageSize +1 and @page*@pageSize)
                                order by RowNumber";
                var parametrs = new
                {
                    page = page,
                    pageSize = pageSize,
                    searchValue = searchValue
                };
                list = connection.Query<Shipper>(sql: sql, param: parametrs, commandType: System.Data.CommandType.Text).ToList();
                connection.Close();
            }

            return list;
        }

        public bool Update(Shipper data)
        {
            bool result = false;
            using (var connection = OpenConnection())
            {

                var sql = @"if not exists(select * from Shippers where ShipperID <> @ShipperID and Phone = @Phone )
                                begin
                                    update Shippers
                                    set ShipperName = @ShipperName,
                                        Phone = @Phone
                                    where ShipperID = @ShipperID    
                                end";
                var parameters = new { ShipperName = data.ShipperName, Phone = data.Phone, ShipperID = data.ShipperID};
                result = connection.Execute(sql: sql, param: parameters, commandType: System.Data.CommandType.Text)> 0;
                connection.Close();
            }
            return result;
        }
    }
}
