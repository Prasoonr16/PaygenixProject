using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

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

        [Column(TypeName = "date")] // Specify the column type as "date"
        public DateTime EnrolledDate { get; set; }

        // Navigation Properties
        public Employee Employee { get; set; }
        public Benefit Benefit { get; set; }
    }
}
