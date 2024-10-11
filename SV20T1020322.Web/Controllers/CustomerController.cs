﻿using SV20T1020322.BusinessLayers;
using Microsoft.AspNetCore.Mvc;
using SV20T1020322.DomainModels;
using SV20T1020322.Web.AppCodes;
using SV20T1020322.Web.Models;
using Microsoft.AspNetCore.Authorization;

namespace SV20T1020322.Web.Controllers
{
    [Authorize(Roles = $"{WebUserRoles.Administrator},{WebUserRoles.Employee}")]
    public class CustomerController : Controller
    {
        const int PAGE_SIZE = 25;
        const string CUSTOMER_SEARCH = "customer_search";//Tên biến session dùng để lưu lại điều kiện tìm kiếm
        public IActionResult Index()
        {
            //Kiểm tra xem trong session có lưu điều kiện tìm kiếm không.
            //Nếu có thì sử dụng lại điều kiện tìm kiếm,ngược lại tìm kiếm theo điều kiện mặc định
            Models.PaginationSearchInput? input = ApplicationContext.GetSessionData<PaginationSearchInput>(CUSTOMER_SEARCH);
            if(input == null ) 
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
            var data = CommonDataService.ListOfCustomers(out rowCount,input.Page,input.PageSize,input.SearchValue ?? "");
            var model = new CustomerSearchResult()
            {
                Page = input.Page,
                PageSize = input.PageSize,
                SearchValue = input.SearchValue ?? "",
                RowCount = rowCount,
                Data = data
            };
            //lưu lại điều kiện tìm kiếm
            ApplicationContext.SetSessionData(CUSTOMER_SEARCH, input);
            return View(model);
        }

        public IActionResult Create()
        {
            ViewBag.Title = "Bổ sung khách hàng";
            var model = new Customer()
            { 
                CustomerId = 0
            };

            return View("Edit",model);
        }

        public IActionResult Edit(int id = 0)
        {
            ViewBag.Title = "Cập nhật thông tin khách hàng";
            var model = CommonDataService.GetCustomer(id);
            if(model == null)
                return RedirectToAction("Index");
            return View(model);
        }
        [HttpPost] // Attribute chỉ nhận dữ liệu gửi lên dạng POST
        public IActionResult Save(Customer model) 
        {
            //Tên khách hàng,tên giao dịch,email,tỉnh không được để trống
            if (string.IsNullOrWhiteSpace(model.CustomerName))
                ModelState.AddModelError("CustomerName", "Tên không được để trống");
            if (string.IsNullOrWhiteSpace(model.ContactName))
                ModelState.AddModelError("ContactName", "Tên giao dịch không được để trống");
            if (string.IsNullOrWhiteSpace(model.Email))
                ModelState.AddModelError("Email", "Tên Email không được để trống");
            if (string.IsNullOrWhiteSpace(model.Province))
                ModelState.AddModelError("Province", "Vui lòng chọn tỉnh thành");

            if(!ModelState.IsValid) 
            {
                ViewBag.Title = model.CustomerId == 0 ? "Bổ sung khách hàng" : "Cập nhật thông tin khách hàng";
                return View("Edit",model);
            }
            if (model.CustomerId == 0)
            {
                int id = CommonDataService.AddCustomer(model);
                if(id <= 0 )
                {
                    ModelState.AddModelError("Email", "Email bị trùng");
                    ViewBag.Title = "Bổ sung khách hàng";
                    return View("Edit", model);
                }    
            }
            else
            {
                bool result = CommonDataService.UppdateCustomer(model);
                if (!result)
                {
                    ModelState.AddModelError("Error", "Không cập nhật được nhân viên. Có thể email bị trùng");
                    return View("Edit", model);
                }
            }
            return RedirectToAction("Index");
        }
        public IActionResult Delete(int id = 0)
        {
            if (Request.Method == "POST") 
            {
                bool result = CommonDataService.DeleteCustomer(id);
                return RedirectToAction("Index");
            }
            var model = CommonDataService.GetCustomer(id);
            if (model == null)
                return RedirectToAction("Index");

            return View(model);
        }
    }
}
