using SV20T1020322.BusinessLayers;
using SV20T1020322.DataLayers.SQLServer;
using SV20T1020322.DataLayers;
using SV20T1020322.DomainModels;

namespace SV20T1020322.BusinessLayers
{
    /// <summary>
    /// Cung cấp các chức năng nghiệp vụ liên quan đến các dữ liệu chung
    /// (Tỉnh/thành, khách hàng, Nhà cung cấp , loại hàng , người giao hàng , nhân viên)
    /// </summary>
    public static class CommonDataService
    {
        private static readonly ICommonDAL<Province> provinceDB;
        private static readonly ICommonDAL<Customer> customerDB;
        private static readonly ICommonDAL<Category> categoryDB;
        private static readonly ICommonDAL<Supplier> supplierDB;
        private static readonly ICommonDAL<Shipper> shipperDB;
        private static readonly ICommonDAL<Employee> employeeDB;

        static CommonDataService()
        {
            string connectionString = Configuration.ConnectionString;

            provinceDB = new ProvinceDAL(connectionString);
            customerDB = new CustomerDAL(connectionString);
            categoryDB = new CategoryDAL(connectionString);
            supplierDB = new SupplierDAL(connectionString);
            shipperDB = new ShipperDAL(connectionString);
            employeeDB = new EmployeeDAL(connectionString);
        }
        public static List<Province> ListOfProvinces()
        {
            return provinceDB.List().ToList();
        }
        /// <summary>
        /// Tìm kiếm và lấy danh sách khách hàng
        /// </summary>
        /// <param name="rowCount">Tham số đầu ra cho biết số dòng dữ liệu tìm được</param>
        /// <param name="page">Trang cần hiển thị</param>
        /// <param name="pageSize">Số dòng trên mỗi trang(0 nếu không phân trang)</param>
        /// <param name="searchValue">Giá trị tìm kiếm ( rỗng nếu lấy toàn bộ khách hàng)</param>
        /// <returns></returns>
        public static List<Customer> ListOfCustomers(out int rowCount, int page = 1, int pageSize = 0, string searchValue = "")
        {
            rowCount = customerDB.Count(searchValue);
            return customerDB.List(page, pageSize, searchValue).ToList();
        }
        /// <summary>
        /// Lấy thông tin của một khách hàng theo mã khách hàng
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static Customer? GetCustomer(int id)
        {
            return customerDB.Get(id);
        }
        /// <summary>
        /// Bổ sung 1 khách hàng mới.Hàm trả về mã khách hàng mới được bổ sung
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static int AddCustomer(Customer data)
        {
            return customerDB.Add(data);
        }

        public static bool UppdateCustomer(Customer data)
        {
            return customerDB.Update(data);
        }

        public static bool DeleteCustomer(int id)
        {
            if (customerDB.IsUsed(id))
                return false;
            return customerDB.Delete(id);
        }
        public static bool IsUsedCustomer(int id)
        {
            return customerDB.IsUsed(id);
        }

        /// <summary>
        /// Tìm kiếm và lấy danh sách loại hàng
        /// </summary>
        /// <param name="rowCount"></param>
        /// <param name="page"></param>
        /// <param name="pageSize"></param>
        /// <param name="searchValue"></param>
        /// <returns></returns>
        public static List<Category> ListOfCategories(out int rowCount, int page = 1, int pageSize = 0, string searchValue = "")
        {
            rowCount = categoryDB.Count(searchValue);
            return categoryDB.List(page, pageSize, searchValue).ToList();
        }
        /// <summary>
        /// Tìm kiếm và lấy danh sách loại hàng không phân trang
        /// </summary>
        /// <param name="rowCount"></param>
        /// <param name="page"></param>
        /// <param name="pageSize"></param>
        /// <param name="searchValue"></param>
        /// <returns></returns>
        public static List<Category> ListOfCategories(string searchValue = "")
        {
            return categoryDB.List(1, 0, searchValue).ToList();
        }
        public static Category? GetCategory(int id)
        {
            return categoryDB.Get(id);
        }
        public static int AddCategory(Category data)
        {
            return categoryDB.Add(data);
        }

