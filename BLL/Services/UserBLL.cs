using DAL.Repositories;
using DTO.DTOs;
using MuVi.DAL;
using MuVi.DTO;

namespace MuVi.BLL
{
    public class UserBLL
    {
        private UserDAL _userDAL = new UserDAL();

        public (bool Success, string Message, UserDTO User) AuthenticateUser(string username, string password)
        {
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
                return (false, "Tên đăng nhập và mật khẩu không được để trống!", null);

            UserDTO user = _userDAL.Login(username, password);

            if (user != null)
                return (true, "Đăng nhập thành công!", user);
            else
                return (false, "Sai tài khoản hoặc mật khẩu!", null);
        }
    }
}