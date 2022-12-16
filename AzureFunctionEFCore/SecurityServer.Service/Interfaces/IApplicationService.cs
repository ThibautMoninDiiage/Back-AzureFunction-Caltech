﻿using SecurityServer.Models.Models;
using SecurityServer.Service.DTO.Down;
using SecurityServer.Service.DTO.Up;

namespace SecurityServer.Service.Interfaces
{
    public interface IApplicationService
    {
        public Task<IEnumerable<ApplicationDtoDown>> GetAllApplications();

        public Task<Application?> GetById(int id);

        public Task<Application> CreateApplication(ApplicationCreationDtoUp application);

        public Task<Application> UpdateApplication(ApplicationUpdateDtoUp application);

        public Task<string> DeleteApplication(Application application);

        public Task<string> DeleteApplication(int appId);
    }
}