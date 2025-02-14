using Dapper;
using SV21T1020484.DomainModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SV21T1020484.DataLayers.Sql
{
    public class CustomerDTO_DAL : BaseDAL, ICommonDAL<CustomerDTO>
    {
        public CustomerDTO_DAL(string connectString) : base(connectString)
        {
        }

        public int Add(CustomerDTO data)
        {
            int id = 0;
            using (var conn = OpenConnection())
            {
                var sql = @"if exists(select * from Customers where Email = @Email)
                                select -1
                            else
                                begin
                                    insert into Customers(CustomerName, ContactName, Email, Password, IsLocked)
                                    values (@CustomerName, @ContactName, @Email,@Password, @IsLocked);
                                    select SCOPE_IDENTITY();
                                end";
                var parameters = new
                {
                    CustomerName = data.CustomerName ?? "",
                    ContactName = data.ContactName ?? "",
                    Email = data.Email ?? "",
                    Password = data.password ??"",
                    IsLocked = data.IsLocked
                };
                id = conn.ExecuteScalar<int>(sql: sql, param: parameters, commandType: System.Data.CommandType.Text);
                conn.Close();
            }
            return id;
        }

        public int Count(string searchValue = "")
        {
            throw new NotImplementedException();
        }

        public bool Delete(int id)
        {
            throw new NotImplementedException();
        }

        public List<CustomerDTO> GetAll()
        {
            throw new NotImplementedException();
        }

        public CustomerDTO? GetByID(int id)
        {
            throw new NotImplementedException();
        }

        public bool InUse(int id)
        {
            throw new NotImplementedException();
        }

        public List<CustomerDTO> List(int page = 1, int pageSize = 0, string searchValue = "")
        {
            throw new NotImplementedException();
        }

        public bool Update(CustomerDTO data)
        {
            throw new NotImplementedException();
        }
    }
}
