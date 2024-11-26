﻿using System.ComponentModel.DataAnnotations;

namespace NewPayGenixAPI.Models
{
    public class LeaveRequest
    {
        [Key]
        public int LeaveRequestID { get; set; }  // Primary Key

        [Required]
        public int EmployeeID { get; set; }  // Foreign Key to Employee

        [Required]
        public DateOnly StartDate { get; set; }

        [Required]
        public DateOnly EndDate { get; set; }

        [Required]
        [MaxLength(50)]
        public string LeaveType { get; set; }

        [MaxLength(50)]
        [Required(ErrorMessage = "Status is required.")]
        [RegularExpression(@"^(Approved|Not Approved)$", ErrorMessage = "Status must be either 'Approved' or 'Not Approved'.")]
        public string Status { get; set; }

        [DataType(DataType.Date)]

        public DateTime RequestDate { get; set; }
       
        [DataType(DataType.Date)]
        public DateTime? ApprovalDate { get; set; }

        //[ForeignKey("EmployeeID")]

        // Navigation property 
        public Employee Employee { get; set; }
    }
}
