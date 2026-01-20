using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace MuVi.ViewModels
{
    /// <summary>
    /// Lớp ViewModel cơ sở
    /// Tất cả ViewModel trong dự án đều kế thừa từ lớp này
    /// Dùng để tự động cập nhật giao diện khi dữ liệu thay đổi
    /// </summary>
    public abstract class BaseViewModel : INotifyPropertyChanged
    {
        /// <summary>
        /// Sự kiện được kích hoạt khi giá trị của một thuộc tính thay đổi
        /// WPF sẽ lắng nghe sự kiện này để cập nhật lại giao diện
        /// </summary>
        public event PropertyChangedEventHandler? PropertyChanged;

        /// <summary>
        /// Gửi thông báo cho WPF biết rằng một thuộc tính đã thay đổi giá trị
        /// </summary>
        /// <param name="propertyName">
        /// Tên thuộc tính thay đổi.
        /// Nếu không truyền vào, C# sẽ tự lấy tên hàm gọi nhờ CallerMemberName
        /// </param>
        protected virtual void OnPropertyChanged(
            [CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(
                this,
                new PropertyChangedEventArgs(propertyName)
            );
        }

        /// <summary>
        /// Gán giá trị mới cho biến và tự động cập nhật giao diện nếu giá trị thay đổi
        /// </summary>
        /// <typeparam name="T">Kiểu dữ liệu của thuộc tính</typeparam>
        /// <param name="field">Biến private phía sau property</param>
        /// <param name="value">Giá trị mới cần gán</param>
        /// <param name="propertyName">
        /// Tên property – được tự động lấy bởi CallerMemberName
        /// </param>
        /// <returns>
        /// true  → giá trị đã thay đổi  
        /// false → giá trị không đổi
        /// </returns>
        protected bool SetProperty<T>(
            ref T field,
            T value,
            [CallerMemberName] string? propertyName = null)
        {
            // Nếu giá trị cũ và mới giống nhau thì không làm gì
            if (Equals(field, value))
                return false;

            // Gán giá trị mới
            field = value;

            // Thông báo cho WPF cập nhật giao diện
            OnPropertyChanged(propertyName);

            return true;
        }
    }
}
