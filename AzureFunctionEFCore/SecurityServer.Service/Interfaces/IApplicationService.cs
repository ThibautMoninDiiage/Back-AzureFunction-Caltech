using SecurityServer.Models.Models;
using SecurityServer.Service.DTO.Up;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SecurityServer.Service.Interfaces
{
    public interface IApplicationService
    {
        public Task<IEnumerable<Application>> GetAllApplications();
        public Task<Application> GetById(int id);

        public Task<Application> CreateApplication(ApplicationCreationDtoUp application);

        public Task<Application> UpdateApplication(Application application);

        public Task<string> DeleteApplication(Application application);

        public Task<string> DeleteApplication(int appId);
    }
}
