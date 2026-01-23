using Muvi.DAL;
using MuVi.DTO.DTOs;
using System.Collections.Generic;
using System.Linq;

namespace MuVi.BLL
{
    public class FavoriteBLL
    {
        private readonly FavoriteDAL _favoriteDAL = new FavoriteDAL();

        // Đổi tên từ GetUserFavorites thành GetFavoritesByUser để hết lỗi
        public List<MovieDTO> GetFavoritesByUser(int userId)
        {
            // 1. Lấy danh sách FavoriteDTO từ DAL (chứa cả thông tin phim nhờ cú pháp JOIN)
            var favoriteDtos = _favoriteDAL.GetFavoritesByUserId(userId);

            // 2. Chuyển đổi (Map) sang List<MovieDTO> để đồng bộ với ViewModel
            return favoriteDtos.Select(f => new MovieDTO
            {
                MovieID = f.MovieID,
                Title = f.Title,
                PosterPath = f.PosterPath,
                ReleaseYear = f.ReleaseYear
                // Copy thêm các thuộc tính khác nếu cần thiết
            }).ToList();
        }

        // Đổi tên từ ToggleFavorite thành AddToFavorites để hết lỗi
        // Lưu ý: Hàm này sẽ tự động thêm nếu chưa có, và xóa nếu đã có (Toggle)
        public bool AddToFavorites(int userId, int movieId)
        {
            if (_favoriteDAL.IsFavorite(userId, movieId))
            {
                // Nếu đã thích rồi thì xóa đi
                return _favoriteDAL.Delete(userId, movieId);
            }
            else
            {
                // Nếu chưa thích thì thêm mới
                return _favoriteDAL.Insert(userId, movieId);
            }
        }

        public bool CheckIsFavorite(int userId, int movieId) => _favoriteDAL.IsFavorite(userId, movieId);
    }
}