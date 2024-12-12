using System.ComponentModel.DataAnnotations;

namespace PaygenixProject.DTO
{
    public class ResetPasswordDTO
    {
        [Required]
        public int UserID { get; set; } // User's ID for verification

        [Required]
        public string ExistingPassword { get; set; } // Existing password for verification

        [Required]
        [StringLength(100, MinimumLength = 8, ErrorMessage = "Password must be at least 8 characters long.")]
        public string NewPassword { get; set; } // New password to be set
    }
}
