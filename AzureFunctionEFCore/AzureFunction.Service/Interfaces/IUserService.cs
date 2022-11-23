using AzureFunction.Models;

namespace AzureFunction.Service.Interfaces
{
    public interface IUserService
    {
        public Task<Role> Get(string login,string password);
    }
}
