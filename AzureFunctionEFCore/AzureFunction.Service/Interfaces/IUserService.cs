using SecurityServer.Models.DTO.Up;
using SecurityServer.Models.DTO.Down;
using SecurityServer.Models.Models;

namespace SecurityServer.Service.Interfaces
{
    public interface IUserService
    {
        public Task<User> Get(string login, string password);
        public Task<UserDtoDown> GetById(int? id);
    }
}
