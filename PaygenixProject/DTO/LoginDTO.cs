using System.ComponentModel.DataAnnotations;

namespace NewPayGenixAPI.DTO
{
    public class LoginDTO
    {
        [Required(ErrorMessage = "Username is required.")]
        public string Username { get; set; }

        [Required(ErrorMessage = "Password is required.")]
        public string Password { get; set; }


        [Required(ErrorMessage = "RoleID is required.")]
        public int RoleID { get; set; } // The role the user is trying to log in with
    }
}
