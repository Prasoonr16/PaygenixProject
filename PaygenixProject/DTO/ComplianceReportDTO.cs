namespace NewPayGenixAPI.DTO
{
    public class ComplianceReportDTO
    {
        public int ReportID { get; set; }
        public DateTime ReportDate { get; set; }
        public int? EmployeeID { get; set; }
        public DateTime PayrollPeriod { get; set; }
        public string ComplianceStatus { get; set; }
        public string IssuesFound { get; set; }
        public string ResolvedStatus { get; set; }
       
        public string Comments { get; set; }
    }
}
