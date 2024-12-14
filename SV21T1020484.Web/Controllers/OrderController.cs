using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SV21T1020484.BusinessLayers;
using SV21T1020484.DomainModels;
using SV21T1020484.Web.Models;
using System.Globalization;

namespace SV21T1020484.Web.Controllers
{
    [Authorize(Roles = $"{WebUserRoles.EMPLOYEE}, {WebUserRoles.ADMINISTRATOR}")]

    public class OrderController : Controller
    {
        private const int PAGE_SIZE = 20;
        private const string ORDER_SEARCH_CONDITON = "OrderSearchCondition";
        private const int PRODUCT_PAGE_SIZE = 5;
        private const string PRODUCT_SEARCH_CONDITION = "ProductSearchForSale";
        private const string SHOPPING_CART = "ShoppingCart";
        public IActionResult Index()
        {
            OrderSearchInput? condition = ApplicationContext.GetSessionData<OrderSearchInput>(ORDER_SEARCH_CONDITON);
            if (condition == null)
            {
                var cultureInfo = new CultureInfo("en-GB");
                condition = new OrderSearchInput
                {
                    Page = 1,
                    PageSize = PAGE_SIZE,
                    SearchValue = "",
                    Status = 0,
                    TimeRange = $"{DateTime.Today.AddDays(-7).ToString("dd/MM/yyyy", cultureInfo)} - {DateTime.Today.ToString("dd/MM/yyyy", cultureInfo)}"

                };
            }
            return View(condition);
        }
        public IActionResult Search(OrderSearchInput condition)
        {
            int rowCount;
            var data = OrderDataService.ListOrders(out rowCount, condition.Page, condition.PageSize,
                                                   condition.Status, condition.FromTime, condition.ToTime,
                                                   condition.SearchValue ?? "");
            var model = new OrderSearchResult()
            {
                Page = condition.Page,
                PageSize = condition.PageSize,
                SearchValue = condition.SearchValue ?? "",
                Status = condition.Status,
                TimeRange = condition.TimeRange,
                RowCount = rowCount,
                Data = data
            };

            ApplicationContext.SetSessionData(ORDER_SEARCH_CONDITON, condition);
            return View(model);
        }
        public IActionResult Create()
        {
            var condition = ApplicationContext.GetSessionData<ProductSearchInput>(PRODUCT_SEARCH_CONDITION);
            if(condition == null)
            {
                condition = new ProductSearchInput
                {
                    Page = 1,
                    PageSize = PRODUCT_PAGE_SIZE,
                    SearchValue =""
                };
                
            }
            return View(condition);
        }
        public IActionResult SearchProduct(ProductSearchInput condition)
        {
            int rowCount = 0;
            var data = ProductDataService.ListProducts(out rowCount, condition.Page, condition.PageSize, condition.SearchValue ?? "");
            var model = new ProductSearchResult()
            {
                Page = condition.Page,
                PageSize = condition.PageSize,
                SearchValue = condition.SearchValue ?? "",
                RowCount = rowCount,
                Data = data
            };
            ApplicationContext.SetSessionData(PRODUCT_SEARCH_CONDITION, condition);
            return View(model);
        }
        private List<CartItem> GetShoppingCart()
        {
            var shoppingCart = ApplicationContext.GetSessionData<List<CartItem>>(SHOPPING_CART);
            if(shoppingCart == null)
            {
                shoppingCart = new List<CartItem>();
                ApplicationContext.SetSessionData(SHOPPING_CART, shoppingCart);
            }
            return shoppingCart;
        }
        public IActionResult AddToCart(CartItem item)
        {
            if (item.SalePrice < 0 || item.Quantity <= 0)
                return Json("Giá bán không hợp lệ");
           var shoppingCart = GetShoppingCart();
           var existsProduct = shoppingCart.FirstOrDefault(m => m.ProductID == item.ProductID);
            if (existsProduct == null)
            {
                shoppingCart.Add(item);
            }
            else
            {
                existsProduct.Quantity += item.Quantity;
                existsProduct.SalePrice = item.SalePrice;
            }
            ApplicationContext.SetSessionData(SHOPPING_CART,shoppingCart);
            return Json("");
        }
        public IActionResult RemoveFromCart(int id = 0)
        { 
            var shoppingCart = GetShoppingCart();
            int index = shoppingCart.FindIndex(m => m.ProductID == id);
            if (index >= 0)
            {
                shoppingCart.RemoveAt(index);
            }
            ApplicationContext.SetSessionData(SHOPPING_CART, shoppingCart);
            return Json("");
        }
        public IActionResult ClearCart()
        {
            var shoppingCart = GetShoppingCart();
            shoppingCart.Clear();
            ApplicationContext.SetSessionData(SHOPPING_CART, shoppingCart);
            return Json("");
        }
        public IActionResult Details(int id = 0)
        {
            var order = OrderDataService.GetOrder(id);
            if (order == null)
                return RedirectToAction("Index");
            var details = OrderDataService.ListOrderDetails(id);
            var model = new OrderDetailModel() { Order = order, Details = details };
            return View(model);
        }
        public IActionResult ShoppingCart()
        {
            return View(GetShoppingCart());
        }
        public IActionResult Init(int customerID = 0, string deliveryProvince ="", string deliveryAddress="")
        {
            var shoppingCart = GetShoppingCart();
            if (shoppingCart.Count == 0)
                return Json("Giỏ hàng trống. Vui lòng chọn mặt hàng cần bán");
            if (customerID == 0 || String.IsNullOrWhiteSpace(deliveryProvince) || String.IsNullOrWhiteSpace(deliveryAddress))
                return Json("Vui lòng nhập đầy đủ thông tin khách hàng và nơi giao hàng");
            int employeeID = Convert.ToInt32(User.GetUserData().UserId);
            List<OrderDetail> orderDetails = new List<OrderDetail>();
            foreach (var item in shoppingCart)
            {
                orderDetails.Add(new OrderDetail()
                {
                    ProductID = item.ProductID,
                    Quantity = item.Quantity,
                    SalePrice = item.SalePrice
                });
            }
            int orderID = OrderDataService.InitOrder(employeeID, customerID, deliveryProvince, deliveryAddress, orderDetails);
            ClearCart();
            return Json(orderID);

        }
        public IActionResult EDitDetail(int id = 0, int productId = 0)
        {
            var model = OrderDataService.GetOrderDetail(id, productId);

            return View(model);
        }
        [HttpPost]
        public IActionResult UpdateDetail(int id = 0, int productId = 0, int Quantity = 0, String SalePrice="")
        {

            if( Quantity <= 0) 
            {
                return Json("Số lượng không hợp lý");
            }
            if (string.IsNullOrWhiteSpace(SalePrice))
                return Json("Giá bán không hợp lý");
            decimal _SalePrice = Convert.ToDecimal(SalePrice);
            if (_SalePrice < 0)
                return Json("Giá bán không hợp lý");
            bool result = OrderDataService.SaveOrderDetail(id, productId, Quantity, _SalePrice);
            if(result == false)
            {
                return Json("Cập nhật chi tiết đơn hàng không thành công!");
            }

            return Json("");
        }
        public IActionResult Accept(int id = 0)
        {
            bool result = OrderDataService.AcceptOrder(id);
            if (result == false)
            {
                ModelState.AddModelError("error", "Không thể duyệt đơn hàng!");
                return View("Detail", new {id});
            }
            return RedirectToAction("Details", new { id });
        }

