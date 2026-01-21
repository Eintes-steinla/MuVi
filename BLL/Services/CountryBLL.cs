using Muvi.DAL;
using MuVi.DTO.DTOs;

namespace MuVi.BLL
{
    public class CountryBLL
    {
        // Khởi tạo đối tượng DAL để giao tiếp với Database
        private readonly CountryDAL _countryDAL = new CountryDAL();

        /// <summary>
        /// Lấy danh sách tất cả quốc gia
        /// </summary>
        public List<CountryDTO> GetAllCountries()
        {
            return _countryDAL.GetAll();
        }

        /// <summary>
        /// Lấy thông tin chi tiết một quốc gia
        /// </summary>
        public CountryDTO? GetCountryById(int id)
        {
            if (id <= 0) return null;
            return _countryDAL.GetById(id);
        }

        /// <summary>
        /// Xử lý thêm mới quốc gia
        /// </summary>
        public bool AddCountry(CountryDTO country, out string message)
        {
            // 1. Kiểm tra dữ liệu đầu vào cơ bản
            if (string.IsNullOrWhiteSpace(country.CountryName))
            {
                message = "Tên quốc gia không được để trống";
                return false;
            }

            // 2. Kiểm tra trùng tên quốc gia
            if (_countryDAL.IsCountryNameExists(country.CountryName))
            {
                message = "Tên quốc gia này đã tồn tại trong hệ thống";
                return false;
            }

            // 3. Gọi DAL để lưu
            bool result = _countryDAL.Insert(country);

            message = result ? "Thêm quốc gia thành công" : "Đã xảy ra lỗi khi thêm dữ liệu";
            return result;
        }

        /// <summary>
        /// Xử lý cập nhật thông tin quốc gia
        /// </summary>
        public bool UpdateCountry(CountryDTO country, out string message)
        {
            if (country.CountryID <= 0)
            {
                message = "ID quốc gia không hợp lệ";
                return false;
            }

            bool result = _countryDAL.Update(country);
            message = result ? "Cập nhật thành công" : "Cập nhật thất bại";
            return result;
        }

        /// <summary>
        /// Xử lý xóa quốc gia
        /// </summary>
        public bool DeleteCountry(int id, out string message)
        {
            // Có thể thêm check ở đây: Nếu quốc gia đang có Phim thuộc về thì không cho xóa
            // (Hiện tại ta làm cơ bản trước)

            bool result = _countryDAL.Delete(id);
            message = result ? "Xóa quốc gia thành công" : "Xóa thất bại hoặc quốc gia không tồn tại";
            return result;
        }
    }
}