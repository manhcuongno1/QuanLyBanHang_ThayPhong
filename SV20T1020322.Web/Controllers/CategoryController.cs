﻿using SV20T1020322.BusinessLayers;
using Microsoft.AspNetCore.Mvc;
using SV20T1020322.DomainModels;
using SV20T1020322.Web.AppCodes;
using SV20T1020322.Web.Models;
using Microsoft.AspNetCore.Authorization;

namespace SV20T1020322.Web.Controllers
{
    [Authorize(Roles = $"{WebUserRoles.Administrator},{WebUserRoles.Employee}")]
    public class CategoryController : Controller
    {
        const int PAGE_SIZE = 20;
        const string CREATE_TITLE = " nhập loại hàng mới";

        const string CATEGORY_SEARCH = "category_search";//session dùng để lưu lại điều kiện tìm kiếm
        public IActionResult Index()
        {
            Models.PaginationSearchInput? input = ApplicationContext.GetSessionData<PaginationSearchInput>(CATEGORY_SEARCH);
            if (input == null)
            {
                input = new PaginationSearchInput()
                {
                    Page = 1,
                    PageSize = PAGE_SIZE,
                    SearchValue = ""
                };
            }
            return View(input);
        }
        public IActionResult Search(PaginationSearchInput input)
        {
            int rowCount = 0;
            var data = CommonDataService.ListOfCategories(out rowCount, input.Page, input.PageSize, input.SearchValue ?? "");
            var model = new CategorySearchResult()
            {
                Page = input.Page,
                PageSize = input.PageSize,
                SearchValue = input.SearchValue ?? "",
                RowCount = rowCount,
                Data = data

            };

            ApplicationContext.SetSessionData(CATEGORY_SEARCH, input);
            return View(model);
        }

        public IActionResult Create()
        {
            ViewBag.Title = "Bổ sung loại hàng";
            var model = new Category()
            {
                CategoryID = 0
            };

            return View("Edit", model);
        }

        public IActionResult Edit(int id = 0)
        {
            ViewBag.Title = "Cập nhật thông tin loại hàng";
            var model = CommonDataService.GetCategory(id);
            if (model == null)
                return RedirectToAction("Index");
            return View(model);
        }

        [HttpPost] // Attribute chỉ nhận dữ liệu gửi lên dạng POST
        public IActionResult Save(Category model)
        {
            if (string.IsNullOrWhiteSpace(model.CategoryName))
                ModelState.AddModelError("CategoryName", "Tên không được để trống");
            if (string.IsNullOrWhiteSpace(model.Description))
                ModelState.AddModelError("Description", "Mô tả không được để trống");
            if (!ModelState.IsValid)
            {
                ViewBag.Title = model.CategoryID == 0 ? "Bổ sung loại hàng" : "Cập nhật thông tin loại hàng";
                return View("Edit", model);
            }
            if (model.CategoryID == 0)
            {
                int id = CommonDataService.AddCategory(model);
            }
            else
            {
                bool result = CommonDataService.UppdateCategory(model);
            }
            return RedirectToAction("Index");
        }
        public IActionResult Delete(int id = 0)
        {
            if (Request.Method == "POST")
            {
                bool result = CommonDataService.DeleteCategory(id);
                return RedirectToAction("Index");
            }
            var model = CommonDataService.GetCategory(id);
            if (model == null)
                return RedirectToAction("Index");

            return View(model);
        }
    }
}
