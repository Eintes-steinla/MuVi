using Muvi.DAL;
using MuVi.DTO.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MuVi.BLL
{
    public class ReviewBLL
    {
        private ReviewDAL reviewDAL = new ReviewDAL();
        private UserDAL userDAL = new UserDAL();
        private MovieDAL movieDAL = new MovieDAL();

        private int currentPage = 1;
        private int pageSize = 10;

        // Filter parameters
        private string _searchKeyword = "";
        private int? _ratingFilter = null;  // Filter theo điểm đánh giá
        private int? _userFilter = null;
        private int? _movieFilter = null;
        private int? _minLikes = null;

        /// <summary>
        /// Lấy danh sách đánh giá với filter và phân trang
        /// </summary>
        public IEnumerable<ReviewDTO> GetReviews()
        {
            var allReviews = reviewDAL.GetAll();

            // Apply search filter
            if (!string.IsNullOrWhiteSpace(_searchKeyword))
            {
                allReviews = allReviews.Where(r =>
                    (r.MovieTitle?.Contains(_searchKeyword, StringComparison.OrdinalIgnoreCase) ?? false) ||
                    (r.Username?.Contains(_searchKeyword, StringComparison.OrdinalIgnoreCase) ?? false) ||
                    (r.Comment?.Contains(_searchKeyword, StringComparison.OrdinalIgnoreCase) ?? false)
                );
            }

            // Apply rating filter
            if (_ratingFilter.HasValue)
            {
                allReviews = allReviews.Where(r => r.Rating == _ratingFilter.Value);
            }

            // Apply user filter
            if (_userFilter.HasValue)
            {
                allReviews = allReviews.Where(r => r.UserID == _userFilter.Value);
            }

            // Apply movie filter
            if (_movieFilter.HasValue)
            {
                allReviews = allReviews.Where(r => r.MovieID == _movieFilter.Value);
            }

            // Apply minimum likes filter
            if (_minLikes.HasValue)
            {
                allReviews = allReviews.Where(r => r.LikeCount >= _minLikes.Value);
            }

            // Apply pagination
            return allReviews
                .Skip((currentPage - 1) * pageSize)
                .Take(pageSize);
        }

        /// <summary>
        /// Lấy tổng số trang
        /// </summary>
        public int GetTotalPages()
        {
            var allReviews = reviewDAL.GetAll();

            // Apply same filters
            if (!string.IsNullOrWhiteSpace(_searchKeyword))
            {
                allReviews = allReviews.Where(r =>
                    (r.MovieTitle?.Contains(_searchKeyword, StringComparison.OrdinalIgnoreCase) ?? false) ||
                    (r.Username?.Contains(_searchKeyword, StringComparison.OrdinalIgnoreCase) ?? false) ||
                    (r.Comment?.Contains(_searchKeyword, StringComparison.OrdinalIgnoreCase) ?? false)
                );
            }

            if (_ratingFilter.HasValue)
            {
                allReviews = allReviews.Where(r => r.Rating == _ratingFilter.Value);
            }

            if (_userFilter.HasValue)
            {
                allReviews = allReviews.Where(r => r.UserID == _userFilter.Value);
            }

            if (_movieFilter.HasValue)
            {
                allReviews = allReviews.Where(r => r.MovieID == _movieFilter.Value);
            }

            if (_minLikes.HasValue)
            {
                allReviews = allReviews.Where(r => r.LikeCount >= _minLikes.Value);
            }

            int totalRecords = allReviews.Count();
            return (int)Math.Ceiling(totalRecords / (double)pageSize);
        }

        public void SetSearchKeyword(string keyword)
        {
            _searchKeyword = keyword;
            currentPage = 1;
        }

        public void SetRatingFilter(int? rating)
        {
            _ratingFilter = rating;
            currentPage = 1;
        }

        public void SetUserFilter(int? userId)
        {
            _userFilter = userId;
            currentPage = 1;
        }

        public void SetMovieFilter(int? movieId)
        {
            _movieFilter = movieId;
            currentPage = 1;
        }

        public void SetMinLikesFilter(int? minLikes)
        {
            _minLikes = minLikes;
            currentPage = 1;
        }

        public void ClearFilters()
        {
            _searchKeyword = "";
            _ratingFilter = null;
            _userFilter = null;
            _movieFilter = null;
            _minLikes = null;
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
        /// Thêm đánh giá mới
        /// </summary>
        public bool AddReview(ReviewDTO review, out string message)
        {
            // Kiểm tra user đã đánh giá phim chưa
            if (reviewDAL.HasUserReviewedMovie(review.UserID, review.MovieID))
            {
                message = "Người dùng đã đánh giá phim này rồi";
                return false;
            }

            bool result = reviewDAL.AddReview(review);
            message = result ? "Thêm đánh giá thành công" : "Thêm đánh giá thất bại";
            return result;
        }

        /// <summary>
        /// Cập nhật đánh giá
        /// </summary>
        public bool UpdateReview(ReviewDTO review, out string message)
        {
            bool result = reviewDAL.UpdateReview(review);
            message = result ? "Cập nhật đánh giá thành công" : "Cập nhật đánh giá thất bại";
            return result;
        }

        /// <summary>
        /// Xóa đánh giá
        /// </summary>
        public bool DeleteReview(int reviewId, out string message)
        {
            bool result = reviewDAL.Delete(reviewId);
            message = result ? "Xóa đánh giá thành công" : "Xóa đánh giá thất bại";
            return result;
        }

        /// <summary>
        /// Xóa nhiều đánh giá
        /// </summary>
        public bool DeleteMultipleReviews(List<int> reviewIds, out string message)
        {
            int successCount = 0;
            foreach (int reviewId in reviewIds)
            {
                if (reviewDAL.Delete(reviewId))
                {
                    successCount++;
                }
            }

            message = $"Đã xóa {successCount}/{reviewIds.Count} đánh giá";
            return successCount > 0;
        }

        /// <summary>
        /// Lấy danh sách users cho filter
        /// </summary>
        public IEnumerable<UserDTO> GetAllUsers()
        {
            return userDAL.GetAll();
        }

        /// <summary>
        /// Lấy danh sách phim cho filter
        /// </summary>
        public IEnumerable<MovieDTO> GetAllMovies()
        {
            return movieDAL.GetAll();
        }

        /// <summary>
        /// Lấy thông tin đánh giá theo ID
        /// </summary>
        public ReviewDTO? GetReviewById(int reviewId)
        {
            return reviewDAL.GetById(reviewId);
        }

        /// <summary>
        /// Tăng/giảm like count
        /// </summary>
        public bool UpdateLikeCount(int reviewId, bool isLike, out string message)
        {
            int increment = isLike ? 1 : -1;
            bool result = reviewDAL.UpdateLikeCount(reviewId, increment);
            message = result ? "Cập nhật like thành công" : "Cập nhật like thất bại";
            return result;
        }

        // Thêm vào trong class ReviewBLL
        public List<ReviewDTO> GetReviewsByMovie(int movieId, out string message)
        {
            try
            {
                var reviews = reviewDAL.GetAll()
                    .Where(r => r.MovieID == movieId)
                    .OrderByDescending(r => r.CreatedAt)
                    .ToList();
                message = "Thành công";
                return reviews;
            }
            catch (Exception ex)
            {
                message = $"Lỗi: {ex.Message}";
                return new List<ReviewDTO>();
            }
        }
    }
}