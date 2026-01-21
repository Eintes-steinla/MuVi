using Muvi.DAL;
using MuVi.DTO.DTOs;
using System;
using System.Collections.Generic;

namespace MuVi.BLL
{
    public class ActorBLL
    {
        private readonly ActorDAL _actorDAL = new ActorDAL();

        public List<ActorDTO> GetListActors() => _actorDAL.GetAll();

        public ActorDTO? GetDetailActor(int id) => _actorDAL.GetById(id);

        public bool SaveActor(ActorDTO actor, out string message)
        {
            // Validation cơ bản
            if (string.IsNullOrWhiteSpace(actor.ActorName))
            {
                message = "Tên diễn viên là bắt buộc.";
                return false;
            }

            bool result;
            if (actor.ActorID == 0) // Thêm mới
            {
                result = _actorDAL.Insert(actor);
                message = result ? "Thêm diễn viên thành công." : "Không thể thêm diễn viên.";
            }
            else // Cập nhật
            {
                result = _actorDAL.Update(actor);
                message = result ? "Cập nhật thông tin diễn viên thành công." : "Cập nhật thất bại.";
            }

            return result;
        }

        public bool RemoveActor(int id, out string message)
        {
            // Lưu ý: Trong DB của bạn, bảng MovieCast có khóa ngoại tham chiếu tới ActorID
            // Nếu diễn viên đã đóng phim, việc xóa sẽ bị lỗi do ràng buộc CASCADE (hoặc NO ACTION)
            try
            {
                bool result = _actorDAL.Delete(id);
                message = result ? "Đã xóa diễn viên khỏi hệ thống." : "Diễn viên không tồn tại.";
                return result;
            }
            catch (Exception ex)
            {
                message = "Không thể xóa vì diễn viên này đã có trong danh sách phim!";
                return false;
            }
        }

        public List<ActorDTO> FindActors(string keyword)
        {
            if (string.IsNullOrWhiteSpace(keyword)) return new List<ActorDTO>();
            return _actorDAL.SearchByName(keyword);
        }
    }
}