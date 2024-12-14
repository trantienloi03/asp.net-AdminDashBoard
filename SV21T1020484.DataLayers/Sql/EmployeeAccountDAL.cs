using Dapper;
using SV21T1020484.DomainModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SV21T1020484.DataLayers.Sql
{
    public class EmployeeAccountDAL : BaseDAL, IUserAccountDAL
    {
        public EmployeeAccountDAL(string connectString) : base(connectString)
        {
        }

        public UserAccount? Authorize(string username, string password)
        {
            UserAccount? userAccount = null;
            using(var conn = OpenConnection())
            {
                var sql = @"select EmployeeID as UserId,
                                   Email as UserName,
                                   FullName as DisplayName,
                                   Photo,
                                   Role
                            from Employees where Email = @Email and Password = @Password";
                var parameters = new
                {
                    Email = username,
                    Password = password
                };
                userAccount = conn.QueryFirstOrDefault<UserAccount>(sql: sql, param: parameters, commandType: System.Data.CommandType.Text);
                conn.Close();
            }
            return userAccount;
        }

        public bool ChangePassword(string userName, string oldPassword, string newPassword)
        {
            bool result = false;
            using (var cn = OpenConnection())
            {
                var sql = @"update Employees 
                            set Password = @newPassword 
                            where Email = @userName and Password = @oldPassword";
                var parameters = new
                {
                    userName = userName,
                    oldPassword = oldPassword,
                    newPassword = newPassword
                };
                result = cn.Execute(sql: sql,param: parameters, commandType :System.Data.CommandType.Text) > 0;
                cn.Close();
            }
            return result;
        }
    }
}
