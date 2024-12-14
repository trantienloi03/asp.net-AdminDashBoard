using Dapper;
using SV21T1020484.DomainModels;
using System.Data;

namespace SV21T1020484.DataLayers.Sql
{
    public class CustomerDAL :BaseDAL, ICommonDAL<Customer>
    {
        public CustomerDAL(string connectString) : base(connectString)
        {
        }

        public List<Customer> GetAll()
        {
            List<Customer> list = new List<Customer>();
            using (var conn = OpenConnection())
            {
                String sql = @"select * from Customers "
;
                list = conn.Query<Customer>(sql: sql, commandType: System.Data.CommandType.Text).ToList();
                conn.Close();
            }
            return list;
        }

        int ICommonDAL<Customer>.Add(Customer data)
        {
            int id = 0;
            using(var conn = OpenConnection())
            {
                var sql = @"if exists(select * from Customers where Email = @Email)
                                select -1
                            else
                                begin
                                    insert into Customers(CustomerName, ContactName, Province, Address, Phone, Email, IsLocked)
                                    values (@CustomerName, @ContactName, @Province, @Address, @Phone, @Email, @IsLocked);
                                    select SCOPE_IDENTITY();
                                end";
                var parameters = new 
                { 
                    CustomerName = data.CustomerName ?? "",
                    ContactName = data.ContactName ?? "",
                    Province = data.Province ?? "",  
                    Address = data.Address ?? "",
                    Email = data.Email ?? "",
                    Phone = data.Phone ??"",
                    Islocked = data.IsLocked
                };
                id = conn.ExecuteScalar<int>(sql: sql,param: parameters, commandType: System.Data.CommandType.Text);
                conn.Close();
            }
            return id;

        }

        int ICommonDAL<Customer>.Count(string searchValue)
        {
            int count=0;
            searchValue = $"%{searchValue}%";
            using(var connection = OpenConnection())
            {
                string sql = @" select count(*)
                        from Customers
                        where (CustomerName like @searchvalue) or ( ContactName like @searchvalue)
                        ";
                var parameters = new { searchValue = searchValue};
                count = connection.ExecuteScalar<int>(sql: sql, param: parameters, commandType:System.Data.CommandType.Text);
                connection.Close();
            }
            return count;
        }

        bool ICommonDAL<Customer>.Delete(int id)
        {
            bool result = false;
            using(var conn = OpenConnection())
            {
                string sql = @"delete from Customers where CustomerID = @CustomerID";
                var parameters = new { CustomerID = id };
                result = conn.Execute(sql: sql, param: parameters, commandType: System.Data.CommandType.Text) > 0;
                conn.Close() ;
            }
            return result;
        }

        Customer? ICommonDAL<Customer>.GetByID(int id)
        {
            Customer? data = null;
            using(var conn = OpenConnection())
            {
                var sql = @"select * from Customers where CustomerID = @CustomerID";
                var parameters = new { CustomerID = id };
                data = conn.QueryFirstOrDefault<Customer>(sql: sql, param: parameters, commandType: System.Data.CommandType.Text);
                conn.Close() ;
            }

        return data;
        }

        bool ICommonDAL<Customer>.InUse(int id)
        {
            bool result = false;
            using(var conn = OpenConnection())
            {
                var sql = @"if exists(select * from Orders where CustomerID = @CustomerID)
                                select 1
                            else
                                select 0;";
                var parameters = new { CustomerID = id };
                result = conn.ExecuteScalar<bool>(sql: sql, param: parameters, commandType : System.Data.CommandType.Text);
                conn.Close();
            }
            return result;
        }
        List<Customer> ICommonDAL<Customer>.List(int page, int pageSize, string searchValue)
        {
            List<Customer> list = new List<Customer>();
            searchValue = $"%{searchValue}%";

            using (var connection = OpenConnection())
            {
                string sql = @"
                                select *
                                from (	select *, ROW_NUMBER() over(order by CustomerName) as RowNumber
		                                from Customers
		                                where CustomerName like @searchvalue or 
			                                  ContactName like @searchvalue
		                                ) as t
                                where (@pageSize=0) or
		                                (t.RowNumber between (@page -1)*@pageSize +1 and @page*@pageSize)
                                order by RowNumber";
                var parametrs = new
                {
                    page = page,
                    pageSize = pageSize,
                    searchValue = searchValue
                };
                list = connection.Query<Customer>(sql: sql, param: parametrs, commandType: System.Data.CommandType.Text).ToList();
                connection.Close();
            }

            return list;
        }

        bool ICommonDAL<Customer>.Update(Customer data)
        {
            bool result = false;
            using (var conn = OpenConnection())
            {
                var sql = @"if not exists(select * from Customers where CustomerId <> @CustomerID and Email = @Email)
                                begin
                                    update Customers
                                    set
                                        CustomerName = @CustomerName,
                                        ContactName = @ContactName,
                                        Province = @Province,
                                        Address = @Address,
                                        Phone = @Phone,
                                        Email = @Email,
                                        IsLocked = @IsLocked
                                    where CustomerID = @CustomerID
                                end";
                var parameters = new 
                { 
                    CustomerName = data.CustomerName ??"",
                    ContactName = data.ContactName ?? "",
                    Province = data.Province ??"",
                    Address = data.Address ?? "",
                    Phone = data.Phone ??"",
                    Email = data.Email??"",
                    Islocked = data.IsLocked,
                    CustomerID = data.CustomerID
                };
                result = conn.Execute(sql: sql, param: parameters, commandType: System.Data.CommandType.Text) > 0;
                conn.Close();
            }
            return result;
        }
    }
}
