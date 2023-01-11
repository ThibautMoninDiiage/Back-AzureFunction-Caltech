using SecurityServer.Models.Models;
using SecurityServer.Service.DTO.Down;
using SecurityServer.Service.DTO.Up;

namespace SecurityServer.Service.Interfaces
{
    public interface IUserService
    {
        public Task<User> GetById(int? id);
        public Task<Guid?> Authenticate(UserDtoUp model);
        public Task<UserDtoDown> AuthenticateWithUrl(UserDtoUp model);
        public Task<UserDtoDown> CreateUser(UserCreationDtoUp model);
        public Task<User> UpdateUser(UserModifyDtoUp model);
    }
}
