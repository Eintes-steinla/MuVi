using Muvi.DAL;
using MuVi.DTO.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MuVi.BLL
{
    public class CountryBLL
    {
        private CountryDAL countryDAL = new CountryDAL();

        private int currentPage = 1;
        private int pageSize = 10;

        // Search and filter parameters
        private string _searchKeyword = "";

        /// <summary>
        /// Lấy danh sách quốc gia với filter
        /// </summary>
        public IEnumerable<CountryDTO> GetCountries()
        {
            var allCountries = countryDAL.GetAll();

            // Apply search filter
            if (!string.IsNullOrWhiteSpace(_searchKeyword))
            {
                allCountries = allCountries.Where(c =>
                    (c.CountryName?.Contains(_searchKeyword, StringComparison.OrdinalIgnoreCase) ?? false) ||
                    (c.CountryCode?.Contains(_searchKeyword, StringComparison.OrdinalIgnoreCase) ?? false)
                );
            }

            // Apply pagination
            return allCountries
                .Skip((currentPage - 1) * pageSize)
                .Take(pageSize);
        }

        /// <summary>
        /// Lấy tổng số trang
        /// </summary>
        public int GetTotalPages()
        {
            var allCountries = countryDAL.GetAll();

            // Apply same filters
            if (!string.IsNullOrWhiteSpace(_searchKeyword))
            {
                allCountries = allCountries.Where(c =>
                    (c.CountryName?.Contains(_searchKeyword, StringComparison.OrdinalIgnoreCase) ?? false) ||
                    (c.CountryCode?.Contains(_searchKeyword, StringComparison.OrdinalIgnoreCase) ?? false)
                );
            }

            int totalRecords = allCountries.Count();
            return (int)Math.Ceiling(totalRecords / (double)pageSize);
        }

        public void SetSearchKeyword(string keyword)
        {
            _searchKeyword = keyword;
            currentPage = 1;
        }

        public void ClearFilters()
        {
            _searchKeyword = "";
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
        /// Thêm quốc gia mới
        /// </summary>
        public bool AddCountry(CountryDTO country, out string message)
        {
            if (countryDAL.IsCountryNameExists(country.CountryName))
            {
                message = "Tên quốc gia đã tồn tại";
                return false;
            }

            if (!string.IsNullOrWhiteSpace(country.CountryCode) && countryDAL.IsCountryCodeExists(country.CountryCode))
            {
                message = "Mã quốc gia đã tồn tại";
                return false;
            }

            int countryId = countryDAL.AddCountry(country);

            if (countryId > 0)
            {
                message = "Thêm quốc gia thành công";
                return true;
            }

            message = "Thêm quốc gia thất bại";
            return false;
        }

        /// <summary>
        /// Cập nhật quốc gia
        /// </summary>
        public bool UpdateCountry(CountryDTO country, out string message)
        {
            if (countryDAL.IsCountryNameExists(country.CountryName, country.CountryID))
            {
                message = "Tên quốc gia đã tồn tại";
                return false;
            }

            if (!string.IsNullOrWhiteSpace(country.CountryCode) && countryDAL.IsCountryCodeExists(country.CountryCode, country.CountryID))
            {
                message = "Mã quốc gia đã tồn tại";
                return false;
            }

            bool result = countryDAL.UpdateCountry(country);

            message = result ? "Cập nhật quốc gia thành công" : "Cập nhật quốc gia thất bại";
            return result;
        }

        /// <summary>
        /// Xóa quốc gia
        /// </summary>
        public bool DeleteCountry(int countryId, out string message)
        {
            // Kiểm tra xem quốc gia có đang được sử dụng không
            if (countryDAL.IsCountryInUse(countryId))
            {
                message = "Không thể xóa quốc gia đang được sử dụng trong phim";
                return false;
            }

            bool result = countryDAL.Delete(countryId);
            message = result ? "Xóa quốc gia thành công" : "Xóa quốc gia thất bại";
            return result;
        }

        /// <summary>
        /// Xóa nhiều quốc gia
        /// </summary>
        public bool DeleteMultipleCountries(List<int> countryIds, out string message)
        {
            int successCount = 0;
            int inUseCount = 0;

            foreach (int countryId in countryIds)
            {
                if (countryDAL.IsCountryInUse(countryId))
                {
                    inUseCount++;
                    continue;
                }

                if (countryDAL.Delete(countryId))
                {
                    successCount++;
                }
            }

            if (inUseCount > 0)
            {
                message = $"Đã xóa {successCount}/{countryIds.Count} quốc gia. {inUseCount} quốc gia đang được sử dụng không thể xóa.";
            }
            else
            {
                message = $"Đã xóa {successCount}/{countryIds.Count} quốc gia";
            }

            return successCount > 0;
        }

        /// <summary>
        /// Lấy thông tin quốc gia theo ID
        /// </summary>
        public CountryDTO? GetCountryById(int countryId)
        {
            return countryDAL.GetById(countryId);
        }

        public List<CountryDTO> GetAllCountries(out string message)
        {
            try
            {
                var countries = countryDAL.GetAll().ToList();
                message = "Success";
                return countries;
            }
            catch (Exception ex)
            {
                message = $"Error: {ex.Message}";
                return new List<CountryDTO>();
            }
        }
    }
}