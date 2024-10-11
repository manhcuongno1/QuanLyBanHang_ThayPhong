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
    /// <summary>
    /// 
    /// </summary>
    public class EmployeeDAL : _BaseDAL, ICommonDAL<Employee>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="connectionString"></param>
        public EmployeeDAL(string connectionString) : base(connectionString)
        {
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public int Add(Employee data)
        {
            int id = 0;
            using (var connection = OpenConnection())
            {
                var sql = @"INSERT INTO Employees(FullName, BirthDate, Address, Phone, Email, Photo,IsWorking)
                                    VALUES(@FullName, @BirthDate, @Address, @Phone, @Email, @Photo,@IsWorking);
                                    SELECT @@identity";
                var parameters = new
                {
                    FullName = data.FullName ?? "",
                    BirthDate = data.BirthDate ,
                    Address = data.Address ?? "",
                    Phone = data.Phone ?? "",
                    Email = data.Email ?? "",
                    Photo = data.Photo ?? "",
                    IsWorking = data.IsWorking,
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
        public int Count(string searchValue)
        {
            int count = 0;
            if (!string.IsNullOrEmpty(searchValue))
                searchValue = "%" + searchValue + "%";
            using (var connection = OpenConnection())
            {
                var sql = @"SELECT	COUNT(*)
                                    FROM	Employees 
                                    WHERE	(@SearchValue = N'')
	                                    OR	(
			                                       (FullName LIKE @SearchValue)
			                                    OR (Email LIKE @SearchValue)
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
                var sql = @"DELETE FROM Employees WHERE EmployeeID = @EmployeeID AND NOT EXISTS(SELECT * FROM Orders WHERE EmployeeID = @EmployeeID)";
                var parameters = new
                {
                    EmployeeID = id,
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
        public Employee Get(int id)
        {
            Employee? data = null;
            using (var connection = OpenConnection())
            {
                var sql = @"SELECT * FROM Employees WHERE EmployeeID = @EmployeeID";
                var parameters = new
                {
                    EmployeeID = id
                };
                //Thực thi
                data = connection.QueryFirstOrDefault<Employee>(sql: sql, param: parameters, commandType: CommandType.Text);
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
                var sql = @"if exists(select * from Orders where EmployeeId = @EmployeeId)
                                select 1
                            else 
                                select 0";
                var parameters = new
                {
                    EmployeeID = id,
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
        public IList<Employee> List(int page, int pageSize, string searchValue)
        {
            List<Employee> list = new List<Employee>();
            if (!string.IsNullOrEmpty(searchValue))
                searchValue = "%" + searchValue + "%"; // tìm kiếm tương đối dấu %
            using (var connection = OpenConnection())
            {
                var sql = @"SELECT *
                                    FROM 
                                    (
	                                    SELECT	*, ROW_NUMBER() OVER (ORDER BY FullName) AS RowNumber
	                                    FROM	Employees 
	                                    WHERE	(@SearchValue = N'')
	                                        OR	(
			                                       (FullName LIKE @SearchValue)
			                                    OR (Email LIKE @SearchValue)
		                                        )
                                    ) AS t
                                    WHERE (@PageSize = 0) OR (t.RowNumber BETWEEN (@Page - 1) * @PageSize + 1 AND @Page * @PageSize)";
                //Có dấu @ là tham số 
                var parameters = new
                {
                    page = page,
                    pageSize = pageSize,
                    searchValue = searchValue ?? "" //nếu searchValue = null thì dc thay bằng ""
                };
                list = connection.Query<Employee>(sql: sql, param: parameters, commandType: CommandType.Text).ToList();
                connection.Close();
            }
            return list;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public bool Update(Employee data)
        {
            bool result = false;
            using (var connection = OpenConnection())
            {
                var sql = @"UPDATE Employees
                                    SET FullName = @FullName, BirthDate = @BirthDate, Address = @Address, Phone = @Phone, Email = @Email, Photo = @Photo, IsWorking = @IsWorking
                                    WHERE EmployeeID = @EmployeeID";
                var parameters = new
                {
                    EmployeeID = data.EmployeeID,
                    FullName = data.FullName ?? "",
                    BirthDate = data.BirthDate ,
                    Address = data.Address ?? "",
                    Phone = data.Phone ?? "",
                    Email = data.Email ?? "",
                    Photo = data.Photo ?? "",
                    IsWorking = data.IsWorking,
                };
                //Thực thi
                result = connection.Execute(sql: sql, param: parameters, commandType: CommandType.Text) > 0;
                connection.Close();
            }
            return result;
        }
    }
}
