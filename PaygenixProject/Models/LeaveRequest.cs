using System.ComponentModel.DataAnnotations;

namespace NewPayGenixAPI.Models
{
    public class LeaveRequest
    {
        [Key]
        public int LeaveRequestID { get; set; }  // Primary Key

        [Required]
        public int EmployeeID { get; set; }  // Foreign Key to Employee

        [Required]
        public DateTime StartDate { get; set; }

        [Required]
        public DateTime EndDate { get; set; }

        [Required]
        [MaxLength(50)]
        public string LeaveType { get; set; }

        [MaxLength(50)]
        [Required(ErrorMessage = "Status is required.")]
        [RegularExpression(@"^(Approved|Not Approved)$", ErrorMessage = "Status must be either 'Approved' or 'Not Approved'.")]
        public string Status { get; set; }

        public DateTime RequestDate { get; set; }
        public DateTime? ApprovalDate { get; set; }

        //[ForeignKey("EmployeeID")]

        // Navigation property 
        public Employee Employee { get; set; }
    }
}
