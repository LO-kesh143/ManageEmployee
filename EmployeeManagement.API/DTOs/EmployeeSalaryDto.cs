namespace EmployeeManagement.API.DTOs
{
    public class EmployeeSalaryDto
    {
        public int EmployeeSalaryId { get; set; }
        public int EmployeeId { get; set; }
        public string Title { get; set; } = string.Empty;
        public decimal Salary { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime? ToDate { get; set; }
    }
}
