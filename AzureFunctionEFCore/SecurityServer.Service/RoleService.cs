using SecurityServer.Contract.UnitOfWork;
using SecurityServer.Models.Models;
using SecurityServer.Service.DTO.Down;
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

        public async Task<IEnumerable<RoleDtoDown>> GetAll()
        {
            IEnumerable<Role> lstRoles = await _uow.RoleRepository.GetAllAsync();

            List<RoleDtoDown> lstRoleDtoDowns = new ();

            lstRoles.ToList().ForEach(r => lstRoleDtoDowns.Add(new RoleDtoDown() { Id = r.Id, Name = r.Name }));

            return lstRoleDtoDowns;

        }
    }
}
