using MuVi.BLL;
using MuVi.Commands;
using MuVi.DTO.DTOs;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace MuVi.ViewModels
{
    public class UserViewModel : BaseViewModel
    {
        private readonly UserBLL _userBLL = new UserBLL();

        // DataGrid sẽ Binding vào danh sách này
        public ObservableCollection<UserDTO> UserList { get; set; }

        public UserViewModel()
        {
            // Khởi tạo danh sách tránh lỗi NullReference
            UserList = new ObservableCollection<UserDTO>();

            // Tải dữ liệu ngay khi khởi tạo ViewModel
            LoadUsers();
        }

        public void LoadUsers()
        {
            // Gọi DAL thông qua BLL với các tham số phân trang
            var users = _userBLL.GetUsers();

            UserList.Clear();
            foreach (var u in users) UserList.Add(u);
        }

        // Hàm thực hiện khi ấn nút "Trang sau"
        public void NextPage()
        {
            _userBLL.NextPage();
            LoadUsers();
        }

        // Hàm thực hiện khi ấn nút "Trang trước"
        public void PreviousPage()
        {
            _userBLL.PreviousPage();
            LoadUsers();
        }
    }
}