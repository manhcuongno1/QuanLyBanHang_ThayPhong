﻿using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Mvc;
using SV20T1020322.BusinessLayers;
using SV20T1020322.DomainModels;
using SV20T1020322.Web.AppCodes;
using SV20T1020322.Web.Models;
using SV20T1020322.Web;
using System;
using System.Reflection;
using Microsoft.AspNetCore.Authorization;

namespace SV20T1020322.Web.Controllers
{
    [Authorize(Roles = $"{WebUserRoles.Administrator},{WebUserRoles.Employee}")]
    public class ProductController : Controller
    {
        const int PAGE_SIZE = 25;
        const string CREATE_TITLE = " nhập mặt hàng mới";
        const string PRODUCT_SEARCH = "product_search";//session dùng để lưu lại điều kiện tìm kiếm
        public IActionResult Index()
        {
            Models.ProductSearchInput? input = ApplicationContext.GetSessionData<ProductSearchInput>(PRODUCT_SEARCH);

            if (input == null)
            {
                input = new ProductSearchInput
                {
                    Page = 1,
                    PageSize = PAGE_SIZE,
                    SearchValue = "",
                    CategoryID = 0,
                    SupplierID = 0,
                };
            }
            return View(input);
        }
        public IActionResult Search(ProductSearchInput input)
        {
            int rowCount = 0;
            var data = ProductDataService.ListProducts(out rowCount, input.Page, input.PageSize, input.SearchValue ?? "", input.CategoryID,
                                            input.SupplierID, input.MinPrice, input.MaxPrice);

            var model = new ProductSearchResult()
            {
                Page = input.Page,
                PageSize = input.PageSize,
                SearchValue = input.SearchValue ?? "",
                RowCount = rowCount,
                Data = data
            };

            // Lưu lại vào session điều kiện tìm kiếm
            ApplicationContext.SetSessionData(PRODUCT_SEARCH, input);

            return View(model);
        }


        public IActionResult Create()
        {
            ViewBag.Title = "Bổ sung mặt hàng";
            ViewBag.IsEdit = false;
            var model = new Product()
            {
                ProductID = 0,
                Photo = "nophoto.jpg",
            };
            return View("Edit", model);
        }

        public IActionResult Edit(int id = 0)
        {
            ViewBag.Title = "Sửa mặt hàng";
            ViewBag.IsEdit = true;
            var model = ProductDataService.GetProduct(id);
            if (model == null)
            {
                return RedirectToAction("Index");
            }
            if (string.IsNullOrWhiteSpace(model.Photo))
            {
                model.Photo = "nophoto.jpg";
            }

            return View(model);
        }
        public IActionResult Save(Product model, IFormFile? uploadPhoto)
        {
            if (string.IsNullOrWhiteSpace(model.Photo))
            {
                model.Photo = "nophoto.jpg";
            }
            if (string.IsNullOrWhiteSpace(model.ProductName))
            {
                ModelState.AddModelError(nameof(model.ProductName), "Tên không được để trống");
            }
            if (model.CategoryID.ToString() == "0")
            {
                ModelState.AddModelError(nameof(model.CategoryID), "Loại sản phẩm không được để trống");
            }
            if (string.IsNullOrWhiteSpace(model.Unit))
            {
                ModelState.AddModelError(nameof(model.Unit), "Đơn vị tính không được để trống");
            }
            if (model.Price == 0)
            {
                ModelState.AddModelError(nameof(model.Price), "Giá của mặt hàng không được để trống");
            }
            if (model.SupplierID.ToString() == "0")
            {
                ModelState.AddModelError(nameof(model.SupplierID), "Nhà cung cấp không được để trống");
            }
            List<Product> list
                = ProductDataService.ListProducts("");
            foreach (Product item in list)
            {
                if (model.ProductName == item.ProductName && model.ProductID != item.ProductID)
                {
                    ModelState.AddModelError(nameof(model.ProductName), $"Tên sản phẩm '{model.ProductName}' đã tồn tại.");
                    break;
                }
            }
            if (!ModelState.IsValid)
            {
                ViewBag.IsEdit = model.ProductID == 0 ? false : true;
                ViewBag.Title = model.ProductID == 0 ? "Bổ sung khách hàng" : "Cập nhật thông tin khách hàng";
                return View("Edit", model);
            }
            // xử lý ảnh upload:
            // Nếu có ảnh được upload thì lưu ảnh lên server, gán tên file ảnh đã lưu cho model.Photo
            if (uploadPhoto != null)
            {
                //string fileName = $"{DateTime.Now:yyyy-MM-dd-HH-mm-ff}_{uploadPhoto.FileName}";
                string fileName = $"{DateTime.Now.Ticks}_{uploadPhoto.FileName}"; // Tên file sẽ lưu trên server
                                                                                  // đường dẫn đến file lưu trên server (ví dụ: D:\MyWeb\wwwroot\images\photo.png)
                                                                                  // string filePath = Path.Combine(@"D:\laptrinhweb\SV20T1020390\SV20T1020390.web\wwwroot\images\employees", fileName);
                string filePath = Path.Combine(ApplicationContext.HostEnviroment.WebRootPath, @"images\products", fileName);

                // lưu file lên Server
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    uploadPhoto.CopyTo(stream);
                }

                // Gán tên file ảnh cho model.Photo
                model.Photo = fileName;
            }
            if (model.ProductID == 0)
            {
                int id = ProductDataService.AddProduct(model);
                if (id <= 0)
                {
                    ModelState.AddModelError(nameof(model.ProductName), "Ten sản phẩm bị trùng ");
                    ViewBag.Title = "Bổ sung mặt hàng";
                    return View("Edit", model);
                }
            }
            else
            {
                bool result = ProductDataService.UpdateProduct(model);
            }
            return RedirectToAction("Index");
        }
        public IActionResult Delete(int id = 0)
        {

            if (Request.Method == "POST")
            {
                bool result = ProductDataService.DeleteProduct(id);
                return RedirectToAction("Index");
            }
            var model = ProductDataService.GetProduct(id);
            if (model == null)
                return RedirectToAction("Index");
            return View(model);
        }

