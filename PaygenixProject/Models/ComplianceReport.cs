using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NewPayGenixAPI.Models
{
    public class ComplianceReport
    {
        [Key]
        public int ReportID { get; set; }  // Primary Key
        [Column(TypeName = "date")] // Specify the column type as "date"
        public DateTime ReportDate { get; set; }
        public int? EmployeeID { get; set; }  // Foreign Key to Employee

        [Required]
        [Column(TypeName = "date")] // Specify the column type as "date"
<<<<<<< HEAD

        public DateTime StartPeriod{ get; set; }

        [MaxLength(50)]
        [Column(TypeName = "date")] // Specify the column type as "date"

=======
        public DateTime StartPeriod { get; set; }
       
        [Required]
        [Column(TypeName = "date")] // Specify the column type as "date"
>>>>>>> bab6098b2e91514a8a095c91f8d3fb5294db04ae
        public DateTime EndPeriod { get; set; }

        [MaxLength(50)]
        public string ComplianceStatus { get; set; }


        public string IssuesFound { get; set; }


        public string ResolvedStatus { get; set; }

        public string Comments { get; set; }

        //Navigation Properties
        public Employee Employee { get; set; }
    }
}
