using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SV20T1020322.BusinessLayers
{
    public static class Configuration
    {
        /// <summary>
        /// chuỗi thông số kết nối CSDL
        /// </summary>
        public static string ConnectionString { get; set; } = "";
        /// <summary>
        /// Hàm khởi tạo cấu hình cho BusinessLayer
        /// Hàm này phải được gọi trước khi chạy ứng dụng
        /// </summary>
        /// <param name="connectionString"></param>
        public static void Initialize(string connectionString)
        {
            Configuration.ConnectionString = connectionString;
        }
    }
}
