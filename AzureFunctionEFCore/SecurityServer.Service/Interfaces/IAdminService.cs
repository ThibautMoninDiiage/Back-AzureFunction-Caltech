﻿using SecurityServer.Models.Models;
using SecurityServer.Service.DTO.Down;
using SecurityServer.Service.DTO.Up;

namespace SecurityServer.Service.Interfaces
{
    public interface IAdminService
    {
        public Task<User> CreateUser(UserCreationDtoUp model);
        public Task<List<UserAllDtoDown>> GetAllUsers();
        public Role GetRoleById(int id);
    }
}
