using SecurityServer.Models.Models;
using SecurityServer.Service.DTO.Down;
using SecurityServer.Service.DTO.Up;

namespace SecurityServer.Service.Interfaces
{
    public interface IUserService
    {
        public Task<UserGetByIdDtoDown> GetById(int? id);
        public Task<GrantDtoDown> Authenticate(UserDtoUp model);
        public Task<UserDtoDown> GetToken(string codeGrant); 
        public Task<UserDtoDown> CreateUser(UserCreationDtoUp model);
        public Task<User> UpdateUser(UserModifyDtoUp model);
        public Task<bool> AddExistantUser(AddUserInApplicationDtoDown model);
        public Task<bool> DeleteUser(int userId);
    }
}
