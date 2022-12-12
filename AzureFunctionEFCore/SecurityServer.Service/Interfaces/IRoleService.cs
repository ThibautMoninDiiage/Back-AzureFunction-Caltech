using SecurityServer.Service.DTO.Down;

namespace SecurityServer.Service.Interfaces
{
    public interface IRoleService
    {
        public Task<IEnumerable<RoleDtoDown>> GetAll();
    }
}
