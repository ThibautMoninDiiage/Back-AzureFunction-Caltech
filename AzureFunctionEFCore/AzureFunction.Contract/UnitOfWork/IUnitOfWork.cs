using AzureFunction.Contrat.Repositories;

namespace AzureFunction.Contrat.UnitOfWork
{
    public interface IUnitOfWork
    {
        IRoleRepository RoleRepository { get; }
        void Commit();
        void Rollback();
        Task CommitAsync();
        Task RollbackAsync();
    }
}
