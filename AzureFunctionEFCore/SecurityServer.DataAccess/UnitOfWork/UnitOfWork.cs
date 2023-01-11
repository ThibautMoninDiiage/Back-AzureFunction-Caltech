using SecurityServer.DataAccess.Repositories;
using SecurityServer.Contract.Repositories;
using SecurityServer.Contract.UnitOfWork;
using SecurityServer.DataAccess.SecurityServerContext;

namespace SecurityServer.DataAccess.UnitOfWork
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly DbContextServer _context;
        private IRoleRepository? _roleRepository;
        private IUserRepository? _userRepository;
        private IApplicationRepository? _applicationRepository;
        private IApplicationUserRoleRepository? _applicationUserRoleRepository;
        private IGrantRepository? _grantRepository;

        public UnitOfWork(DbContextServer context)
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

        public IApplicationRepository ApplicationRepository => _applicationRepository ?? new ApplicationRepository(_context);

        public IApplicationUserRoleRepository ApplicationUserRoleRepository
        {
            get { return _applicationUserRoleRepository = _applicationUserRoleRepository ?? new ApplicationUserRoleRepository(_context); }
        }

        public IGrantRepository GrantRepository
        {
            get { return _grantRepository = _grantRepository ?? new GrantRepository(_context); }
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
