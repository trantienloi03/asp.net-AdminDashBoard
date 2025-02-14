using Microsoft.AspNetCore.Authentication;
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
        public IActionResult AddtoCart(int ProductID, int quantity)
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
            var product = ProductDataService.GetProductById(ProductID);
            if (product != null)
            {
                var CartID = Cart.CartId;
                int productID = product.ProductID;
                var exists = CartDataService.checkProductExists(CartID, productID);
                if (exists == null)
                {
                    var data = new Cartdetail();
                    data.Quantity = quantity;
                    data.Price = product.Price;
                    data.CartId = Cart.CartId;
                    data.ProductId = product.ProductID;
                    int id = CartDataService.AddCartDetail(data);
                    int sum = Cart.Sum + 1;
                    Cart.Sum = sum;
                    bool kq = CartDataService.SaveCart(Cart);

                    var user = User.GetUserData();
                    int userID = Convert.ToInt32(user.UserId);

                    HttpContext.Session.SetInt32("CartItemCount", Cart.Sum);
                }
                else
                {
                    int Quantity = exists.Quantity + quantity;
                    bool id = CartDataService.SaveCartdetail(CartID, ProductID, Quantity);
                }

            }
            return RedirectToAction("Index", "Home");
        }


    }
}
