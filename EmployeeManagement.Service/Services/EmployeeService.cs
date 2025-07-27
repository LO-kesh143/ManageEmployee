using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EmployeeManagement.Model.Models;
using EmployeeManagement.Repository.Data;
using EmployeeManagement.Repository.Interfaces;
using EmployeeManagement.Service.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace EmployeeManagement.Service.Services
{
    public class EmployeeService : IEmployeeService
    {
        private readonly IGenericRepository<Employee> _empRepository;
        private readonly IGenericRepository<EmployeeSalary> _empSalaryRepository;
        private readonly ITitleRepository _titleRepository;
        private readonly ILogger<EmployeeService> _logger;
        private readonly ApplicationDbContext _context;

        public EmployeeService(IGenericRepository<Employee> genRepository, 
            ITitleRepository titleRepository, ILogger<EmployeeService> logger,
            ApplicationDbContext context, IGenericRepository<EmployeeSalary> empSalaryRepository)
        {
            _empRepository = genRepository;
            _titleRepository = titleRepository;
            _logger = logger;
            _context = context;
            _empSalaryRepository = empSalaryRepository;
        }

        public async Task<(IEnumerable<Employee> Employees, int TotalCount)> GetEmployeesAsync(string? searchString, int page, int pageSize)
        {
            try
            {
                var queryable = _empRepository.Query().Include(e => e.Salaries).AsQueryable();

                if (!string.IsNullOrWhiteSpace(searchString))
                    queryable = queryable.Where(e =>
                        e.Name.Contains(searchString) ||
                        e.Salaries.Any(s => s.Title.Contains(searchString))
                    );

                var totalCount = await queryable.CountAsync();

                var employees = await queryable
                    .OrderBy(e => e.EmployeeId)
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();

                return (employees, totalCount);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while fetching employee list (search: {Search}, page: {Page}, pageSize: {PageSize})", searchString, page, pageSize);

                throw new Exception("An error occurred while retrieving employee data. Please try again later.");
            }
        }

        public async Task<Employee> AddEmployeeAsync(Employee employee)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                var exists = await _empRepository.Query().AnyAsync(e => e.SSN == employee.SSN);
                if (exists)
                    throw new ApplicationException("Employee already exists");

                var createdEmployee = await _empRepository.AddAsync(employee);
                
                foreach (var salary in employee.Salaries)
                {
                    salary.EmployeeId = createdEmployee.EmployeeId;
                    await _empSalaryRepository.AddAsync(salary);
                }

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return createdEmployee;
            }
            catch (ApplicationException appEx)
            {
                await transaction.RollbackAsync();
                _logger.LogWarning(appEx, "Duplicate SSN detected while adding employee (SSN: {SSN})", employee.SSN);
                throw;
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex,
                    "Error occurred while adding employee (Name: {Name}, SSN: {SSN}, JoinDate: {JoinDate})",
                    employee.Name, employee.SSN, employee.JoinDate);

                throw new Exception("An unexpected error occurred while adding employee. Please try again later.");
            }
        }

        public async Task<List<ViewTitleSalary>> GetTitleSalarySummaryAsync()
        {
            try
            {
                var salaries = await _titleRepository.GetTitleSalarySummaryAsync();

                var result = salaries
                    .GroupBy(s => s.Title)
                    .Select(g => new ViewTitleSalary
                    {
                        Title = g.Key,
                        MinSalary = g.Min(x => x.MinSalary),
                        MaxSalary = g.Max(x => x.MaxSalary)
                    })
                    .ToList();

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while fetching title salary summary.");

                throw new Exception("An unexpected error occurred while retrieving salary summary. Please try again later.");
            }
        }
    }
}