using System;
using MuVi.DTO.DTOs;

namespace MuVi.Helpers
{
    /// <summary>
    /// Lưu trữ thông tin phiên làm việc hiện tại của người dùng (Singleton Pattern)
    /// </summary>
    public class AppSession
    {
        private static AppSession _instance;
        public static AppSession Instance => _instance ??= new AppSession();

        // Private constructor để ngăn chặn việc khởi tạo từ bên ngoài
        private AppSession() { }

        /// <summary>
        /// Thông tin người dùng hiện tại
        /// </summary>
        public UserDTO CurrentUser { get; set; }

        /// <summary>
        /// Thời điểm đăng nhập
        /// </summary>
        public DateTime LoginTime { get; set; }

        /// <summary>
        /// Kiểm tra xem người dùng đã đăng nhập chưa
        /// </summary>
        public bool IsLoggedIn => CurrentUser != null;

        /// <summary>
        /// Xóa phiên làm việc khi đăng xuất
        /// </summary>
        public void Clear()
        {
            CurrentUser = null;
            LoginTime = default;
        }
    }
}