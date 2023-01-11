using SecurityServer.Contract.Repositories;

namespace SecurityServer.Contract.UnitOfWork
{
    public interface IUnitOfWork
    {
        IApplicationRepository ApplicationRepository { get; }
        IRoleRepository RoleRepository { get; }
        IUserRepository UserRepository { get; }
        IApplicationUserRoleRepository ApplicationUserRoleRepository { get; }
        void Commit();
        void Rollback();
        Task CommitAsync();
        Task RollbackAsync();
    }
}
