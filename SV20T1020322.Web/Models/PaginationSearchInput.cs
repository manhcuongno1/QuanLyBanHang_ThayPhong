﻿namespace SV20T1020322.Web.Models
{
    public class PaginationSearchInput
    {
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 0;
        public string SearchValue { get; set; } = "";

    }

    /// <summary>
    /// đầu vào tìm kiếm dành cho mặt hàng
    /// </summary>
    public class ProductSearchInput : PaginationSearchInput
    {
        public int CategoryID { get; set; } = 0;
        public int SupplierID { get; set; } = 0;
        public decimal MinPrice { get; set; }
        public decimal MaxPrice { get; set; }
        public int CustomerID { get; set; } = 0;
        public string Province { get; set; } = "";
    }

   

}
