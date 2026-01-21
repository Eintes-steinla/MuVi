using Muvi.DAL;
using MuVi.DTO.DTOs;
using System.Collections.Generic;

namespace MuVi.BLL
{
    public class MovieCastBLL
    {
        private readonly MovieCastDAL _castDAL = new MovieCastDAL();

        public List<MovieCastDTO> GetMovieCasts(int movieId)
        {
            return _castDAL.GetCastByMovie(movieId);
        }

        public bool AddActorToMovie(MovieCastDTO cast, out string message)
        {
            // Kiểm tra xem đã tồn tại bản ghi này chưa để tránh lỗi Primary Key
            var currentCast = _castDAL.GetCastByMovie(cast.MovieID);
            if (currentCast.Exists(x => x.ActorID == cast.ActorID))
            {
                message = "Diễn viên này đã được thêm vào phim trước đó.";
                return false;
            }

            bool result = _castDAL.Insert(cast);
            message = result ? "Thêm vai diễn thành column công." : "Thêm vai diễn thất bại.";
            return result;
        }

        public bool RemoveActorFromMovie(int movieId, int actorId, out string message)
        {
            bool result = _castDAL.Delete(movieId, actorId);
            message = result ? "Đã xóa diễn viên khỏi bộ phim." : "Lỗi: Không tìm thấy dữ liệu để xóa.";
            return result;
        }
    }
}