using SV21T1020484.DomainModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SV21T1020484.DataLayers
{
    public interface ICartDAL
    {
        public int Add(Cart data);
        public int AddCartDetail(Cartdetail data);
        public int Delete(int customerID);
        public bool Update(Cart data);
        public int Count(int customerID);
        public Cart GetByID(int cutomerID);
        public List<Cartdetail> GetDetailList(int cartID);
        public Cartdetail GetDetail(int cartID, int productID);
        bool SaveDetail(int cartDetailID, int productID, int quantity);
        bool DeleteDetail(int cartID,int cartDetailID);
        bool SaveCart(int customerID, int sum);
        public Cartdetail? checkProductExists(int cartID, int productID);
        public List<ViewCart> GetViewCarts(int userID);
    }
}
