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

        // Thêm vào trong class MovieBLL
        public List<MovieDTO> GetTopMoviesByViewCount(int count)
        {
            // Lấy toàn bộ danh sách và dùng LINQ để sắp xếp theo lượt xem
            return _movieDAL.GetAll()
                .OrderByDescending(m => m.ViewCount)
                .Take(count)
                .ToList();
        }

        public List<MovieDTO> GetNewReleases(int count)
        {
            // Sắp xếp theo năm phát hành và ID mới nhất
            return _movieDAL.GetAll()
                .OrderByDescending(m => m.ReleaseYear)
                .ThenByDescending(m => m.MovieID)
                .Take(count)
                .ToList();
        }

        public List<MovieDTO> SearchMovies(string keyword)
        {
            if (string.IsNullOrWhiteSpace(keyword)) return new List<MovieDTO>();
            return _movieDAL.SearchByTitle(keyword);
        }

        public List<MovieDTO> GetMoviesByType(string movieType)
        {
            // movieType có thể là "Phim lẻ" hoặc "Phim bộ" dựa trên cột MovieType trong DB
            return _movieDAL.GetAll()
                .Where(m => m.MovieType == movieType)
                .ToList();
        }

        public List<MovieDTO> GetMoviesByGenre(int genreId)
        {
            // Lưu ý: Hàm này cần Join với bảng MovieCategory trong SQL
            // Tạm thời nếu DAL chưa có hàm chuyên biệt, ta có thể dùng DAL để lấy 
            // Nhưng tốt nhất là nên viết một hàm GetByGenreId trong MovieDAL
            return _movieDAL.GetAll(); // Bạn nên bổ sung query Join ở tầng DAL sau
        }
    }
}