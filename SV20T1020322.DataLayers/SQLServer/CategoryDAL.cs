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
    /// cài đặt xử lý dữ liệu cho loại hàng
    /// </summary>
    public class CategoryDAL : _BaseDAL, ICommonDAL<Category>
    {
        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="connectionString"></param>
        public CategoryDAL(string connectionString) : base(connectionString)
        {

        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public int Add(Category data)
        {
            int id = 0;
            using (var connection = OpenConnection())
            {
                var sql = @"insert into Categories(CategoryName, Description)
                                    VALUES(@CategoryName, @Description);
                                    select @@identity;";
                var parameters = new
                {
                    CategoryName = data.CategoryName ?? "",
                    Description = data.Description ?? "",     
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
                                    FROM	Categories 
                                    WHERE	(@SearchValue = N'')
	                                    OR	(
			                                    (CategoryName LIKE @SearchValue)
			                                    OR (Description LIKE @SearchValue)

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
                var sql = @"DELETE FROM Categories 
                                    WHERE  CategoryID = @CategoryID AND NOT EXISTS(SELECT * FROM Products WHERE CategoryID = @CategoryID)";
                var parameters = new
                {
                    CategoryID = id,
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
        public Category Get(int id)
        {
            Category? data = null;
            using (var connection = OpenConnection())
            {
                var sql = @"select * from Categories where CategoryID = @CategoryID";
                var parameters = new
                {
                    CategoryID = id
                };
                //Thực thi
                data = connection.QueryFirstOrDefault<Category>(sql: sql, param: parameters, commandType: CommandType.Text);
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
                var sql = @"if exists(select * from Products where CategoryId = @CategoryId)
                                select 1
                            else 
                                select 0";
                var parameters = new
                {
                    CategoryID = id,
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
        public IList<Category> List(int page = 1, int pageSize = 0, string searchValue = "")
        {
            List<Category> list = new List<Category>();
            if (!string.IsNullOrEmpty(searchValue))
                searchValue = "%" + searchValue + "%"; // tìm kiếm tương đối dấu %
            using (var connection = OpenConnection())
            {
                var sql = @"SELECT *
                                    FROM 
                                    (
	                                    SELECT	*, ROW_NUMBER() OVER (ORDER BY CategoryName) AS RowNumber
	                                    FROM	Categories 
	                                    WHERE	(@SearchValue = N'')
		                                    OR	(
				                                    (CategoryName LIKE @SearchValue)
			                                     OR (Description LIKE @SearchValue)
			                                    
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
                list = connection.Query<Category>(sql: sql, param: parameters, commandType: CommandType.Text).ToList();
                connection.Close();
            }
            return list;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public bool Update(Category data)
        {
            bool result = false;
            using (var connection = OpenConnection())
            {
                var sql = @"UPDATE Categories
                                    SET CategoryName = @CategoryName, Description = @Description
                                    WHERE CategoryID = @CategoryID";
                var parameters = new
                {
                    CategoryID = data.CategoryID,
                    CategoryName = data.CategoryName ?? "",
                    Description = data.Description ?? "",
                    
                };
                //Thực thi
                result = connection.Execute(sql: sql, param: parameters, commandType: CommandType.Text) > 0;
                connection.Close();
            }
            return result;
        }

    }
}
