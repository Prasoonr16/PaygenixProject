﻿using System.ComponentModel.DataAnnotations;
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

        public DateTime StartPeriod{ get; set; }

        [MaxLength(50)]
       
        [Required]
        [Column(TypeName = "date")] // Specify the column type as "date"
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
