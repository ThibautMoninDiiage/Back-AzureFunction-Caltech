using SecurityServer.Models.Models;

namespace SecurityServer.Service.Interfaces
{
    public interface IRoleService
    {
        public Task<IEnumerable<Role>> GetAll();
    }
}
