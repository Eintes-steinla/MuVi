using Muvi.DAL;
using MuVi.DTO.DTOs;
using System;
using System.Collections.Generic;

namespace MuVi.BLL
{
    public class MovieBLL
    {
        private readonly MovieDAL _movieDAL = new MovieDAL();

        public List<MovieDTO> GetListMovies() => _movieDAL.GetAll();

        public MovieDTO? GetMovieDetail(int id) => _movieDAL.GetById(id);

        public bool SaveMovie(MovieDTO movie, out string message)
        {
            // Kiểm tra nghiệp vụ
            if (string.IsNullOrWhiteSpace(movie.Title))
            {
                message = "Tên phim không được để trống.";
                return false;
            }

            if (movie.ReleaseYear < 1900 || movie.ReleaseYear > DateTime.Now.Year + 5)
            {
                message = "Năm phát hành không hợp lệ.";
                return false;
            }

            bool result;
            if (movie.MovieID == 0)
            {
                result = _movieDAL.Insert(movie);
                message = result ? "Thêm phim mới thành công." : "Lỗi khi lưu phim.";
            }
            else
            {
                result = _movieDAL.Update(movie);
                message = result ? "Cập nhật thông tin phim thành công." : "Cập nhật thất bại.";
            }
            return result;
        }

        public bool DeleteMovie(int id, out string message)
        {
            // Có thể thêm logic kiểm tra xem phim có đang được xem nhiều không trước khi xóa
            bool result = _movieDAL.Delete(id);
            message = result ? "Đã xóa phim khỏi hệ thống." : "Không tìm thấy phim để xóa.";
            return result;
        }
    }
}