using EmployeeManagement.API.DTOs;
using EmployeeManagement.Model.Models;
using EmployeeManagement.Service.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EmployeeManagement.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EmployeeApiController : ControllerBase
    {
        private readonly IEmployeeService _employeeService;
        private readonly ILogger<EmployeeApiController> _logger;

        public EmployeeApiController(IEmployeeService employeeService, ILogger<EmployeeApiController> logger)
        {
            _employeeService = employeeService;
            _logger = logger;
        }

        // GET api/employee/list?searchString=John&title=Manager&page=1&pageSize=10
        [HttpGet("list")]
        public async Task<IActionResult> GetEmployeesList(string? searchString, int page = 1, int pageSize = 10)
        {
            _logger.LogInformation("API - Fetching all employees");

            var (employees, totalCount) = await _employeeService.GetEmployeesAsync(searchString, page, pageSize);
            _logger.LogInformation("Fetching all employees");
            var data = employees.Select(e => new EmployeeDto
            {
                EmployeeId = e.EmployeeId,
                Name = e.Name,
                SSN = e.SSN,
                DOB = e.DOB,
                Address = e.Address,
                City = e.City,
                State = e.State,
                Zip = e.Zip,
                Phone = e.Phone,
                JoinDate = e.JoinDate,
                ExitDate = e.ExitDate,
                Salaries = e.Salaries.Select(s => new EmployeeSalaryDto
                {
                    EmployeeSalaryId = s.EmployeeSalaryId,
                    EmployeeId = s.EmployeeId,
                    Title = s.Title,
                    Salary = s.Salary,
                    FromDate = s.FromDate,
                    ToDate = s.ToDate
                }).ToList()
            });

            return Ok(new EmployeeServiceResponse
            {
                Data = data.ToList(),
                TotalRecords = totalCount
            });
        }

        // POST api/employee/add
        [HttpPost("add")]
        public async Task<IActionResult> AddEmployee(EmployeeDto employeeDto)
        {
            var employee = new Employee
            {
                EmployeeId = employeeDto.EmployeeId,
                Name = employeeDto.Name,
                SSN = employeeDto.SSN,
                DOB = employeeDto.DOB,
                Address = employeeDto.Address,
                City = employeeDto.City,
                State = employeeDto.State,
                Zip = employeeDto.Zip,
                Phone = employeeDto.Phone,
                JoinDate = employeeDto.JoinDate,
                ExitDate = employeeDto.ExitDate,
                Salaries = employeeDto.Salaries.Select(s => new EmployeeSalary
                {
                    EmployeeSalaryId = s.EmployeeSalaryId,
                    EmployeeId = s.EmployeeId,
                    Title = s.Title,
                    Salary = s.Salary,
                    FromDate = s.FromDate,
                    ToDate = s.ToDate
                }).ToList()
            };

            var addedEmployee = await _employeeService.AddEmployeeAsync(employee);

            var addedDto = new EmployeeDto
            {
                Name = addedEmployee.Name,
                SSN = addedEmployee.SSN,
                DOB = addedEmployee.DOB,
                Address = addedEmployee.Address,
                City = addedEmployee.City,
                State = addedEmployee.State,
                Zip = addedEmployee.Zip,
                Phone = addedEmployee.Phone,
                JoinDate = addedEmployee.JoinDate,
                ExitDate = addedEmployee.ExitDate,
                Salaries = addedEmployee.Salaries.Select(s => new EmployeeSalaryDto
                {
                    EmployeeId = s.EmployeeId,
                    Title = s.Title,
                    Salary = s.Salary,
                    FromDate = s.FromDate,
                    ToDate = s.ToDate
                }).ToList()
            };

            return Ok(addedDto);
        }

        [HttpGet("titlelist")]
        public async Task<ActionResult<List<TitleSalaryDto>>> GetTitleSalaryList()
        {

            var titleSalaryStats = await _employeeService.GetTitleSalarySummaryAsync();

            var groupedData = titleSalaryStats
                .GroupBy(e => e.Title)
                .Select(group => new TitleSalaryDto
                {
                    Title = group.Key,
                    MinSalary = group.Average(e => e.MinSalary),
                    MaxSalary = group.Average(e => e.MaxSalary)
                })
                .ToList();

            return Ok(groupedData);
        }
    }
}