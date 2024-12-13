using System.Runtime.CompilerServices;

namespace NewPayGenixAPI.DTO
{
    public class UserDTO
    {
        public int UserID { get; set; }
        public string Username { get; set; }
        public int RoleID { get; set; }
        public string Email { get; set; }
        public string? PasswordHash { get; set; }

        public DateTime CreatedDate { get; set; }
    }
}
