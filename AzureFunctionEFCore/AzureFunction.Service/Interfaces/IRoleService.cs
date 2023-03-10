using AzureFunction.Context.Models;

namespace AzureFunction.Service.Interfaces
{
    public interface IRoleService
    {
        public Task<IEnumerable<Role>> GetAll();
    }
}
