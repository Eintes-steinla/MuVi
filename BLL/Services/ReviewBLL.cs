using Muvi.DAL;
using MuVi.DTO.DTOs;
using System.Collections.Generic;

namespace MuVi.BLL
{
    public class ReviewBLL
    {
        private readonly ReviewDAL _reviewDAL = new ReviewDAL();

        public List<ReviewDTO> GetMovieReviews(int movieId) => _reviewDAL.GetByMovieId(movieId);

        public bool PostReview(ReviewDTO review, out string message)
        {
            // 1. Kiểm tra điểm đánh giá
            if (review.Rating < 1 || review.Rating > 10)
            {
                message = "Điểm đánh giá phải từ 1 đến 10.";
                return false;
            }

            // 2. Kiểm tra xem đã review chưa
            if (_reviewDAL.HasUserReviewed(review.UserID, review.MovieID))
            {
                message = "Bạn đã đánh giá bộ phim này rồi.";
                return false;
            }

            // 3. Lưu review
            bool result = _reviewDAL.Insert(review);
            message = result ? "Gửi đánh giá thành công." : "Gửi đánh giá thất bại.";
            return result;
        }

        public bool RemoveReview(int reviewId, out string message)
        {
            bool result = _reviewDAL.Delete(reviewId);
            message = result ? "Đã xóa bình luận." : "Xóa thất bại.";
            return result;
        }
    }
}