using Muvi.DAL;
using MuVi.DTO.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MuVi.BLL
{
    public class ActorBLL
    {
        private ActorDAL actorDAL = new ActorDAL();

        private int currentPage = 1;
        private int pageSize = 10;

        // Search and filter parameters
        private string _searchKeyword = "";
        private string? _nationalityFilter = null;

        /// <summary>
        /// Lấy danh sách diễn viên với filter
        /// </summary>
        public IEnumerable<ActorDTO> GetActors()
        {
            var allActors = actorDAL.GetAll();

            // Apply search filter
            if (!string.IsNullOrWhiteSpace(_searchKeyword))
            {
                allActors = allActors.Where(a =>
                    (a.ActorName?.Contains(_searchKeyword, StringComparison.OrdinalIgnoreCase) ?? false) ||
                    (a.Nationality?.Contains(_searchKeyword, StringComparison.OrdinalIgnoreCase) ?? false) ||
                    (a.Bio?.Contains(_searchKeyword, StringComparison.OrdinalIgnoreCase) ?? false)
                );
            }

            // Apply nationality filter
            if (!string.IsNullOrEmpty(_nationalityFilter) && _nationalityFilter != "Tất cả")
            {
                allActors = allActors.Where(a => a.Nationality == _nationalityFilter);
            }

            // Apply pagination
            return allActors
                .Skip((currentPage - 1) * pageSize)
                .Take(pageSize);
        }

        /// <summary>
        /// Lấy tổng số trang
        /// </summary>
        public int GetTotalPages()
        {
            var allActors = actorDAL.GetAll();

            // Apply same filters
            if (!string.IsNullOrWhiteSpace(_searchKeyword))
            {
                allActors = allActors.Where(a =>
                    (a.ActorName?.Contains(_searchKeyword, StringComparison.OrdinalIgnoreCase) ?? false) ||
                    (a.Nationality?.Contains(_searchKeyword, StringComparison.OrdinalIgnoreCase) ?? false) ||
                    (a.Bio?.Contains(_searchKeyword, StringComparison.OrdinalIgnoreCase) ?? false)
                );
            }

            if (!string.IsNullOrEmpty(_nationalityFilter) && _nationalityFilter != "Tất cả")
            {
                allActors = allActors.Where(a => a.Nationality == _nationalityFilter);
            }

            int totalRecords = allActors.Count();
            return (int)Math.Ceiling(totalRecords / (double)pageSize);
        }

        public void SetSearchKeyword(string keyword)
        {
            _searchKeyword = keyword;
            currentPage = 1;
        }

        public void SetNationalityFilter(string? nationality)
        {
            _nationalityFilter = nationality;
            currentPage = 1;
        }

        public void ClearFilters()
        {
            _searchKeyword = "";
            _nationalityFilter = null;
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
        /// Thêm diễn viên mới
        /// </summary>
        public bool AddActor(ActorDTO actor, out string message)
        {
            if (actorDAL.IsActorNameExists(actor.ActorName))
            {
                message = "Tên diễn viên đã tồn tại";
                return false;
            }

            bool result = actorDAL.AddActor(actor);
            message = result ? "Thêm diễn viên thành công" : "Thêm diễn viên thất bại";
            return result;
        }

        /// <summary>
        /// Cập nhật diễn viên
        /// </summary>
        public bool UpdateActor(ActorDTO actor, out string message)
        {
            if (actorDAL.IsActorNameExists(actor.ActorName, actor.ActorID))
            {
                message = "Tên diễn viên đã tồn tại";
                return false;
            }

            bool result = actorDAL.UpdateActor(actor);
            message = result ? "Cập nhật diễn viên thành công" : "Cập nhật diễn viên thất bại";
            return result;
        }

        /// <summary>
        /// Xóa diễn viên
        /// </summary>
        public bool DeleteActor(int actorId, out string message)
        {
            bool result = actorDAL.Delete(actorId);
            message = result ? "Xóa diễn viên thành công" : "Xóa diễn viên thất bại";
            return result;
        }

        /// <summary>
        /// Xóa nhiều diễn viên
        /// </summary>
        public bool DeleteMultipleActors(List<int> actorIds, out string message)
        {
            int successCount = 0;
            foreach (int actorId in actorIds)
            {
                if (actorDAL.Delete(actorId))
                {
                    successCount++;
                }
            }

            message = $"Đã xóa {successCount}/{actorIds.Count} diễn viên";
            return successCount > 0;
        }

        /// <summary>
        /// Lấy danh sách quốc tịch duy nhất
        /// </summary>
        public IEnumerable<string> GetDistinctNationalities()
        {
            var allActors = actorDAL.GetAll();
            return allActors
                .Where(a => !string.IsNullOrEmpty(a.Nationality))
                .Select(a => a.Nationality!)
                .Distinct()
                .OrderBy(n => n);
        }
    }
}