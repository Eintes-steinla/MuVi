namespace MuVi.DTO.DTOs
{
    public class MovieDTO
    {
        public int MovieID { get; set; }
        public string Title { get; set; } = "";
        public string MovieType { get; set; } = "";
        public int CountryID { get; set; }
        public string? Director { get; set; }
        public int ReleaseYear { get; set; }
        public string? Description { get; set; }
        public string? PosterPath { get; set; }
        public string? TrailerUrl { get; set; }
        public string? VideoPath { get; set; }
        public int Duration { get; set; }
        public int TotalEpisodes { get; set; }
        public int Rating { get; set; }
        public int ViewCount { get; set; }
        public string? Status { get; set; }
    }
}
