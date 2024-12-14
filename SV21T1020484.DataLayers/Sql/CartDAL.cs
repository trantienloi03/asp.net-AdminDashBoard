using Dapper;
using SV21T1020484.DomainModels;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace SV21T1020484.DataLayers.Sql
{
    public class CartDAL : BaseDAL, ICartDAL
    {
        public CartDAL(string connectString) : base(connectString)
        {
        }

        public int Add(Cart data)
        {
            int id = 0;

            using (var connection = OpenConnection())
            {
                var sql = @"insert into Carts(Sum, CustomerID)
                                    values(@Sum, @CustomerID);
                                    select SCOPE_IDENTITY();";
                var parameters = new
                {
                    Sum = data.Sum,
                    CustomerID = data.CustomerID
                };
                id = connection.ExecuteScalar<int>(sql: sql, param: parameters, commandType: System.Data.CommandType.Text);
                connection.Close();
            }
            return id;
        }

        public int AddCartDetail(Cartdetail data)
        {
            int id = 0;

            using (var connection = OpenConnection())
            {
                var sql = @"insert into Cartdetails(Quantity, Price, CartID, ProductID)
                                    values(@Quantity, @Price, @CartID, @ProductID);
                                    select SCOPE_IDENTITY();";
                var parameters = new
                {
                    Quantity = data.Quantity,
                    Price = data.Price,
                    CartID = data.CartId,
                    ProductID = data.ProductId
                   
                };
                id = connection.ExecuteScalar<int>(sql: sql, param: parameters, commandType: System.Data.CommandType.Text);
                connection.Close();
            }
            return id;
        }

        public Cartdetail? checkProductExists(int cartID, int productID)
        {
            Cartdetail? data = new Cartdetail();

            using (var connection = OpenConnection())
            {
                var sql = @"select * from Cartdetails where CartID = @CartID and ProductID = @ProductID";
                var parameters = new
                {
                    CartID = cartID,
                    ProductID = productID 

                };
                data = connection.QueryFirstOrDefault<Cartdetail>(sql: sql, param: parameters, commandType: System.Data.CommandType.Text);
                connection.Close();
            }
            return data;

        }

        public int Count(int customerID)
        {
            int count = 0;
            using (var connection = OpenConnection())
            {
                string sql = @" select count(*)
                                from Carts
                                where CustomerID = @CustomerID;
                               ";
                var parameters = new { CustomerID = customerID };
                count = connection.ExecuteScalar<int>(sql: sql, param: parameters, commandType: System.Data.CommandType.Text);
                connection.Close();
            }
            return count;
        }

        public int Delete(int customerID)
        {
            throw new NotImplementedException();
        }

        public int DeleteAll(int customerID)
        {
            throw new NotImplementedException();
        }

        public bool DeleteDetail(int cartID, int productID)
        {
            throw new NotImplementedException();
        }

        public Cart GetByID(int customerID)
        {
            var cart = new Cart();
            using (var connection = OpenConnection())
            {
                string sql = @" select * from Carts
                                where CustomerID = @CustomerID";
                var parameters = new
                {
                    CustomerID = customerID
                };
                cart = connection.QueryFirstOrDefault<Cart>(sql: sql, param: parameters, commandType: System.Data.CommandType.Text);
                connection.Close();
            }
            return cart;
        }

        public Cartdetail GetDetail(int cartID, int productID)
        {
            throw new NotImplementedException();
        }

        public List<Cartdetail> GetDetailList(int cartID)
        {
            throw new NotImplementedException();
        }

        public bool SaveCart(int customerID, int sum)
        {
            throw new NotImplementedException();
        }

        public bool SaveDetail(int cartID, int productID, int quantity)
        {
            bool result = false;

            using (var connection = OpenConnection())
            {
                var sql = @"update Cartdetails set Quantity = @Quantity 
                            where CartID = @CartID and ProductID = @ProductID";
                var parameters = new
                {
                    Quantity = quantity,
                    CartID = cartID,
                    ProductID = productID

                }; 
                result = connection.Execute(sql: sql, param: parameters, commandType: System.Data.CommandType.Text) > 0;
                connection.Close();
            }
            return result;
        }

        public bool Update(Cart data)
        {
            bool result = false;

            using (var connection = OpenConnection())
            {
                var sql = @"update Carts set Sum = @Sum 
                            where CustomerID = @CustomerID and CartID = @CartID";
                var parameters = new
                {
                    Sum = data.Sum,
                    CustomerID = data.CustomerID,
                    CartID = data.CartId

                };
                result = connection.Execute(sql: sql, param: parameters, commandType: System.Data.CommandType.Text) > 0;
                connection.Close();
            }
            return result;
        }
    }
}
