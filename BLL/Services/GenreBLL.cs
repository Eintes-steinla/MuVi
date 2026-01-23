using Muvi.DAL;
using MuVi.DTO.DTOs;
using System.Collections.Generic;

namespace MuVi.BLL
{
    public class GenreBLL
    {
        private readonly GenreDAL _genreDAL = new GenreDAL();

        public List<GenreDTO> GetGenres() => _genreDAL.GetAll();

        // Thêm vào trong class GenreBLL
        public List<GenreDTO> GetAllGenres()
        {
            return _genreDAL.GetAll();
        }
        public bool AddGenre(GenreDTO genre, out string message)
        {
            if (string.IsNullOrWhiteSpace(genre.GenreName))
            {
                message = "Tên thể loại không được để trống.";
                return false;
            }

            if (_genreDAL.IsGenreNameExists(genre.GenreName))
            {
                message = "Thể loại này đã tồn tại.";
                return false;
            }

            bool result = _genreDAL.Insert(genre);
            message = result ? "Thêm thành công." : "Thêm thất bại.";
            return result;
        }

        public bool UpdateGenre(GenreDTO genre, out string message)
        {
            bool result = _genreDAL.Update(genre);
            message = result ? "Cập nhật thành công." : "Cập nhật thất bại.";
            return result;
        }

        public bool DeleteGenre(int id, out string message)
        {
            // Kiểm tra xem có phim nào đang thuộc thể loại này không trước khi xóa
            // Nếu có MovieCategory liên kết, SQL sẽ báo lỗi Foreign Key
            try
            {
                bool result = _genreDAL.Delete(id);
                message = result ? "Đã xóa thể loại." : "Không tìm thấy thể loại.";
                return result;
            }
            catch
            {
                message = "Không thể xóa thể loại này vì đang có phim thuộc về nó.";
                return false;
            }
        }
    }
}