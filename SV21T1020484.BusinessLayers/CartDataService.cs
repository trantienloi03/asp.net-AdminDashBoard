using SV21T1020484.DataLayers;
using SV21T1020484.DataLayers.Sql;
using SV21T1020484.DomainModels;


namespace SV21T1020484.BusinessLayers
{
    public static class CartDataService
    {
        private static readonly ICartDAL cartDB;

        static CartDataService()
        {
           cartDB = new CartDAL(Configuration.ConnectionString);
        }
        public static int AddCart(Cart Cart)
        {
            return cartDB.Add(Cart);
        }
        public static int AddCartDetail(Cartdetail data)
        {
            return cartDB.AddCartDetail(data);
        }
        public static int Count(int customerID)
        {
            return cartDB.Count(customerID);
        }
        public  static Cart getCartByCustomerID(int cutomerID)
        {
            return cartDB.GetByID(cutomerID);
        }
        public static Cartdetail checkProductExists(int CartID, int productID)
        {
            return cartDB.checkProductExists(CartID, productID);
        }
        public static bool SaveCartdetail(int cartDetailID, int productID, int quantity) {
            return cartDB.SaveDetail(cartDetailID, productID, quantity);
        }
        public static bool SaveCart(Cart Cart)
        {
            return cartDB.Update(Cart);
        }
        public static List<ViewCart> listViewCart(int userID)
        {
            return cartDB.GetViewCarts(userID);
        }
        public static bool DeleteDetail(int cartID, int cartDetailID)
        {
            return cartDB.DeleteDetail(cartID, cartDetailID);
        }
    }
}