        public static bool UppdateCategory(Category data)
        {
            return categoryDB.Update(data);
        }

        public static bool DeleteCategory(int id)
        {
            if (categoryDB.IsUsed(id))
                return false;
            return categoryDB.Delete(id);
        }
        public static bool IsUsedCategory(int id)
        {
            return categoryDB.IsUsed(id);
        }

        /// <summary>
        /// Supplier
        /// </summary>
        /// <param name="rowCount"></param>
        /// <param name="page"></param>
        /// <param name="pageSize"></param>
        /// <param name="searchValue"></param>
        /// <returns></returns>
        public static List<Supplier> ListOfSuppliers(out int rowCount, int page = 1, int pageSize = 0, string searchValue = "")
        {
            rowCount = supplierDB.Count(searchValue);
            return supplierDB.List(page, pageSize, searchValue).ToList();
        }
        /// <summary>
        /// Tìm kiếm và lấy danh sách Nhà Cung Cấp không phân trang
        /// </summary>
        /// <param name="rowCount"></param>
        /// <param name="page"></param>
        /// <param name="pageSize"></param>
        /// <param name="searchValue"></param>
        /// <returns></returns>
        public static List<Supplier> ListOfSuppliers(string searchValue = "")
        {
            return supplierDB.List(1, 0, searchValue).ToList();
        }
        public static Supplier? GetSupplier(int id)
        {
            return supplierDB.Get(id);
        }
        public static int AddSupplier(Supplier data)
        {
            return supplierDB.Add(data);
        }

        public static bool UppdateSupplier(Supplier data)
        {
            return supplierDB.Update(data);
        }

        public static bool DeleteSupplier(int id)
        {
            if (supplierDB.IsUsed(id))
                return false;
            return supplierDB.Delete(id);
        }
        public static bool IsUsedSupplier(int id)
        {
            return supplierDB.IsUsed(id);
        }
        /// <summary>
        /// Shipper
        /// </summary>
        /// <param name="rowCount"></param>
        /// <param name="page"></param>
        /// <param name="pageSize"></param>
        /// <param name="searchValue"></param>
        /// <returns></returns>
        public static List<Shipper> ListOfShippers(out int rowCount, int page = 1, int pageSize = 0, string searchValue = "")
        {
            rowCount = shipperDB.Count(searchValue);
            return shipperDB.List(page, pageSize, searchValue).ToList();
        }
        public static Shipper? GetShipper(int id)
        {
            return shipperDB.Get(id);
        }
        public static int AddShipper(Shipper data)
        {
            return shipperDB.Add(data);
        }

        public static bool UppdateShipper(Shipper data)
        {
            return shipperDB.Update(data);
        }

        public static bool DeleteShipper(int id)
        {
            if (shipperDB.IsUsed(id))
                return false;
            return shipperDB.Delete(id);
        }
        public static bool IsUsedShipper(int id)
        {
            return shipperDB.IsUsed(id);
        }

        //Employee
        public static List<Employee> ListOfEmployees(out int rowCount, int page = 1, int pageSize = 0, string searchValue = "")
        {
            rowCount = employeeDB.Count(searchValue);
            return employeeDB.List(page, pageSize, searchValue).ToList();
        }
        public static Employee? GetEmployee(int id)
        {
            return employeeDB.Get(id);
        }
        public static int AddEmployee(Employee data)
        {
            return employeeDB.Add(data);
        }

        public static bool UppdateEmployee(Employee data)
        {
            return employeeDB.Update(data);
        }

        public static bool DeleteEmployee(int id)
        {
            if (employeeDB.IsUsed(id))
                return false;
            return employeeDB.Delete(id);
        }
        public static bool IsUsedEmployee(int id)
        {
            return employeeDB.IsUsed(id);
        }
    }
}
//ctr +M+O
//ctr + R + R (refactor)
//ctr + M + L