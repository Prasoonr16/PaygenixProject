using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NewPayGenixAPI.Models
{
    public class User
    {
        [Key]
        [Required(ErrorMessage = "UserID is required")]
        public int UserID { get; set; }  // Primary Key

        [Required]
        [MaxLength(100)]
        public string? Username { get; set; }

        [Required(ErrorMessage = "Password is required.")]
        [StringLength(100, MinimumLength = 8, ErrorMessage = "Password must be between 8 and 100 characters.")]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d).+$", ErrorMessage = "Password must contain at least one uppercase letter, one lowercase letter, and one number.")]
        public string? PasswordHash { get; set; }

        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Invalid Email Address.")]
        public string Email { get; set; }

        [Required]
        public int RoleID { get; set; }  // Foreign Key to Role

        [Column(TypeName = "date")] // Specify the column type as "date"
        public DateTime CreatedDate { get; set; }
        
       

        //[ForeignKey("RoleID")]

        //Navigation Properties
        public Role Role { get; set; } // Many to one
        public Employee Employee { get; set; } //One to one 
    }
}

