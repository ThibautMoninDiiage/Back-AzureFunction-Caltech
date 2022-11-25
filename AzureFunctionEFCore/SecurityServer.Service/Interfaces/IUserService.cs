using SecurityServer.Models.Models;
using SecurityServer.Service.DTO.Down;
using SecurityServer.Service.DTO.Up;

namespace SecurityServer.Service.Interfaces
{
    public interface IUserService
    {
        public Task<User> GetById(int? id);
        public Task<UserDtoDown> Authenticate(UserDtoUp model);
        public Task<User> CreateUser(User model);
    }
}
