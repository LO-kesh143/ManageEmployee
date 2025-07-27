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
    public class GenericRepository<T> : IGenericRepository<T> where T : class
    {
        private readonly ApplicationDbContext _context;
        private readonly DbSet<T> _dbSet;

        public GenericRepository(ApplicationDbContext context)
        {
            _context = context;
            _dbSet = context.Set<T>();
        }

        public async Task<T> AddAsync(T entity)
        {
            await _dbSet.AddAsync(entity);
            //await _context.SaveChangesAsync();
            return entity;
        }

        public IQueryable<T> Query() => _dbSet.AsQueryable();

        //public async Task<IEnumerable<T>> GetAllAsync() =>
        //    await _dbSet.ToListAsync();

        //public async Task<T?> GetByIdAsync(int id) =>
        //    await _dbSet.FindAsync(id);

        //public async Task<T?> UpdateAsync(T entity)
        //{
        //    _dbSet.Update(entity);
        //    await _context.SaveChangesAsync();
        //    return entity;
        //}

        //public async Task<bool> DeleteAsync(int id)
        //{
        //    var entity = await GetByIdAsync(id);
        //    if (entity == null) return false;
        //    _dbSet.Remove(entity);
        //    await _context.SaveChangesAsync();
        //    return true;
        //}
    }
}