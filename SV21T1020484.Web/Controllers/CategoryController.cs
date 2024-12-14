using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SV21T1020484.BusinessLayers;
using SV21T1020484.DomainModels;
using SV21T1020484.Web.Models;

namespace SV21T1020484.Web.Controllers
{
    [Authorize(Roles = $"{WebUserRoles.EMPLOYEE}, {WebUserRoles.ADMINISTRATOR}")]
    public class CategoryController : Controller
    {
        private const int PAGE_SIZE = 10;
        private const String CATEGORY_SEARCH_CONDITION = "CategorySearchCondition";
        public IActionResult Index()
        {
            PaginationSearchInput? condition = ApplicationContext.GetSessionData<PaginationSearchInput>(CATEGORY_SEARCH_CONDITION);
            if(condition == null )
            {
                condition = new PaginationSearchInput
                {
                    Page = 1,
                    PageSize = PAGE_SIZE,
                    SearchValue = ""
                };
            }
            return View(condition);
        }
        public IActionResult Search(PaginationSearchInput condition)
        {
            int rowCount;
            var data = CommonDataService.ListOfCategories(out rowCount, condition.Page, condition.PageSize, condition.SearchValue??"" +"");
            CategorySearchResult model = new CategorySearchResult()
            {
                Page = condition.Page,
                PageSize = condition.PageSize,
                SearchValue = condition.SearchValue,
                RowCount = rowCount,
                Data = data
            };
            ApplicationContext.SetSessionData(CATEGORY_SEARCH_CONDITION, condition);
            return View(model);
        }
        public IActionResult Create()
        {
            ViewBag.Title = "Bổ sung loại hàng";
            var data = new Category
            {
                CategoryID = 0
            };
            return View("Edit",data);
        }
        public IActionResult Edit(int id = 0)
        {
            ViewBag.Title = "Cập nhật loại hàng";
            var data = CommonDataService.GetCategory(id);
            if(data == null) { return RedirectToAction("Index"); }
            return View(data);
        }
        public IActionResult Delete(int id = 0)
        {
            ViewBag.Title = "Xóa loại hàng";
           
            if(Request.Method == "POST")
            {
                CommonDataService.DeleteCategory(id);
                return RedirectToAction("Index");
            } 
            var data = CommonDataService.GetCategory(id);
            if(data == null) { return RedirectToAction("Index"); }
            return View(data);
        }
        [HttpPost]
        public IActionResult Save(Category data)
        {
            ViewBag.Title = data.CategoryID == 0 ? "Bổ sung mặt hàng" : "Cập nhật mặt hàng";
            if(string.IsNullOrEmpty(data.CategoryName))
                ModelState.AddModelError(nameof(data.CategoryName), "Tên mặt hàng không được để trống");
            if (string.IsNullOrEmpty(data.Description))
                ModelState.AddModelError(nameof(data.Description), "nhập mô tả cho mặt hàng");
            if (ModelState.IsValid == false)
            {
                return View("Edit", data);
            }
            try
            {
                if (data.CategoryID == 0)
                {
                    int id = CommonDataService.AddCategory(data);
                    if(id <= 0)
                    {
                        ModelState.AddModelError(nameof(data.CategoryID), "Mặt hàng đã tồn tại");
                        return View("Edit", data);
                    }
                }
                else
                {
                    bool result = CommonDataService.UpdateCategory(data);
                    if (result == false)
                    {
                        ModelState.AddModelError(nameof(data.CategoryID), "Mặt hàng bị trùng");
                        return View("Edit", data);
                    }
                }
                return RedirectToAction("Index");
            }catch(Exception ex)
            {
                ModelState.AddModelError("error", "Hệ thống bị gián đoạn");
                return View("Edit", data);
            }
        }
    }
}
