namespace NewPayGenixAPI.DTO
{
    public class EmployeeDTO
    {
        public int EmployeeID { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string Position { get; set; }
        public string Department { get; set; }
        public DateTime HireDate { get; set; }
        public string ActiveStatus { get; set; }

        public int? ManagerUserID { get; set; }
        public int? UserID { get; set; } 

    }
}
