using Dapper;
using Microsoft.Data.SqlClient;
using SV20T1020322.DomainModels;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SV20T1020322.DataLayers.SQLServer
{
    public class SupplierDAL : _BaseDAL, ICommonDAL<Supplier>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="connectionString"></param>
        public SupplierDAL(string connectionString) : base(connectionString)
        {
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public int Add(Supplier data)
        {
            int id = 0;
            using (var connection = OpenConnection())
            {
                var sql = @"INSERT INTO Suppliers(SupplierName, ContactName, Province, Address, Phone, Email,NgayThanhLap)
                                    VALUES(@SupplierName, @ContactName, @Province, @Address, @Phone, @Email,@NgayThanhLap);
                                    SELECT @@identity";


                var parameters = new
                {
                    SupplierName = data.SupplierName ?? "",
                    ContactName = data.ContactName ?? "",
                    Province = data.Province ?? "",
                    Address = data.Address ?? "",
                    Phone = data.Phone ?? "",
                    Email = data.Email ?? "",
                    NgayThanhLap = data.NgayThanhLap 

                };
                id = connection.ExecuteScalar<int>(sql: sql, param: parameters, commandType: CommandType.Text);
                connection.Close();
            }
            return id;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="searchValue"></param>
        /// <returns></returns>
        public int Count(string searchValue = "")
        {
            int count = 0;
            if (!string.IsNullOrEmpty(searchValue))
                searchValue = "%" + searchValue + "%";
            using (var connection = OpenConnection())
            {
                var sql = @"SELECT	COUNT(*)
                                    FROM	Suppliers 
                                    WHERE	(@SearchValue = N'')
	                                    OR	(
			                                    (SupplierName LIKE @SearchValue)
			                                    OR (ContactName LIKE @SearchValue)
			                                    OR (Address LIKE @SearchValue)
                                                OR (Phone LIKE @SearchValue)
		                                    )";
                var parameters = new { searchValue = searchValue ?? "" };
                count = connection.ExecuteScalar<int>(sql: sql, param: parameters, commandType: CommandType.Text);
                connection.Close();
            }
            return count;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public bool Delete(int id)
        {
            bool result = false;
            using (var connection = OpenConnection())
            {
                var sql = @"DELETE FROM Suppliers 
                                    WHERE SupplierID = @SupplierID AND NOT EXISTS(SELECT * FROM Products WHERE SupplierID = @SupplierID)";
                var parameters = new
                {
                    SupplierID = id,
                };
                //Thực thi
                result = connection.Execute(sql: sql, param: parameters, commandType: CommandType.Text) > 0;
                connection.Close();
            }
            return result;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Supplier Get(int id)
        {
            Supplier? data = null;
            using (var connection = OpenConnection())
            {
                var sql = @"SELECT * FROM Suppliers WHERE SupplierID = @SupplierID";
                var parameters = new
                {
                    SupplierID = id
                };
                //Thực thi
                data = connection.QueryFirstOrDefault<Supplier>(sql: sql, param: parameters, commandType: CommandType.Text);
                connection.Close();
            }
            return data;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public bool IsUsed(int id)
        {
            bool result = false;
            using (var connection = OpenConnection())
            {
                var sql = @"if exists(select * from Products where SupplierId = @SupplierId)
                                select 1
                            else 
                                select 0";
                var parameters = new
                {
                    SupplierID = id,
                };
                //Thực thi
                result = connection.ExecuteScalar<bool>(sql: sql, param: parameters, commandType: CommandType.Text);
                connection.Close();
            }
            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="page"></param>
        /// <param name="pageSize"></param>
        /// <param name="searchValue"></param>
        /// <returns></returns>
        public IList<Supplier> List(int page = 1, int pageSize = 0, string searchValue = "")
        {
            List<Supplier> list = new List<Supplier>();
            if (!string.IsNullOrEmpty(searchValue))
                searchValue = "%" + searchValue + "%"; // tìm kiếm tương đối dấu %
            using (var connection = OpenConnection())
            {
                var sql = @"SELECT *
                                    FROM 
                                    (
	                                    SELECT	*, ROW_NUMBER() OVER (ORDER BY SupplierName) AS RowNumber
	                                    FROM	Suppliers 
	                                    WHERE	(@SearchValue = N'')
		                                    OR	(
				                                    (SupplierName LIKE @SearchValue)
			                                     OR (ContactName LIKE @SearchValue)
			                                     OR (Address LIKE @SearchValue)
                                                 OR (Phone LIKE @SearchValue)
			                                    )
                                    ) AS t
                                    WHERE (@PageSize = 0) OR (t.RowNumber BETWEEN (@Page - 1) * @PageSize + 1 AND @Page * @PageSize);";
                //Có dấu @ là tham số 
                var parameters = new
                {
                    page = page,
                    pageSize = pageSize,
                    searchValue = searchValue ?? "" //nếu searchValue = null thì dc thay bằng ""
                };
                list = connection.Query<Supplier>(sql: sql, param: parameters, commandType: CommandType.Text).ToList();
                connection.Close();
            }
            return list;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public bool Update(Supplier data)
        {
            bool result = false;
            using (var connection = OpenConnection())
            {
                var sql = @"UPDATE Suppliers
                                    SET SupplierName = @SupplierName, ContactName = @ContactName, Province = @Province, Address = @Address, Phone = @Phone, Email = @Email, NgayThanhLap = @NgayThanhLap
                                    WHERE SupplierID = @SupplierID";
                var parameters = new
                {
                    SupplierID = data.SupplierID,
                    SupplierName = data.SupplierName ?? "",
                    ContactName = data.ContactName ?? "",
                    Province = data.Province ?? "",
                    Address = data.Address ?? "",
                    Phone = data.Phone ?? "",
                    Email = data.Email ?? "",
                    NgayThanhLap = data.NgayThanhLap

                };
                //Thực thi
                result = connection.Execute(sql: sql, param: parameters, commandType: CommandType.Text) > 0;
                connection.Close();
            }
            return result;
        }
    }
}
