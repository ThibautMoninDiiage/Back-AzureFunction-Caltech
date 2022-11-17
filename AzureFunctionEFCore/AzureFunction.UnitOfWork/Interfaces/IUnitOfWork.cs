using AzureFunction.Context.Models;
using AzureFunction.Repository.Interfaces;

namespace AzureFunction.UnitOfWork.Interfaces
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
