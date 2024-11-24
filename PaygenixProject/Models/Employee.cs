using System.ComponentModel.DataAnnotations;

namespace NewPayGenixAPI.Models
{
    public class Employee
    {
        [Key]
        public int EmployeeID { get; set; }  // Primary Key

        [Required]
        [MaxLength(50)]
        public string FirstName { get; set; }

        [Required]
        [MaxLength(50)]
        public string LastName { get; set; }

        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Invalid Email Address.")]
        public string Email { get; set; }

        [MaxLength(10)]
        [Required(ErrorMessage = "Phone number is required.")]
        [Phone(ErrorMessage = "Invalid phone number.")]
        public string PhoneNumber { get; set; }

        [MaxLength(50)]
        public string Position { get; set; }

        [MaxLength(50)]
        public string Department { get; set; }

        public DateTime HireDate { get; set; }

        public int? UserID { get; set; }  // Foreign Key to User 

        //[ForeignKey("UserID")]

        //Navigation Property
        public User User { get; set; }
        public ICollection<Payroll> Payrolls { get; set; } //one to many relationship
        public ICollection<LeaveRequest> LeaveRequests { get; set; }
        public ICollection<EmployeeBenefit> EmployeeBenefits { get; set; }
    }
}

