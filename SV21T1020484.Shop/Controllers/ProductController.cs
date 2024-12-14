using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SV21T1020484.BusinessLayers;
using SV21T1020484.DomainModels;
using SV21T1020484.Shop.Models;

namespace SV21T1020484.Shop.Controllers
{
    public class ProductController : Controller
    {
        private const string SHOPPING_CART = "ShoppingCart";
        [HttpGet]
        [AllowAnonymous]   
        public IActionResult Detail(int productID)
        {
            Product? model = SV21T1020484.BusinessLayers.ProductDataService.GetProductById(productID);
            if (model == null)
            {
                // Trường hợp không tìm thấy sản phẩm
                return RedirectToAction("Index", "Home");
            }
            return View(model);
        }
        [Authorize]
        public IActionResult AddtoCart(int productID)
        {
            var customer = User.GetUserData();
            var Cart = CartDataService.getCartByCustomerID(Convert.ToInt32(customer.UserId));
            if (Cart == null)
            {
                var newCart = new Cart();
                newCart.CustomerID = Convert.ToInt32(customer.UserId);
                newCart.Sum = 0;
                Cart = newCart;
                CartDataService.AddCart(Cart);
            }
            Cart = CartDataService.getCartByCustomerID(Convert.ToInt32(customer.UserId));
            var product = ProductDataService.GetProductById(productID);
            if (product != null)
            {
                var cartID = Cart.CartId;
                var exists = CartDataService.checkProductExists(cartID, product.ProductID);
                if(exists == null)
                {
                    var cartDetail = new Cartdetail();
                    cartDetail.Quantity = 1;
                    cartDetail.Price = product.Price;
                    cartDetail.CartId = Cart.CartId;
                    cartDetail.ProductId = product.ProductID;
                    int id = CartDataService.AddCartDetail(cartDetail);
                    int sum = Cart.Sum + 1;
                    Cart.Sum = sum;
                    bool kq = CartDataService.SaveCart(Cart);
                }
                else
                {
                    bool id = CartDataService.SaveCartdetail(Cart.CartId, product.ProductID, exists.Quantity + 1);
                }
                
            }

            return RedirectToAction("Index", "Home");
        }
       

    }
}