        [HttpGet]
        public IActionResult Shipping(int id = 0)
        {
            ViewBag.OrderID = id;
             return View();
        }
        [HttpPost]
        public IActionResult Shipping(int id = 0, int shipperID = 0)
        {
            if (shipperID <= 0)
            {
                return Json("Vui lòng chọn người giao hàng");
            }
            bool result = OrderDataService.ShipOrder(id, shipperID);
            if (!result)
            {
                return Json("Chuyển đơn hàng cho giao hàng không thành công");
            }
            return Json("");
        }
        public IActionResult Finish(int id = 0)
        {
            bool result = OrderDataService.FinishOrder(id);
            if (!result)
            {
                ModelState.AddModelError("error", "Không thể cập nhật trạng thái hoàn tất cho đơn hàng!");
            }
            return RedirectToAction("Details", new { id });
        }
        public IActionResult Cancel(int id = 0)
        {
            bool result = OrderDataService.CancelOrder(id);
            if (!result)
            {
                ModelState.AddModelError("error", "Không thể hủy đơn hàng");
                return RedirectToAction("Details", new { id });
            }
            return RedirectToAction("Details", new { id });
        }

        public IActionResult Reject(int id = 0)
        {
            bool result = OrderDataService.RejectOrder(id);
            if (!result)
            {
                ModelState.AddModelError("error", "Không thể từ chối đơn hàng!");
                return RedirectToAction("Details", new { id });
            }
            return RedirectToAction("Details", new { id });
        }
        public IActionResult Delete(int id)
        {
            bool result = OrderDataService.DeleteOrder(id);
            if (!result)
            {
                ModelState.AddModelError("error", "Không thể xóa đơn hàng!");
                return RedirectToAction("Details", new { id });
            }
            return RedirectToAction("Index");
        }
        public IActionResult DeleteDetail(int orderID = 0, int productID = 0)
        {
            bool result = OrderDataService.DeleteOrderDetail(orderID, productID);
            if (result == false)
            {
                ModelState.AddModelError("error", "Không thể xóa mặt hàng!");
                return RedirectToAction("Details", new { orderID });
            }
            return RedirectToAction("Details", new { id = orderID });
        }
    }
}
