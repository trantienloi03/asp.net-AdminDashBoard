using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SV21T1020484.BusinessLayers;
using SV21T1020484.DomainModels;
using SV21T1020484.Web.Models;

namespace SV21T1020484.Web.Controllers
{
    [Authorize(Roles = $"{WebUserRoles.ADMINISTRATOR}")]
    public class EmployeeController : Controller
    {
        private const int PAGE_SIZE = 12;
        private const string Employee_SEARCH_CONDITION = "EmployeeSearchCondition";
        public IActionResult Index()
        {
            PaginationSearchInput? condition = ApplicationContext.GetSessionData<PaginationSearchInput>(Employee_SEARCH_CONDITION);
            if (condition == null)
                condition = new PaginationSearchInput
                {
                    Page = 1,
                    PageSize = PAGE_SIZE,
                    SearchValue = ""
                };
            return View(condition);
        }
        public IActionResult Search(PaginationSearchInput condition)
        {
            int rowCount;
            var data = CommonDataService.ListOfEmployees(out rowCount, condition.Page, condition.PageSize, condition.SearchValue ?? "");
            EmployeeSearchResult model = new EmployeeSearchResult()
            {
                Page = condition.Page,
                PageSize = condition.PageSize,
                SearchValue = condition.SearchValue ?? "",
                RowCount = rowCount,
                Data = data
            };
            ApplicationContext.SetSessionData(Employee_SEARCH_CONDITION, condition);
            return View(model);
        }
        public IActionResult Create()
        {
            ViewBag.Title = "Bổ sung nhân viên";
            var data = new Employee()
            {
                EmployeeID = 0, IsWorking = true, Photo = "nophoto.png"
            };
            return View("Edit",data);
        }
        public IActionResult Edit(int id = 0)
        {
            ViewBag.Title = "Cập nhật nhân viên";
            var data = CommonDataService.GetEmployee(id);
            if(data == null)
            {
                return RedirectToAction("Index");
            }
            return View(data);
        }
        public IActionResult Delete(int id = 0)
        {
            var data = CommonDataService.GetEmployee(id);
            if (Request.Method == "POST")
            {
                CommonDataService.DeleteEmployee(id);
                return RedirectToAction("Index");
            }
           if(data == null)
            {
                return RedirectToAction("Index");
            }
            return View(data);
        }
        [HttpPost]
        public IActionResult Save(Employee data, string _BirthDate, IFormFile? _Photo)
        {
            ViewBag.Title = data.EmployeeID == 0 ? "Bổ sung nhân viên" : "Cập nhật nhân viên";
            // xw li du lieu
            if (string.IsNullOrEmpty(data.Role))
            {
                ModelState.AddModelError(nameof(data.Role), "Vui lòng chọn ít nhất một vai trò.");
            }
            else
            {
                // Xử lý Role nếu cần (tách các vai trò)
                var roles = data.Role.Split(',', StringSplitOptions.RemoveEmptyEntries);
                if (!roles.All(r => r == "admin" || r == "employee"))
                {
                    ModelState.AddModelError(nameof(data.Role), "Vai trò không hợp lệ.");
                }
            }
            DateTime? date = _BirthDate.ToDateTime();
  //          if (String.IsNullOrEmpty(_BirthDate))
   //             ModelState.AddModelError("_BirthDate", "Vui lòng nhập ngày sinh");
     //       if (_Photo == null)
       //         ModelState.AddModelError("_Photo", "Vui lòng chọn ảnh");
            if (data.BirthDate == null)
                ModelState.AddModelError(nameof(data.BirthDate), "Vui lòng nhập ngày sinh");
            if(data.Photo == null)
                ModelState.AddModelError(nameof(data.Photo), "Vui lòng chọn ảnh");
            if(String.IsNullOrEmpty(data.FullName))
                ModelState.AddModelError(nameof(data.FullName), "Tên nhân viên không được để trống");
            if (String.IsNullOrEmpty(data.Email))
                ModelState.AddModelError(nameof(data.Email), "Email nhân viên không được để trống");
            if (String.IsNullOrEmpty(data.Address))
                ModelState.AddModelError(nameof(data.Address), "Vui lòng nhập địa chỉ của nhân viên");
            if (String.IsNullOrEmpty(data.Phone))
                ModelState.AddModelError(nameof(data.Phone), "Vui lòng nhập số điện thoại");
            if (ModelState.IsValid == false)
            {
                return View("Edit", data);
            }
            if (date.HasValue)
            {
                // data.BirthDate = _BirthDate;
                data.BirthDate = date.Value;
            }
            if (_Photo != null)
            {
                string fileName = $"{DateTime.Now.Ticks}-{_Photo.FileName}";
                string filePath = Path.Combine(ApplicationContext.WebRootPath, @"images\employees", fileName);
                using(var stream = new FileStream(filePath, FileMode.Create))
                {
                    _Photo.CopyTo(stream);
                }
                data.Photo = fileName;
            }
            try
            {
                if (data.EmployeeID == 0)
                {
                    int id = CommonDataService.AddEmployee(data);
                    if (id <= 0)
                    {
                        ModelState.AddModelError("existed", "Email bị trùng");
                        return View("Edit", data);
                    }
                }
                else
                {
                    bool result = CommonDataService.UpdateEmployee(data);
                    if (result == false)
                    {
                        ModelState.AddModelError("existed", "Email bị trùng");
                        return View("Edit", data);
                    }
                }
                return RedirectToAction("Index");
            }catch (Exception ex)
            {
                ModelState.AddModelError("error", "Hệ thống bị gián đoạn");
                return View("Edit", data);
            }
        }
    }
}
