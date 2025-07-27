using EmployeeManagement.Model.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeManagement.Repository.Interfaces
{
    public interface IGenericRepository<T> where T : class
    {
        Task<T> AddAsync(T entity);
        IQueryable<T> Query();
        //Task<IEnumerable<T>> GetAllAsync();
        //Task<T?> GetByIdAsync(int id);
        //Task<T?> UpdateAsync(T entity);
        //Task<bool> DeleteAsync(int id);
    }
}