
namespace NewPayGenixAPI.DTO
{
    public class PayrollDTO
    {
        public int PayrollID { get; set; }
        public int EmployeeID { get; set; }
        public decimal BasicSalary { get; set; }
        public decimal HRA { get; set; }
        public decimal LTA { get; set; }
        public decimal TravellingAllowance { get; set; }
        public decimal DA { get; set; }
        public decimal GrossPay { get; set; }
        public decimal PF { get; set; }
        public decimal TDS { get; set; }
        public decimal ESI { get; set; }
        public decimal Deduction { get; set; }
        public decimal NetPay { get; set; }
        public DateTime StartPeriod { get; set; }
        public DateTime EndPeriod { get; set; }
        public DateTime GeneratedDate { get; set; }

    }
}
