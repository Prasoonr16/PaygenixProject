using System.ComponentModel.DataAnnotations;

namespace PaygenixProject.Models
{
    public class AuditTrail
    {
        [Key]
        public int AuditID { get; set; } // Primary Key

        [Required]
        public string Action { get; set; } // The type of action performed (e.g., Login, Update, Delete)

        [Required]
        public string PerformedBy { get; set; } // Username or UserID of the person who performed the action

        [Required]
        [DataType(DataType.DateTime)]
        public DateTime Timestamp { get; set; } // When the action occurred

        [MaxLength(500)]
        public string Details { get; set; } // Additional details about the action

    }
}
