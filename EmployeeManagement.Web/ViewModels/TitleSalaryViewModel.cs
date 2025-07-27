namespace EmployeeManagement.Web.ViewModels
{
    public class TitleSalaryViewModel
    {
        public string Title { get; set; } = string.Empty;
        public decimal MinSalary { get; set; }
        public decimal MaxSalary { get; set; }
    }
}
