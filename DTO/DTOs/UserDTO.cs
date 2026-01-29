using System.ComponentModel;

namespace MuVi.DTO.DTOs
{
    public class UserDTO
    {
        // main properties
        public int UserID { get; set; }
        public string Username { get; set; } = "";
        public string Password { get; set; } = "";
        public string? Email { get; set; }
        public string Role { get; set; } = "User";
        public bool IsActive { get; set; }
        public string? LastLogin { get; set; }

        // additional properties
        public string? FullName { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string? Avatar { get; set; }


        // Property for checkbox selection
        private bool _isSelected;
        public bool IsSelected
        {
            get => _isSelected;
            set
            {
                if (_isSelected != value)
                {
                    _isSelected = value;
                    OnPropertyChanged(nameof(IsSelected));
                }
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public override string ToString()
        {
            return Username ?? "";
        }
    }

}
