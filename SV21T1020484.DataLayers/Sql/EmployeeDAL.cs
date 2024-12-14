using Dapper;
using SV21T1020484.DomainModels;
using System.Data;

namespace SV21T1020484.DataLayers.Sql
{
    public class EmployeeDAL : BaseDAL, ICommonDAL<Employee>
    {
        public EmployeeDAL(string connectString) : base(connectString)
        {
        }

        public int Add(Employee data)
        {
            int id = 0;
            using(var conn = OpenConnection())
            {
                string sql = @"if exists(select * from Employees where Email = @Email)
                                select -1
                            else
                                begin
                                    insert into Employees(FullName, BirthDate, Address, Phone, Email, Photo, IsWorking)
                                    values(@FullName, @BirthDate, @Address, @Phone, @Email, @Photo, @IsWorking);

                                    select @@identity;
                                end";
                var parameters = new 
                {
                    FullName = data.FullName,
                    BirthDate = data.BirthDate,
                    Address = data.Address,
                    Phone = data.Phone,
                    Email = data.Email,
                    Password = data.PassWord,
                    Photo = data.Photo??"",
                    IsWorking = data.IsWorking
                };
                id = conn.ExecuteScalar<int>(sql: sql, param: parameters, commandType:System.Data.CommandType.Text);
                conn.Close();
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
                        from Employees
                        where (FullName like @searchvalue)";
                var parameters = new { searchValue = searchValue };
                count = connection.ExecuteScalar<int>(sql: sql, param: parameters, commandType: System.Data.CommandType.Text);
                connection.Close();
            }
            return count;
        }

        public bool Delete(int id)
        {
            bool result = false;
            using(var conn = OpenConnection())
            {
                string sql = @"delete from Employees where EmployeeID = @EmployeeID";
                var parameters = new { EmployeeID = id };
                result = conn.Execute(sql:sql, param: parameters, commandType:System.Data.CommandType.Text) > 0;   
            }
            return result;
        }

        public List<Employee> GetAll()
        {
            throw new NotImplementedException();
        }

        public Employee? GetByID(int id)
        {
            Employee? employee = null;
            using (var conn = OpenConnection())
            {
                string sql = @"select * from Employees where EmployeeID = @EmployeeID";
                var parameters = new { EmployeeID = id };
                employee =conn.QueryFirstOrDefault<Employee>(sql: sql, param: parameters, commandType: System.Data.CommandType.Text);
            }
            return employee;
        }

        public bool InUse(int id)
        {
            bool result = false;
            using (var conn = OpenConnection())
            {
                string sql = @"if exists(select * from Employees where EmployeeID = @EmployeeID and IsWorking = 1)
                                    select 1
                                else 
                                    select 0;";
                var parameters = new { EmployeeID = id };
                result = conn.ExecuteScalar<bool>(sql: sql, param: parameters, commandType: System.Data.CommandType.Text) ;
            }
            return result;
        }

        public List<Employee> List(int page, int pageSize, string searchValue)
        {
            List<Employee> list = new List<Employee>();
            searchValue = $"%{searchValue}%";

            using (var connection = OpenConnection())
            {
                string sql = @"
                                select *
                                from (	select *, ROW_NUMBER() over(order by FullName) as RowNumber
		                                from Employees
		                                where FullName like @searchvalue
		                                ) as t
                                where (@pageSize=0) or
		                                (t.RowNumber between (@page -1)*@pageSize +1 and @page*@pageSize)
                                order by RowNumber";
                var parametrs = new
                {
                    page= page,
                    pageSize=pageSize,
                    searchValue = searchValue
                };
                list = connection.Query<Employee>(sql: sql, param: parametrs, commandType: System.Data.CommandType.Text).ToList();
                connection.Close();
            }

            return list;
        }

        public bool Update(Employee data)
        {
            bool result = false;
            using (var conn = OpenConnection())
            {
                string sql = @"if not exists(select * from Employees where EmployeeID <> @EmployeeID and Email = @Email)
                                begin
                                    update Employees 
                                    set 
                                        FullName = @FullName,
                                        BirthDate = @BirthDate,
                                        Address = @Address,
                                        Phone = @Phone,
                                        Email = @Email,
                                        Photo = @Photo,
                                        IsWorking = @IsWorking
                                    where EmployeeID = @EmployeeID;
                                end";
                var parameters = new 
                { 
                    EmployeeID = data.EmployeeID,
                    FullName = data.FullName ?? "",
                    BirthDate = data.BirthDate,
                    Address = data.Address ?? "",
                    Email = data.Email ??"",
                    Photo = data.Photo,
                    IsWorking = data.IsWorking,
                    Phone = data.Phone ??""
                };
                result = conn.Execute(sql: sql, param: parameters, commandType: System.Data.CommandType.Text) > 0;
            }
            return result;
        }
    }

     
    
}
