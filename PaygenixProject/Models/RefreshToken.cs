using NewPayGenixAPI.Models;
using System.ComponentModel.DataAnnotations.Schema;

namespace PaygenixProject.Models
{
    public class RefreshToken
    {
        public int RefreshTokenID { get; set; }
        public int UserID { get; set; } // Foreign key to the Users table
        public string Token { get; set; }

        [Column(TypeName = "date")] // Specify the column type as "date"
        public DateTime Expires { get; set; }
        public bool IsRevoked { get; set; }
        public bool IsUsed { get; set; }

        // Navigation property
        public User User { get; set; }
    }
}
