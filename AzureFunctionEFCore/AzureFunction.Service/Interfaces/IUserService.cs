using AzureFunction.Models;
using AzureFunction.Models.DTO.Down;
using AzureFunction.Models.DTO.Up;

namespace AzureFunction.Service.Interfaces
{
    public interface IUserService
    {
        public Task<User> Get(string login,string password);
        public Task<UserDtoDown> GetById(int? id);
    }
}
