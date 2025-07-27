namespace EmployeeManagement.API.DTOs
{
    public class EmployeeServiceResponse
    {
        public int TotalRecords { get; set; }
        public List<EmployeeDto> Data { get; set; } = new();
    }
}
