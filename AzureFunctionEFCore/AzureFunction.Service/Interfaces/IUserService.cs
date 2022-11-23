using AzureFunction.Models;

namespace AzureFunction.Service.Interfaces
{
    public interface IUserService
    {
        public Task<User> Get(string login,string password);
    }
}
