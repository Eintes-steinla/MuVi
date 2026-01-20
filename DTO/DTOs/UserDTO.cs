namespace MuVi.DTO.DTOs
{
    public class UserDTO
    {
        public int UserID { get; set; }
        public string Username { get; set; } = "";
        public string Password { get; set; } = "";
        public string? Email { get; set; }
        public string? DateOfBirth { get; set; }
        public string? Avatar { get; set; }
        public string Role { get; set; } = "User";
        public bool IsActive { get; set; }
        public string? LastLogin { get; set; }
    }

}
