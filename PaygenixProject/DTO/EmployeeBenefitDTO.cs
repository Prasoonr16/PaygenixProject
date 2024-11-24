namespace NewPayGenixAPI.DTO
{
    public class EmployeeBenefitDTO
    {
        public int EmployeeBenefitID { get; set; }
        public int EmployeeID { get; set; }
        public int BenefitID { get; set; }
        public DateTime EnrolledDate { get; set; }
    }
}