        public IActionResult Photo(int id = 0, string method = "", int photoId = 0)
        {
            ProductPhoto model = null;
            switch (method)
            {
                case "add":
                    ViewBag.Title = "Bổ sung ảnh";
                    model = new ProductPhoto()
                    {
                        PhotoID = 0,
                        ProductID = id,
                        Photo = "nophoto.jpg",
                    };
                    return View(model);

                case "edit":
                    ViewBag.Title = "Thay đổi ảnh";
                    if (photoId < 0)
                    {
                        return RedirectToAction("Edit");
                    }
                    model = ProductDataService.GetPhoto(photoId);

                    if (model == null)
                    {
                        return RedirectToAction("Index");
                    }
                    return View(model);
                case "delete":
                    ProductDataService.DeletePhoto(photoId);
                    return RedirectToAction("Edit", new { id = id });
                default:
                    return RedirectToAction("Index");
            }

        }
        [HttpPost]
        public ActionResult SavePhoto(ProductPhoto model, IFormFile? uploadPhoto = null)
        {
            if (string.IsNullOrWhiteSpace(model.Description))
            {
                ModelState.AddModelError(nameof(model.Description), "Mô tả ảnh mặt hàng không được để trống");
            }
            if (model.DisplayOrder == 0)
            {
                ModelState.AddModelError("DisplayOrder", "Thứ tự hiển thị hình ảnh không được để trống");
            }

            else if (model.DisplayOrder < 1)
            {
                ModelState.AddModelError("DisplayOrder", "Thứ tự hiển thị hình ảnh phải là một số tự nhiên dương");
            }
            List<ProductPhoto> productPhotos = ProductDataService.ListPhotos(model.ProductID);
            bool isUsedDisplayOrder = false;
            foreach (ProductPhoto item in productPhotos)
            {
                if (item.DisplayOrder == model.DisplayOrder && model.PhotoID != item.PhotoID)
                {
                    isUsedDisplayOrder = true;
                    break;
                }
            }
            if (isUsedDisplayOrder)
            {
                ModelState.AddModelError("DisplayOrder",
                    $"Thứ tự hiển thị {model.DisplayOrder} của hình ảnh đã được sử dụng trước đó");
            }


            model.IsHidden = Convert.ToBoolean(model.IsHidden.ToString());
            // xử lý nghiệp vụ upload file
            if (uploadPhoto != null)
            {
                //Tên file sẽ lưu trên server
                /* string fileName = $"{DateTime.Now.Ticks}_{uploadPhoto.FileName}"; /*//*/Tên file sẽ lưu trên server*/

                string fileName = $"{model.ProductID}_{uploadPhoto.FileName}";                                                                //Đường dẫn đến file sẽ lưu trên server 
                string filePath = Path.Combine(ApplicationContext.HostEnviroment.WebRootPath, @"images\products", fileName);

                //Lưu file lên server
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    uploadPhoto.CopyTo(stream);
                }
                //Gán tên file ảnh cho model.Photo
                model.Photo = fileName;
            }
            if (!ModelState.IsValid)
            {
                ViewBag.Title = model.PhotoID == 0 ? "Bổ sung ảnh" : "Thay đổi ảnh";
                return View("Photo", model);
            }

