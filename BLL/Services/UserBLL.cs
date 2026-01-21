using Muvi.DAL;
using MuVi.DTO;
using MuVi.DTO.DTOs;
using BCrypt.Net;

namespace MuVi.BLL
{
    public class UserBLL
    {
        UserDAL userDAL = new UserDAL();

        /// <summary>
        /// xử lý đăng ký tài khoản
        /// </summary>
        /// <param name="username"></param>
        /// <param name="email"></param>
        /// <param name="password"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        public bool Register(string username, string email, string password, out string message)
        {
            // Username tồn tại
            if (userDAL.IsUsernameExists(username))
            {
                message = "Tên đăng nhập đã tồn tại";
                return false;
            }

            // Email tồn tại
            if (userDAL.IsEmailExists(email))
            {
                message = "Email đã được sử dụng";
                return false;
            }

            // Hash mật khẩu
            string hashPassword = BCrypt.Net.BCrypt.HashPassword(password);

            // Tạo object User
            UserDTO user = new UserDTO()
            {
                Username = username,
                Password = hashPassword,
                Email = email
            };

            // Lưu DB
            bool result = userDAL.Register(user);

            message = result ? "Đăng ký thành công" : "Đăng ký thất bại";
            return result;
        }

        /// <summary>
        /// xử lý đăng nhập
        /// </summary>
        /// <param name="usernameOrEmail"></param>
        /// <param name="password"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        public UserDTO? Login(string usernameOrEmail, string password, out string message)
        {
            // Lấy user theo username hoặc email
            UserDTO? user = userDAL.GetByUsernameOrEmail(usernameOrEmail);

            if (user == null)
            {
                message = "Tài khoản không tồn tại";
                return null;
            }

            // Kiểm tra tài khoản bị khóa
            if (user.IsActive == false)
            {
                message = "Tài khoản đã bị khóa";
                return null;
            }

            // So sánh mật khẩu hash
            bool isPasswordValid = BCrypt.Net.BCrypt.Verify(password, user.Password);

            if (isPasswordValid == false)
            {
                message = "Mật khẩu không chính xác";
                return null;
            }

            // Thành công
            message = "Đăng nhập thành công";
            return user;
        }

        /// <summary>
        /// lần cuối đăng nhập
        /// </summary>
        /// <param name="userID"></param>
        /// <returns></returns>
        public bool UpdateLastLogin(int userID)
        {
            return userDAL.UpdateLastLogin(userID);
        }
    }

}