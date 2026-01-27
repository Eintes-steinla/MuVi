using Dapper;
using Microsoft.Data.SqlClient;
using Muvi.DAL;
using MuVi.DAL;
using MuVi.DTO.DTOs;
using BCrypt.Net;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MuVi.BLL
{
    public class UserBLL
    {
        private UserDAL userDAL = new UserDAL();

        private int currentPage = 1;
        private int pageSize = 10;

        // Search and filter parameters
        private string _searchKeyword = "";
        private string _statusFilter = "Tất cả";
        private string _roleFilter = "Tất cả";
        private DateTime? _dateFilter = null;

        /// <summary>
        /// Đăng ký người dùng mới
        /// </summary>
        public bool Register(string username, string email, string password, out string message)
        {
            if (userDAL.IsUsernameExists(username))
            {
                message = "Tên đăng nhập đã tồn tại";
                return false;
            }

            if (userDAL.IsEmailExists(email))
            {
                message = "Email đã được sử dụng";
                return false;
            }

            string hashPassword = BCrypt.Net.BCrypt.HashPassword(password);

            UserDTO user = new UserDTO()
            {
                Username = username,
                Password = hashPassword,
                Email = email
            };

            bool result = userDAL.Register(user);
            message = result ? "Đăng ký thành công" : "Đăng ký thất bại";
            return result;
        }

        /// <summary>
        /// Đăng nhập
        /// </summary>
        public UserDTO? Login(string usernameOrEmail, string password, out string message)
        {
            UserDTO? user = userDAL.GetByUsernameOrEmail(usernameOrEmail);
            if (user == null)
            {
                message = "Tài khoản không tồn tại";
                return null;
            }

            if (user.IsActive == false)
            {
                message = "Tài khoản đã bị khóa";
                return null;
            }

            bool isPasswordValid = BCrypt.Net.BCrypt.Verify(password, user.Password);
            if (isPasswordValid == false)
            {
                message = "Mật khẩu không chính xác";
                return null;
            }

            message = "Đăng nhập thành công";
            return user;
        }

        /// <summary>
        /// Cập nhật thời gian đăng nhập cuối
        /// </summary>
        public bool UpdateLastLogin(int userID)
        {
            return userDAL.UpdateLastLogin(userID);
        }

        /// <summary>
        /// Lấy danh sách users với filter
        /// </summary>
        public IEnumerable<UserDTO> GetUsers()
        {
            var allUsers = userDAL.GetAll();

            // Apply search filter
            if (!string.IsNullOrWhiteSpace(_searchKeyword))
            {
                allUsers = allUsers.Where(u =>
                    (u.Username?.Contains(_searchKeyword, StringComparison.OrdinalIgnoreCase) ?? false) ||
                    (u.Email?.Contains(_searchKeyword, StringComparison.OrdinalIgnoreCase) ?? false)
                );
            }

            // Apply status filter
            if (_statusFilter != "Tất cả")
            {
                bool isActive = _statusFilter == "Hoạt động";
                allUsers = allUsers.Where(u => u.IsActive == isActive);
            }

            // Apply role filter
            if (_roleFilter != "Tất cả")
            {
                allUsers = allUsers.Where(u => u.Role == _roleFilter);
            }

            // Apply date filter
            if (_dateFilter != null)
            {
                allUsers = allUsers.Where(u => u.DateOfBirth.HasValue &&
                                               u.DateOfBirth.Value.Date == _dateFilter.Value.Date);
            }

            // Apply pagination
            return allUsers
                .Skip((currentPage - 1) * pageSize)
                .Take(pageSize);
        }

        /// <summary>
        /// Lấy tổng số trang
        /// </summary>
        public int GetTotalPages()
        {
            var allUsers = userDAL.GetAll();

            // Apply same filters
            if (!string.IsNullOrWhiteSpace(_searchKeyword))
            {
                allUsers = allUsers.Where(u =>
                    (u.Username?.Contains(_searchKeyword, StringComparison.OrdinalIgnoreCase) ?? false) ||
                    (u.Email?.Contains(_searchKeyword, StringComparison.OrdinalIgnoreCase) ?? false)
                );
            }

            if (_statusFilter != "Tất cả")
            {
                bool isActive = _statusFilter == "Hoạt động";
                allUsers = allUsers.Where(u => u.IsActive == isActive);
            }

            if (_roleFilter != "Tất cả")
            {
                allUsers = allUsers.Where(u => u.Role == _roleFilter);
            }

            if (_dateFilter != null)
            {
                allUsers = allUsers.Where(u => u.DateOfBirth.HasValue &&
                                               u.DateOfBirth.Value.Date == _dateFilter.Value.Date);
            }

            int totalRecords = allUsers.Count();
            return (int)Math.Ceiling(totalRecords / (double)pageSize);
        }

        public void SetSearchKeyword(string keyword)
        {
            _searchKeyword = keyword;
            currentPage = 1;
        }

        public void SetStatusFilter(string status)
        {
            _statusFilter = status;
            currentPage = 1;
        }

        public void SetRoleFilter(string role)
        {
            _roleFilter = role;
            currentPage = 1;
        }

        public void SetDateFilter(DateTime? date)
        {
            _dateFilter = date;
            currentPage = 1;
        }

        public void ClearFilters()
        {
            _searchKeyword = "";
            _statusFilter = "Tất cả";
            _roleFilter = "Tất cả";
            _dateFilter = null;
            currentPage = 1;
        }

        public void NextPage()
        {
            int totalPages = GetTotalPages();
            if (currentPage < totalPages)
            {
                currentPage++;
            }
        }

        public void PreviousPage()
        {
            if (currentPage > 1)
            {
                currentPage--;
            }
        }

        public void FirstPage()
        {
            currentPage = 1;
        }

        public void LastPage()
        {
            currentPage = GetTotalPages();
        }

        public int GetCurrentPage() => currentPage;

        /// <summary>
        /// Thêm user mới
        /// </summary>
        public bool AddUser(UserDTO user, out string message)
        {
            if (userDAL.IsUsernameExists(user.Username))
            {
                message = "Tên đăng nhập đã tồn tại";
                return false;
            }

            if (!string.IsNullOrEmpty(user.Email) && userDAL.IsEmailExists(user.Email))
            {
                message = "Email đã được sử dụng";
                return false;
            }

            // Hash password trước khi lưu
            user.Password = BCrypt.Net.BCrypt.HashPassword(user.Password);

            bool result = userDAL.AddUser(user);
            message = result ? "Thêm người dùng thành công" : "Thêm người dùng thất bại";
            return result;
        }

        /// <summary>
        /// Cập nhật user
        /// </summary>
        public bool UpdateUser(UserDTO user, out string message)
        {
            bool result = userDAL.UpdateUser(user);
            message = result ? "Cập nhật người dùng thành công" : "Cập nhật người dùng thất bại";
            return result;
        }

        /// <summary>
        /// Xóa user
        /// </summary>
        public bool DeleteUser(int userId, out string message)
        {
            bool result = userDAL.Delete(userId);
            message = result ? "Xóa người dùng thành công" : "Xóa người dùng thất bại";
            return result;
        }

        /// <summary>
        /// Xóa nhiều users
        /// </summary>
        public bool DeleteMultipleUsers(List<int> userIds, out string message)
        {
            int successCount = 0;
            foreach (int userId in userIds)
            {
                if (userDAL.Delete(userId))
                {
                    successCount++;
                }
            }

            message = $"Đã xóa {successCount}/{userIds.Count} người dùng";
            return successCount > 0;
        }
    }
}