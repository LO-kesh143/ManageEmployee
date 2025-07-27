namespace EmployeeManagement.Web.ViewModels
{
    public class EmployeeApiResponse
    {
        public int TotalRecords { get; set; }
        public List<EmployeeViewModel> Data { get; set; } = new();
    }
}
