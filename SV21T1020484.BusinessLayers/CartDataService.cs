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
        public static int AddCart(Cart data)
        {
            return cartDB.Add(data);
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
        public static Cartdetail checkProductExists(int cartID, int productID)
        {
            return cartDB.checkProductExists(cartID, productID);
        }
        public static bool SaveCartdetail(int cartID, int quantity, int productID) {
            return cartDB.SaveDetail(cartID, productID, quantity);
        }
        public static bool SaveCart(Cart data)
        {
            return cartDB.Update(data);
        }
    }
}