            // thực hiện thêm hoặc cập nhật
            if (model.PhotoID == 0)
            {
                long id = ProductDataService.AddPhoto(model);
                if (id <= 0)
                {
                    ModelState.AddModelError(nameof(model.Photo), "ảnh bị trùng ");
                    ViewBag.Title = "Bổ sung mặt hàng";
                    return View("Photo", model);
                }
            }
            else
            {
                bool result = ProductDataService.UpdatePhoto(model);
                if (!result)
                {
                    ModelState.AddModelError("Error", "Không cập nhật được ảnh mặt hàng. Có thể ảnh bị trùng");
                    ViewBag.Title = "Cập nhật mặt hàng";
                    return View("Photo", model);
                }
            }
            return RedirectToAction("Edit", new { id = model.ProductID });
        }
        public IActionResult Attribute(int id = 0, string method = "", int attributeId = 0)
        {
            ProductAttribute model = null;
            switch (method)
            {
                case "add":
                    ViewBag.Title = "Bổ sung thuộc tính";
                    model = new ProductAttribute()
                    {
                        AttributeID = 0,
                        ProductID = id,
                    };
                    return View(model);
                case "edit":
                    ViewBag.Title = "Thay đổi thuộc tính";
                    if (attributeId < 0)
                    {
                        return RedirectToAction("Index");
                    }
                    model = ProductDataService.GetAttribute(attributeId);
                    if (model == null)
                    {
                        return RedirectToAction("Index");
                    }
                    return View(model);
                case "delete":
                    ProductDataService.DeleteAttribute(attributeId);
                    return RedirectToAction("Edit", new { id = id });
                default:
                    return RedirectToAction("Index");
            }

        }
        [HttpPost]
        public ActionResult SaveAttribute(ProductAttribute model)
        {
            // kiểm tra dữ liệu đầu vào
            if (string.IsNullOrWhiteSpace(model.AttributeName))
            {
                ModelState.AddModelError("AttributeName", "Tên thuộc tính không được để trống");
            }
            if (string.IsNullOrWhiteSpace(model.AttributeValue))
            {
                ModelState.AddModelError("AttributeValue", "Giá trị thuộc tính không được để trống");
            }

            if (string.IsNullOrWhiteSpace(model.DisplayOrder.ToString()))
            {
                ModelState.AddModelError("DisplayOrder", "Thứ tự hiển thị thuộc tính không được để trống");
            }
            else if (model.DisplayOrder < 1)
            {
                ModelState.AddModelError("DisplayOrder", "Thứ tự hiển thị thuộc tính phải là một số tự nhiên dương");
            }
            List<ProductAttribute> productAttributes = ProductDataService.ListAttributes(model.ProductID);
            bool isUsedDisplayOrder = false;
            foreach (ProductAttribute item in productAttributes)
            {
                if (item.DisplayOrder == model.DisplayOrder && model.AttributeID != item.AttributeID)
                {
                    isUsedDisplayOrder = true;
                    break;
                }
            }
            if (isUsedDisplayOrder)
            {
                ModelState.AddModelError("DisplayOrder",
                        $"Thứ tự hiển thị {model.DisplayOrder} của thuộc tính đã được sử dụng trước đó");
            }

            if (!ModelState.IsValid)
            {
                ViewBag.Title = model.AttributeID == 0 ? "Bổ sung thuộc tính" : "Thay đổi thuộc tính";
                return View("Attribute", model);
            }

            // thực hiện thêm hoặc cập nhật
            if (model.AttributeID == 0)
            {
                long id = ProductDataService.AddAttribute(model);
                if (id <= 0)
                {
                    ModelState.AddModelError(nameof(model.AttributeName), "Tên thuộc tính bị trùng ");
                    return View("Attribute", model);
                }
            }
            else
            {
                ProductDataService.UpdateAttribute(model);
                bool result = ProductDataService.UpdateAttribute(model);
                if (!result)
                {
                    ModelState.AddModelError("Error", "Không cập nhật được thuộc tính. Có thể tên thuộc tính bị trùng");
                    ViewBag.Title = "Cập nhật mặt hàng";
                    return View("Attribute", model);
                }
            }
            return RedirectToAction("Edit", new { id = model.ProductID });
        }
    }
}