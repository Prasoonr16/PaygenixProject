namespace NewPayGenixAPI.DTO
{
    public class ComplianceReportDTO
    {
        public int ReportID { get; set; }
        public DateTime ReportDate { get; set; }
        public int? EmployeeID { get; set; }
        public DateTime StartPeriod { get; set; }
        public DateTime EndPeriod { get; set; }
<<<<<<< HEAD

=======
>>>>>>> bab6098b2e91514a8a095c91f8d3fb5294db04ae
        public string ComplianceStatus { get; set; }
        public string IssuesFound { get; set; }
        public string ResolvedStatus { get; set; }
       
        public string Comments { get; set; }
    }
}
