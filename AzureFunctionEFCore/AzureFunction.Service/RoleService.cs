using AzureFunction.Context.Models;
using AzureFunction.Service.Interfaces;
using AzureFunction.UnitOfWork.Interfaces;

namespace AzureFunction.Service
{
    public class RoleService : IRoleService
    {
        private readonly IUnitOfWork _uow;
        public RoleService(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<IEnumerable<Role>> GetAll()
        {
            return await _uow.RoleRepository.GetAllAsync();
        }
    }
}
