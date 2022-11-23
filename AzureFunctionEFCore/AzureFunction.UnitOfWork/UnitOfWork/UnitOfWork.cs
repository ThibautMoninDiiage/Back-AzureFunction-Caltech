using AzureFunction.Contract.Repositories;
using AzureFunction.Contract.UnitOfWork;
using AzureFunction.UnitOfWork.DbContextAZ;
using AzureFunction.UnitOfWork.Repositories;

namespace AzureFunction.UnitOfWork.UnitOfWork
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly DbContextServeur _context;
        private IRoleRepository? _roleRepository;
        private IUserRepository? _userRepository;

        public UnitOfWork(DbContextServeur context)
        {
            _context = context;
        }

        public IRoleRepository RoleRepository
        {
            get { return _roleRepository = _roleRepository ?? new RoleRepository(_context); }
        }

        public IUserRepository UserRepository
        {
            get { return _userRepository = _userRepository ?? new UserRepository(_context); }
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
