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
    public class ShipperDAL : _BaseDAL, ICommonDAL<Shipper>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="connectionString"></param>
        public ShipperDAL(string connectionString) : base(connectionString)
        {
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public int Add(Shipper data)
        {
            int id = 0;
            using (var connection = OpenConnection())
            {
                var sql = @"INSERT INTO Shippers(ShipperName,Phone)
                                    VALUES(@ShipperName,@Phone);
                                    SELECT @@identity";
                var parameters = new
                {
                    ShipperName = data.ShipperName ?? "",
                    Phone = data.Phone ?? "",                   
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
                                    FROM	Shippers 
                                    WHERE	(@SearchValue = N'')
	                                    OR	(
			                                    (ShipperName LIKE @SearchValue)
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
                var sql = @"DELETE FROM Shippers 
                                    WHERE ShipperID = @ShipperID AND NOT EXISTS(SELECT * FROM Orders WHERE ShipperID = @ShipperID)";
                var parameters = new
                {
                    ShipperID = id,
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
        public Shipper Get(int id)
        {
            Shipper? data = null;
            using (var connection = OpenConnection())
            {
                var sql = @"SELECT * FROM Shippers WHERE ShipperID = @ShipperID";
                var parameters = new
                {
                    ShipperID = id
                };
                //Thực thi
                data = connection.QueryFirstOrDefault<Shipper>(sql: sql, param: parameters, commandType: CommandType.Text);
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
                var sql = @"if exists(select * from Orders where ShipperId = @ShipperId)
                                select 1
                            else 
                                select 0";
                var parameters = new
                {
                    ShipperID = id,
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
        public IList<Shipper> List(int page = 1, int pageSize = 0, string searchValue = "")
        {
            List<Shipper> list = new List<Shipper>();
            if (!string.IsNullOrEmpty(searchValue))
                searchValue = "%" + searchValue + "%"; // tìm kiếm tương đối dấu %
            using (var connection = OpenConnection())
            {
                var sql = @"SELECT *
                                    FROM 
                                    (
	                                    SELECT	*, ROW_NUMBER() OVER (ORDER BY ShipperName) AS RowNumber
	                                    FROM	Shippers 
	                                    WHERE	(@SearchValue = N'')
		                                    OR	(
				                                    (ShipperName LIKE @SearchValue)
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
                list = connection.Query<Shipper>(sql: sql, param: parameters, commandType: CommandType.Text).ToList();
                connection.Close();
            }
            return list;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public bool Update(Shipper data)
        {
            bool result = false;
            using (var connection = OpenConnection())
            {
                var sql = @"UPDATE Shippers
                                    SET ShipperName = @ShipperName,Phone = @Phone
                                    WHERE ShipperID = @ShipperID";
                var parameters = new
                {
                    ShipperID = data.ShipperID,
                    ShipperName = data.ShipperName ?? "",
                    Phone = data.Phone ?? "",       
                };
                //Thực thi
                result = connection.Execute(sql: sql, param: parameters, commandType: CommandType.Text) > 0;
                connection.Close();
            }
            return result;
        }
    }
}
