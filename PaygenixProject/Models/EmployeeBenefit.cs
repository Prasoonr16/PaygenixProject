using System.ComponentModel.DataAnnotations;

namespace NewPayGenixAPI.Models
{
    public class EmployeeBenefit
    {
        [Key]
        public int EmployeeBenefitID { get; set; }  // Primary Key

        [Required]
        public int EmployeeID { get; set; }  // Foreign Key to Employee

        [Required]
        public int BenefitID { get; set; }  // Foreign Key to Benefit

        public DateTime EnrolledDate { get; set; }

        // Navigation Properties
        public Employee Employee { get; set; }
        public Benefit Benefit { get; set; }
    }
}
