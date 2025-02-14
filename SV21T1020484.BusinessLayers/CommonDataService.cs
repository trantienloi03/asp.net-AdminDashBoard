
using SV21T1020484.DataLayers;
using SV21T1020484.DataLayers.Sql;
using SV21T1020484.DomainModels;

namespace SV21T1020484.BusinessLayers
{
    public static class CommonDataService
    {
        private static readonly ISimpleQueryDAL<Province> ProvinceDB;
        private static readonly ICommonDAL<Customer> CustomerDB;
        private static readonly ICommonDAL<Shipper> ShipperDB;
        private static readonly ICommonDAL<Employee> EmployeeDB;
        private static readonly ICommonDAL<Supplier> SupplierDB;
        private static readonly ICommonDAL<Category> CategoryDB;
        private static readonly ICommonDAL<CustomerDTO> CustomerDTO;

        static CommonDataService()
        {
            string connectionString = Configuration.ConnectionString;
            CustomerDB = new DataLayers.Sql.CustomerDAL(connectionString);
            ShipperDB = new DataLayers.Sql.ShipperDAL(connectionString);
            EmployeeDB = new DataLayers.Sql.EmployeeDAL(connectionString);
            SupplierDB = new DataLayers.Sql.SupplierDAL(connectionString);
            CategoryDB = new DataLayers.Sql.CategoryDAL(connectionString);
            ProvinceDB = new DataLayers.Sql.ProviceDAL(connectionString);
            CustomerDTO = new DataLayers.Sql.CustomerDTO_DAL(connectionString);
            

        }
        public static List<Province> ListOfProvices() 
        {
            
            return ProvinceDB.ListNoParam();
        }
        public static List<Category> GetAllCategory()
        {
            return CategoryDB.GetAll();
        }
        public static List<Supplier> GetAllSupplier()
        {
            return SupplierDB.GetAll();
        }
        public static List<Customer> GetAllCustomer()
        {
            return CustomerDB.GetAll();
        }
        public static Customer GetCustomer(int id)
        {
            return CustomerDB.GetByID(id);
        }
        public static int AddCustomer(Customer data)
        {
            return CustomerDB.Add(data);
        }
        public static int AddCustomerDTO(CustomerDTO data)
        {
            return CustomerDTO.Add(data);
        }
        public static bool UpdateCustomer(Customer data)
        {
            return CustomerDB.Update(data);
        }
        public static bool DeleteCustomer(int id)
        {
            return CustomerDB.Delete(id);
        }
        public static bool InUse(int id)
        {
            return CustomerDB.InUse(id);
        }
        //
         public static Category GetCategory(int id)
        {
            return CategoryDB.GetByID(id);
        }
        public static int AddCategory(Category data)
        {
            return CategoryDB.Add(data);
        }
        public static bool UpdateCategory(Category data)
        {
            return CategoryDB.Update(data);
        }
        public static bool DeleteCategory(int id)
        {
            return CategoryDB.Delete(id);
        }
        public static bool InUseCategory(int id)
        {
            return CategoryDB.InUse(id);
        }
        //
         public static Employee GetEmployee(int id)
        {
            return EmployeeDB.GetByID(id);
        }
        public static int AddEmployee(Employee data)
        {
            return EmployeeDB.Add(data);
        }
        public static bool UpdateEmployee(Employee data)
        {
            return EmployeeDB.Update(data);
        }
        public static bool DeleteEmployee(int id)
        {
            return EmployeeDB.Delete(id);
        }
        public static bool InUseEmployee(int id)
        {
            return EmployeeDB.InUse(id);
        }
        //
        public static Shipper GetShipper(int id)
        {
            return ShipperDB.GetByID(id);
        }
        public static int AddShipper(Shipper data)
        {
            return ShipperDB.Add(data);
        }
        public static bool UpdateShipper(Shipper data)
        {
            return ShipperDB.Update(data);
        }
        public static bool DeleteShipper(int id)
        {
            return ShipperDB.Delete(id);
        }
        public static bool InUseShipper(int id)
        {
            return ShipperDB.InUse(id);
        }
        //
        public static Supplier GetSupplier(int id)
        {
            return SupplierDB.GetByID(id);
        }
        public static int AddSupplier(Supplier data)
        {
            return SupplierDB.Add(data);
        }
        public static bool UpdateSupplier(Supplier data)
        {
            return SupplierDB.Update(data);
        }
        public static bool DeleteSupplier(int id)
        {
            return SupplierDB.Delete(id);
        }
        public static bool InUseSupplier(int id)
        {
            return SupplierDB.InUse(id);
        }
        public static List<Customer> ListOfCustomers(out int rowCount, int page = 1, int pageSize = 0, string searchValue="") {
                rowCount = CustomerDB.Count(searchValue);
                return CustomerDB.List(page, pageSize, searchValue);
            }
        public static List<Shipper> ListOfShippers(out int rowCount, int page = 1, int pageSize = 0, string searchValue = "")
        {
            rowCount = ShipperDB.Count(searchValue);
            return ShipperDB.List(page, pageSize, searchValue);
        }

        public static List<Employee> ListOfEmployees(out int rowCount, int page = 1, int pageSize = 0, string searchValue ="")
        {
            rowCount = EmployeeDB.Count(searchValue);
            return EmployeeDB.List(page, pageSize, searchValue);
        }
        public static List<Supplier> ListOfSuppliers(out int rowCount, int page = 1, int pageSize = 0, string searchValue = "")
        {
            rowCount = SupplierDB.Count(searchValue);
            return SupplierDB.List(page, pageSize, searchValue);
        }
        public static List<Category> ListOfCategories(out int rowCount, int page = 1, int pageSize = 0, string searchValue = "")
        {
            rowCount = CategoryDB.Count(searchValue);
            return CategoryDB.List(page, pageSize, searchValue);
        }
    }
}
