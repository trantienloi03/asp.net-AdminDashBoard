using Azure.Identity;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SV21T1020484.BusinessLayers;
using SV21T1020484.DomainModels;
using SV21T1020484.Shop.Models;
using System.ComponentModel.DataAnnotations;

namespace SV21T1020484.Shop.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {
        [HttpGet]
        [AllowAnonymous]
        public IActionResult Login()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [AllowAnonymous]
        public async Task<IActionResult> Login(string username, string password)
        {
            ViewBag.Username = username;
            //kiem tra thong tin ddau vao
            if (string.IsNullOrEmpty(password) || string.IsNullOrEmpty(username))
            {
                ModelState.AddModelError("Error", "Nhập tên và mật khẩu");
                return View();
            }
            var userAccount = UserAccountService.Authorize(UserAccountService.UserTypes.Customer, username, password);
            if (userAccount == null)
            {
                ModelState.AddModelError("Error", "Dang nhap that bai");
                return View();
            }
            string customerID = userAccount.UserId;
            int cartItemCount = 0;
            if(SV21T1020484.BusinessLayers.CartDataService.getCartByCustomerID(Convert.ToInt32(customerID)) != null)
                cartItemCount =  SV21T1020484.BusinessLayers.CartDataService.getCartByCustomerID(Convert.ToInt32(customerID)).Sum ;
            HttpContext.Session.SetInt32("CartItemCount", cartItemCount);
         
            // Dang nhap thanh cong:
            var userData = new WebUserData
            {
                UserId = userAccount.UserId,
                UserName = userAccount.UserName,
                DisplayName = userAccount.DisplayName,
                Roles = userAccount.Role.Split(",").ToList(),
                
            };

            //
            await HttpContext.SignInAsync(userData.CreatePrincipal());
            return RedirectToAction("Index", "Home");

        }
        public async Task<IActionResult> Logout()
        {
            HttpContext.Session.Clear();
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login");
        }
        public IActionResult Cart()
        {
            var user = User.GetUserData();
            int userID = Convert.ToInt32(user.UserId);
            var model = SV21T1020484.BusinessLayers.CartDataService.listViewCart(userID);
            return View(model);
        }
        [HttpGet]
        public IActionResult EditProfile(int CustomerID)
        {
            var customer = SV21T1020484.BusinessLayers.CommonDataService.GetCustomer(CustomerID);

            return View(customer);
        }
        [HttpPost]
        public IActionResult SaveEditProfile(Customer data)
        {

            if (string.IsNullOrEmpty(data.ContactName))
                ModelState.AddModelError(nameof(data.ContactName), "Tên giao dịch không được để trống");
            if (string.IsNullOrEmpty(data.Phone))
                ModelState.AddModelError(nameof(data.Phone), "Số điện thoại không được để trống");
            if (string.IsNullOrEmpty(data.Address))
                ModelState.AddModelError(nameof(data.Address), "Vui lòng nhập địa chỉ");
            if (string.IsNullOrEmpty(data.Province))
                ModelState.AddModelError(nameof(data.Province), "Hãy chọn tỉnh thành");
            // IsValid cua ModelState de ktra ton tai loi
            if (ModelState.IsValid == false)
            {
                return View("EditProfile", data.CustomerID);
            }
            var customer = SV21T1020484.BusinessLayers.CommonDataService.GetCustomer(data.CustomerID);
            customer.ContactName = data.ContactName;
            customer.Province = data.Province;
            customer.Address = data.Address;
            customer.Phone = data.Phone;
            bool result = CommonDataService.UpdateCustomer(customer);
            if (result == false)
            {
                ModelState.AddModelError(nameof(data.Email), "Email bị trùng");
                return View("EditProfile", data.CustomerID);
            }


            return Json(new { success = true, message = "Cập nhật thành công!" });
            //return Json("");
        }
        public IActionResult History()
        {
            var user = User.GetUserData();
            var listOrder = OrderDataService.ListOrderByCustomerID(Convert.ToInt32(user.UserId));

            return View(listOrder);
        }
        public IActionResult EditOrder(int id)
        {
            var order = OrderDataService.GetOrder(id);
            if (order == null)
                return RedirectToAction("History");
            var details = OrderDataService.ListOrderDetails(id);
            var model = new OrderDetailModel() { Order = order, Details = details };

            return View(model);
        }
        public IActionResult DeleteOrderDetail(int orderID = 0, int productID = 0)
        {
            bool result = OrderDataService.DeleteOrderDetail(orderID, productID);
            if (result == false)
            {
                ModelState.AddModelError("error", "Không thể xóa mặt hàng!");
                return RedirectToAction("EditOrder", new { id = orderID });
            }
            return RedirectToAction("EditOrder", new { id = orderID });
        }
        [AllowAnonymous]
        public IActionResult Register()
        {
            CustomerDTO customer = new CustomerDTO();
            customer.CustomerID = 0;
            customer.IsLocked = false;
            return View(customer);
        }
        [HttpPost]
        [AllowAnonymous]
        public IActionResult Save(CustomerDTO data)
        {
            //CustomerDTO customer = new CustomerDTO();
            //customer.CustomerName = data.CustomerName;
            //customer.CustomerID = data.CustomerID; 
            //customer.IsLocked = data.IsLocked;
            //customer.Email = data.Email;
            //customer.password = data.password;
            //customer.ContactName = data.ContactName;
            
           int id = CommonDataService.AddCustomerDTO(data);
           if (id <= 0)
           {
              ModelState.AddModelError("Error", "Tài khoản đã tồn tại");
              return View("Register", data);
           }

            return RedirectToAction("Login");
        }
        public IActionResult EditCartView(int cartDetailID, int productID, int quantity)
        {
            bool result = CartDataService.SaveCartdetail(cartDetailID, productID, quantity);
            return RedirectToAction("Cart");
        }
        public IActionResult DeleteCartDetail(int cartID, int cartDetailID)
        {
            var user = User.GetUserData();
            int userID = Convert.ToInt32(user.UserId);
            var lstDetails = SV21T1020484.BusinessLayers.CartDataService.listViewCart(userID);
            int cartItemCount = lstDetails.Count;
            bool result = CartDataService.DeleteDetail(cartID, cartDetailID);
            if (result)
            {
                cartItemCount--;
            }
            HttpContext.Session.SetInt32("CartItemCount", cartItemCount);
            return RedirectToAction("Cart");
        }
        public IActionResult Checkout(string FullName, string Address, string Province)
        {

            var user = User.GetUserData();
            int userID = Convert.ToInt32(user.UserId);
            var lstDetails = SV21T1020484.BusinessLayers.CartDataService.listViewCart(userID);
            int cartItemCount = lstDetails.Count;
            List<OrderDetail> orderDetails = new List<OrderDetail>();
            foreach (var item in lstDetails)
            {
                orderDetails.Add(new OrderDetail()
                {
                    ProductID = item.ProductID,
                    Quantity = item.Quantity,
                    SalePrice = item.Price
                });
                bool result = CartDataService.DeleteDetail(item.CartID, item.CartDetailID);
                if (result)
                {
                    cartItemCount--; 
                }
            }
            HttpContext.Session.SetInt32("CartItemCount", cartItemCount);
            int orderID = OrderDataService.KhachHangDatHang(userID,Address, Province, orderDetails);
            //int orderID = OrderDataService.InitOrder(1, userID, Address, Province, orderDetails);
            if(orderID == 0)
            {
                return RedirectToAction("Cart");
            }
            return RedirectToAction("History");
        }
        public IActionResult Cancel(int id = 0)
        {
            bool result = OrderDataService.CancelOrder(id);
            if (!result)
            {
                ModelState.AddModelError("error", "Không thể hủy đơn hàng");
                return RedirectToAction("History");
            }
            return RedirectToAction("History");
        }
        public IActionResult ChangePassword(string userName, string oldPassword, string newPassword, string confirmPassword)
        {
            if (Request.Method == "POST")
            {
                if (confirmPassword.Trim().Equals(newPassword.Trim()) == false)
                    ModelState.AddModelError("confirmPass", "Xác nhận lại mật khẩu sai");
                if (ModelState.IsValid == false)
                {
                    return View();
                }
                else
                {
                    var result = UserAccountService.ChangePass(UserAccountService.UserTypes.Customer, userName, oldPassword, newPassword);
                    if (result == true)
                    {
                        return RedirectToAction("Logout");
                    }
                    else
                    {
                        ModelState.AddModelError("oldPass", "Mật khẩu cũ không đúng");
                        return View();
                    }
                }
            }
            return View();
        }
        public IActionResult AccessDenined()
        {
            return View();
        }
    }
}
