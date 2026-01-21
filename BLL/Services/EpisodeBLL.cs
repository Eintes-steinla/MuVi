using Muvi.DAL;
using MuVi.DTO.DTOs;
using System.Collections.Generic;

namespace MuVi.BLL
{
    public class EpisodeBLL
    {
        private readonly EpisodeDAL _episodeDAL = new EpisodeDAL();

        public List<EpisodeDTO> GetEpisodesByMovie(int movieId) => _episodeDAL.GetByMovieId(movieId);

        public bool SaveEpisode(EpisodeDTO episode, out string message)
        {
            if (episode.EpisodeNumber <= 0)
            {
                message = "Số tập phải lớn hơn 0.";
                return false;
            }

            if (string.IsNullOrWhiteSpace(episode.VideoPath))
            {
                message = "Đường dẫn video không được để trống.";
                return false;
            }

            bool result;
            if (episode.EpisodeID == 0)
            {
                // Kiểm tra xem số tập này đã tồn tại trong phim này chưa
                var existingEpisodes = _episodeDAL.GetByMovieId(episode.MovieID);
                if (existingEpisodes.Exists(e => e.EpisodeNumber == episode.EpisodeNumber))
                {
                    message = $"Tập {episode.EpisodeNumber} đã tồn tại trong bộ phim này.";
                    return false;
                }

                result = _episodeDAL.Insert(episode);
                message = result ? "Thêm tập phim thành công." : "Thêm thất bại.";
            }
            else
            {
                result = _episodeDAL.Update(episode);
                message = result ? "Cập nhật thành công." : "Cập nhật thất bại.";
            }
            return result;
        }

        public bool DeleteEpisode(int id, out string message)
        {
            bool result = _episodeDAL.Delete(id);
            message = result ? "Đã xóa tập phim." : "Xóa thất bại.";
            return result;
        }

        public void IncreaseView(int episodeId)
        {
            _episodeDAL.IncrementViewCount(episodeId);
        }
    }
}