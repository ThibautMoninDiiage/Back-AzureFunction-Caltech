using AzureFunction.Repository.Interfaces;
using AzureFunction.Repository;
using AzureFunction.UnitOfWork.Interfaces;
using Microsoft.EntityFrameworkCore;
using AzureFunction.Context.Models;
using AzureFunction.Context.DbContextAZ;

namespace AzureFunction.UnitOfWork
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly DbContextServeur _context;
        private IRoleRepository? _roleRepository;

        public UnitOfWork(DbContextServeur context)
        {
            _context = context;
        }

        public IRoleRepository RoleRepository
        {
            get { return _roleRepository = _roleRepository ?? new RoleRepository(_context); }
        }

        public void Commit()
        {
            _context.SaveChanges();
        }

        public async Task CommitAsync()
        {
            await _context.SaveChangesAsync();
        }

        public void Rollback()
        {
            _context.Dispose();
        }

        public async Task RollbackAsync()
        {
            await _context.DisposeAsync();
        }
    }
}
