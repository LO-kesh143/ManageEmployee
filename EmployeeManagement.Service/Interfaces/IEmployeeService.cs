using EmployeeManagement.Model.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeManagement.Service.Interfaces
{
    public interface IEmployeeService
    {
        Task<(IEnumerable<Employee> Employees, int TotalCount)> GetEmployeesAsync(string? searchString, int page, int pageSize);
        Task<Employee> AddEmployeeAsync(Employee employee);
        Task<List<ViewTitleSalary>> GetTitleSalarySummaryAsync();
    }
}