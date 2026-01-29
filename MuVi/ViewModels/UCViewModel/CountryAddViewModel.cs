using MuVi.BLL;
using MuVi.Commands;
using MuVi.DTO.DTOs;
using System.Windows;
using System.Windows.Input;

namespace MuVi.ViewModels.UCViewModel
{
    public class CountryAddViewModel : BaseViewModel
    {
        #region Fields
        private CountryDTO _country;
        private CountryBLL _countryBLL = new CountryBLL();
        private bool _isAddMode = true;
        #endregion

        #region Properties

        public CountryDTO Country
        {
            get => _country;
            set => SetProperty(ref _country, value);
        }

        public string CountryName
        {
            get => _country?.CountryName;
            set
            {
                if (_country != null)
                {
                    _country.CountryName = value;
                    OnPropertyChanged(nameof(CountryName));
                }
            }
        }

        public string CountryCode
        {
            get => _country?.CountryCode;
            set
            {
                if (_country != null)
                {
                    _country.CountryCode = value;
                    OnPropertyChanged(nameof(CountryCode));
                }
            }
        }

        public bool IsAddMode
        {
            get => _isAddMode;
            set => SetProperty(ref _isAddMode, value);
        }

        #endregion

        #region Constructor

        public CountryAddViewModel(CountryDTO existingCountry = null)
        {
            if (existingCountry != null)
            {
                // Edit mode
                _country = new CountryDTO
                {
                    CountryID = existingCountry.CountryID,
                    CountryName = existingCountry.CountryName,
                    CountryCode = existingCountry.CountryCode
                };
                IsAddMode = false;
            }
            else
            {
                // Add mode
                _country = new CountryDTO
                {
                    CountryName = "",
                    CountryCode = ""
                };
                IsAddMode = true;
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Validate dữ liệu
        /// </summary>
        public bool Validate()
        {
            if (string.IsNullOrWhiteSpace(CountryName))
            {
                MessageBox.Show("Vui lòng nhập tên quốc gia!",
                    "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            if (!string.IsNullOrWhiteSpace(CountryCode) && CountryCode.Length > 10)
            {
                MessageBox.Show("Mã quốc gia không được vượt quá 10 ký tự!",
                    "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            return true;
        }

        #endregion
    }
}