
using Dapper;
using SV21T1020484.DomainModels;
using System.Buffers;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace SV21T1020484.DataLayers.Sql
{
    public class SupplierDAL : BaseDAL, ICommonDAL<Supplier>
    {
        public SupplierDAL(string connectString) : base(connectString)
        {
        }

        public int Add(Supplier data)
        {
            int id = 0;
            
            using (var connection = OpenConnection())
            {
                var sql = @"if exists(select * from Suppliers where SupplierName = @supplierName or Email = @Email)
                                select -1
                            else
                                begin
                                    insert into Suppliers(SupplierName, ContactName, Provice, Address, Phone, Email)
                                    values(@SupplierName, @ContactName, @Province, @Address, @Phone, @Email);
                                    select SCOPE_IDENTITY();
                                end";
                var parameters = new 
                {
                    SupplierName = data.SupplierName??"",
                    ContactName = data.ConTactName??"",
                    Province = data.Province??"",
                    Address = data.Address??"",
                    Phone = data.Phone ?? "",
                    Email = data.Email ??""

                };
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
                                from Suppliers
                                where (SupplierName like @searchvalue)
                                       or (ContactName like @searchValue)
                               ";
                var parameters = new { searchValue=searchValue };
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
                string sql = @"delete from Suppliers where SupplierID = @SupplierID";
                var parameters = new
                {
                    SupplierID = id
                };
                result = connection.Execute(sql: sql, param: parameters, commandType: System.Data.CommandType.Text) > 0;
                connection.Close();
            }
            return result;
        }

        public Supplier? GetByID(int id)
        {
            Supplier? supplier = null;

            using (var connection = OpenConnection())
            {
                string sql = @"select * from Suppliers where SupplierID = @SupplierID";
                var parameters = new
                {
                    SupplierID = id
                };
                supplier= connection.QueryFirstOrDefault<Supplier>(sql: sql, param: parameters, commandType: System.Data.CommandType.Text);
                connection.Close();
            }
            return supplier;
        }

        public bool InUse(int id)
        {
            bool result = false;
            using (var connection = OpenConnection())
            {
                string sql = @"if exists(select * from Products where SupplierID = @SupplierID and IsSelling = 1)
                                    select 1
                               else 
                                     select 0;";
                var parameters = new
                {
                    SupplierID = id
                    
                };
                result = connection.ExecuteScalar<bool>(sql: sql, param: parameters, commandType: System.Data.CommandType.Text);
                connection.Close();
            }
            return result;
        }

        public List<Supplier> List(int page, int pageSize, string searchValue)
        {
            List<Supplier> list = new List<Supplier>();
            searchValue = $"%{searchValue}%";
            using(var connection = OpenConnection())
            {
                string sql = @"select *
                                from(
                                    select *, row_number() over(order by SupplierName) as RowNumber
                                    from Suppliers
                                    where (SupplierName like @searchValue) or 
                                          (ContactName like @searchValue) 
                                    ) as sup
                                where  (@page = 0) or
                                        ( sup.RowNumber between (@page -1)*@pageSize+1 and @page*@pageSize)
                                 order by RowNumber";
                var parameters = new
                {
                    page = page,
                    pageSize = pageSize,
                    searchValue = searchValue
                };
                list = connection.Query<Supplier>(sql: sql, param: parameters, commandType:System.Data.CommandType.Text).ToList();
                connection.Close();
            }
            return list;
        }

        public List<Supplier> GetAll()
        {
            List<Supplier> list = new List<Supplier>();
            using (var conn = OpenConnection())
            {
                string sql = @"select * from Suppliers "
;
                list = conn.Query<Supplier>(sql: sql, commandType: System.Data.CommandType.Text).ToList();
                conn.Close();
            }
            return list;
        }

        public bool Update(Supplier data)
        {
            bool result = false;
            using (var connection = OpenConnection())
            {
                var sql = @"if not exists(select * from Suppliers where SupplierID <> @SupplierID and Email = @Email)
                                begin
                                   update Suppliers
                                   set 
                                       SupplierName = @SupplierName,
                                       ContactName = @ContactName,
                                       Provice = @Province,
                                       Address = @Address,
                                       Phone = @Phone,
                                       Email = @Email
                                   where SupplierID = @SupplierID
                                end";
                var parameters = new
                {
                    SupplierName = data.SupplierName ?? "",
                    ContactName = data.ConTactName ?? "",
                    Province = data.Province ?? "",
                    Address = data.Address ?? "",
                    Phone = data.Phone ?? "",
                    Email = data.Email ?? "",
                    SupplierID = data.SupplierID

                };
                result = connection.Execute(sql: sql, param: parameters, commandType: System.Data.CommandType.Text) > 0;
                connection.Close();
            }
            return result;
        }
    }
}
