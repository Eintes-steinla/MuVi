using System.Windows.Input;

namespace MuVi.Commands
{
    /// <summary>
    /// RelayCommand dùng để liên kết hành động từ View (Button)
    /// sang ViewModel trong mô hình MVVM.
    /// </summary>
    public class RelayCommand : ICommand
    {
        // Hàm thực thi khi command được gọi
        private readonly Action<object> _execute;

        // Điều kiện cho phép command được thực thi
        private readonly Predicate<object> _canExecute;

        /// <summary>
        /// Khởi tạo command luôn cho phép thực thi
        /// </summary>
        public RelayCommand(Action<object> execute) : this(execute, null) { }

        /// <summary>
        /// Khởi tạo command với điều kiện thực thi
        /// </summary>
        public RelayCommand(Action<object> execute, Predicate<object> canExecute)
        {
            _execute = execute ?? throw new ArgumentNullException(nameof(execute));
            _canExecute = canExecute;
        }

        /// <summary>
        /// Sự kiện được gọi khi trạng thái CanExecute thay đổi
        /// (giúp WPF tự động bật / tắt Button)
        /// </summary>
        public event EventHandler? CanExecuteChanged
        {
            add    { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        /// <summary>
        /// Kiểm tra command có được phép thực thi hay không
        /// </summary>
        public bool CanExecute(object parameter)
        {
            return _canExecute == null || _canExecute(parameter);
        }

        /// <summary>
        /// Thực thi hành động của command
        /// </summary>
        public void Execute(object parameter)
        {
            _execute(parameter);
        }

        /// <summary>
        /// Cập nhật lại trạng thái CanExecute
        /// </summary>
        public void RaiseCanExecuteChanged()
        {
            CommandManager.InvalidateRequerySuggested();
        }
    }

    /// <summary>
    /// Phiên bản generic của RelayCommand
    /// giúp kiểm soát kiểu dữ liệu an toàn hơn
    /// </summary>
    public class RelayCommand<T> : ICommand
    {
        private readonly Action<T> _execute;
        private readonly Predicate<T> _canExecute;

        public RelayCommand(Action<T> execute)
            : this(execute, null)
        {
        }

        public RelayCommand(Action<T> execute, Predicate<T> canExecute)
        {
            _execute = execute ?? throw new ArgumentNullException(nameof(execute));
            _canExecute = canExecute;
        }

        public event EventHandler CanExecuteChanged
        {
            add    { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public bool CanExecute(object parameter)
        {
            return _canExecute == null || _canExecute((T)parameter);
        }

        public void Execute(object parameter)
        {
            _execute((T)parameter);
        }

        public void RaiseCanExecuteChanged()
        {
            CommandManager.InvalidateRequerySuggested();
        }
    }
}
