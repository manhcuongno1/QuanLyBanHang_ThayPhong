﻿using SV20T1020322.DataLayers;
using SV20T1020322.DataLayers.SQLServer;
using SV20T1020322.DomainModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SV20T1020322.BusinessLayers
{
    public static class ProductDataService
    {
        private static readonly IProductDAL productDB;
        /// <summary>
        /// Ctor
        /// </summary>
        static ProductDataService()
        {
            productDB = new ProductDAL(Configuration.ConnectionString);
        }
        /// <summary>
        /// Tìm kiếm và lấy danh sách mặt hàng (không phân trang)
        /// </summary>
        /// <param name="searchValue">Tên mặt hàng cần tìm (chuỗi rỗng nếu không tìm kiếm)</param>
        /// <returns></returns>
        public static List<Product> ListProducts(string searchValue = "")
        {
            return productDB.List(1, 0, searchValue, 0, 0, 0, 0).ToList();
        }
        /// <summary>
        /// Tìm kiếm và lấy danh sách mặt hàng dưới dạng phân trang
        /// </summary>
        /// <param name="page"></param>
        /// <param name="pageSize"></param>
        /// <param name="searchValue"></param>
        /// <param name="categoryID"></param>
        /// <param name="supplierID"></param>
        /// <param name="rowCount"></param>
        /// <returns></returns>
        public static List<Product> ListProducts(out int rowCount, int page = 1, int pageSize = 0,
                        string searchValue = "", int categoryID = 0, int supplierID = 0,
                        decimal minPrice = 0, decimal maxPrice = 0)
        {
            rowCount = productDB.Count(searchValue, categoryID, supplierID, minPrice, maxPrice);
            return productDB.List(page, pageSize, searchValue, categoryID, supplierID, minPrice, maxPrice).ToList();
        }



        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        /// 
        public static Product GetProduct(int productID)
        {
            return productDB.Get(productID);
        }
        public static int AddProduct(Product data)
        {
            return productDB.Add(data);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static bool UpdateProduct(Product data)
        {
            return productDB.Update(data);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="productID"></param>
        /// <returns></returns>
        public static bool DeleteProduct(int productID)
        {
            return productDB.Delete(productID);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="productID"></param>
        /// <returns></returns>
        public static bool InUsedProduct(int productID)
        {
            return productDB.InUsed(productID);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="productID"></param>
        /// <returns></returns>
        public static List<ProductPhoto> ListPhotos(int productID)
        {
            return productDB.ListPhotos(productID).ToList();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="photoID"></param>
        /// <returns></returns>
        public static ProductPhoto GetPhoto(long photoID)
        {
            return productDB.GetPhoto(photoID);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static long AddPhoto(ProductPhoto data)
        {
            return productDB.AddPhoto(data);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static bool UpdatePhoto(ProductPhoto data)
        {
            return productDB.UpdatePhoto(data);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="photoID"></param>
        /// <returns></returns>
        public static bool DeletePhoto(long photoID)
        {
            return productDB.DeletePhoto(photoID);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="productID"></param>
        /// <returns></returns>
        public static List<ProductAttribute> ListAttributes(int productID)
        {
            return productDB.ListAttributes(productID).ToList();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="attributeID"></param>
        /// <returns></returns>
        public static ProductAttribute GetAttribute(long attributeID)
        {
            return productDB.GetAttribute(attributeID);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static long AddAttribute(ProductAttribute data)
        {
            return productDB.AddAttribute(data);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static bool UpdateAttribute(ProductAttribute data)
        {
            return productDB.UpdateAttribute(data);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="attributeID"></param>
        /// <returns></returns>
        public static bool DeleteAttribute(long attributeID)
        {
            return productDB.DeleteAttribute(attributeID);
        }
    }
}
