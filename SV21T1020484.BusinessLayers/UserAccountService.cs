using SV21T1020484.DataLayers;
using SV21T1020484.DomainModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SV21T1020484.BusinessLayers
{
    public class UserAccountService
    {
        private static readonly IUserAccountDAL employeeAccountDB;
        private static readonly IUserAccountDAL customerDB;

        static UserAccountService()
        {
            string connectionString = Configuration.ConnectionString;
            employeeAccountDB = new DataLayers.Sql.EmployeeAccountDAL(connectionString);
            customerDB = new DataLayers.Sql.CustomerAccountDAL(connectionString);
        }

        public static UserAccount? Authorize(UserTypes userTypes, string username, string password)
        {
            if(userTypes == UserTypes.Employee)
            {
                return employeeAccountDB.Authorize(username, password);
            }
            else
            {
                return customerDB.Authorize(username, password);
            }
        }
        public static bool ChangePass(UserTypes userTypes, string userName, string oldPass, string newPass)
        {
            if (userTypes == UserTypes.Employee)
            {
                return employeeAccountDB.ChangePassword(userName, oldPass, newPass);
            }
            else
            {
                return customerDB.ChangePassword(userName, oldPass, newPass);
            }
        }
        
        public enum UserTypes
        {
            Employee,
            Customer,
        }

    }
}
