namespace EmployeeManagement.API.DTOs
{
    public class TitleSalaryDto
    {
        public string Title { get; set; } = string.Empty;
        public decimal MinSalary { get; set; }
        public decimal MaxSalary { get; set; }
    }
}
