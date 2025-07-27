using EmployeeManagement.Model.Models;
using EmployeeManagement.Repository.Data;
using EmployeeManagement.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeManagement.Repository.Repositories
{
    public class TitleRepository : ITitleRepository
    {
        private readonly ApplicationDbContext _context;
        public TitleRepository(ApplicationDbContext context) => _context = context;

        public async Task<List<ViewTitleSalary>> GetTitleSalarySummaryAsync()
        {
            return await _context.EmployeeSalaries
                .GroupBy(x => x.Title)
                .Select(g => new ViewTitleSalary
                {
                    Title = g.Key,
                    MinSalary = g.Min(x => x.Salary),
                    MaxSalary = g.Max(x => x.Salary)
                })
                .ToListAsync();
        }
    }
}