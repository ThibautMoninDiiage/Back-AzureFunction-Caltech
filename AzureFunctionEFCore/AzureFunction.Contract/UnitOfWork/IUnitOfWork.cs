﻿using AzureFunction.Contract.Repositories;

namespace AzureFunction.Contract.UnitOfWork
{
    public interface IUnitOfWork
    {
        IRoleRepository RoleRepository { get; }
        IUserRepository UserRepository { get; }
        void Commit();
        void Rollback();
        Task CommitAsync();
        Task RollbackAsync();
    }
}
