using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace NewPayGenixAPI.Models
{
    public class Payroll
    {
        [Key]
        public int PayrollID { get; set; }  // Primary Key

        [Required]
        public int EmployeeID { get; set; }  // Foreign Key to Employee

        [Required]
        [Column(TypeName = "decimal(10, 2)")]
        public decimal BasicSalary { get; set; } // Basic salary of the employee

        [Column(TypeName = "decimal(10, 2)")]
        public decimal HRA { get; set; }  // House Rent Allowance

        [Column(TypeName = "decimal(10, 2)")]
        public decimal LTA { get; set; }  // Leave Travel Allowance

        [Column(TypeName = "decimal(10, 2)")]
        public decimal TravellingAllowance { get; set; }  // Allowance for travel

        [Column(TypeName = "decimal(10, 2)")]
        public decimal DA { get; set; }  // Dearness Allowance

        [Column(TypeName = "decimal(10, 2)")]
        public decimal GrossPay { get; set; }  // Total pay before deductions

        [Column(TypeName = "decimal(10, 2)")]
        public decimal PF { get; set; }  // Provident Fund

        [Column(TypeName = "decimal(10, 2)")]
        public decimal TDS { get; set; }  // Tax Deducted at Source

        [Column(TypeName = "decimal(10, 2)")]
        public decimal ESI { get; set; } // Employee State Insurance

        [Column(TypeName = "decimal(10, 2)")]
        public decimal Deduction { get; set; }

        [Column(TypeName = "decimal(10, 2)")]
        public decimal TaxAmount { get; set; }

        [Column(TypeName = "decimal(10, 2)")]
        public decimal NetPay { get; set; }  // Final take-home pay

        [Required]
        [MaxLength(50)]
        public DateTime StartPeriod { get; set; }
        [Required]
        [MaxLength(50)]
        public DateTime EndPeriod { get; set; }

        public DateTime GeneratedDate { get; set; }

        //[ForeignKey("EmployeeID")]

        // Navigation property to Employe
        public Employee Employee { get; set; }

    }
}
