using SecurityServer.Contract.UnitOfWork;
using SecurityServer.Models.Models;
using SecurityServer.Service.Interfaces;

namespace SecurityServer.Service
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
