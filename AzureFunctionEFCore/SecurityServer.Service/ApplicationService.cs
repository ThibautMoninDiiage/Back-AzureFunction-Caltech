using SecurityServer.Contract.UnitOfWork;
using SecurityServer.Models.Models;
using SecurityServer.Service.DTO.Down;
using SecurityServer.Service.DTO.Up;
using SecurityServer.Service.Interfaces;

namespace SecurityServer.Service
{
    public class ApplicationService : IApplicationService
    {
        private readonly IUnitOfWork _iuow;

        public ApplicationService(IUnitOfWork iuow)
        {
            _iuow = iuow;
        }   

        public async Task<IEnumerable<ApplicationDtoDown>> GetAllApplications()
        {
            return 
                (await _iuow.ApplicationRepository.GetAllAsync())
                .Select(app => new ApplicationDtoDown
                {
                    Id = app.Id,
                    Description = app.Description,
                    Name = app.Name,
                    Url = app.Url
                });
        }

        public async Task<Application> CreateApplication(ApplicationCreationDtoUp application)
        {
            return await Task.Run(() =>
            {
                List<User> users =  _iuow.UserRepository.GetAll(u => (application.UserIds ?? new List<int>()).Any(id => u.Id == id)).ToList();

                Application committed = new()
                {
                    Description = application.Description,
                    Name = application.Name,
                    Url = application.Url,
                    Users = users,
                };

                _iuow.ApplicationRepository.Add(committed);
                _iuow.Commit();

                return _iuow.ApplicationRepository.Get(app => 
                    app.Name == committed.Name &&
                    app.Url == committed.Url &&
                    app.Description == (committed.Description ?? "")
                );
            });
        }

        public async Task<string> DeleteApplication(Application application)
        {
            return await Task.Run(() =>
            {
                _iuow.ApplicationRepository.Remove(application);
                _iuow.Commit();
                return "Succesfully removed the application.";
            });
        }

        public async Task<string> DeleteApplication(int appId)
        {
            Application target = await GetById(appId);

            return await DeleteApplication(target);
        }

        public async Task<Application> GetById(int id)
        {
            return await Task.Run(() =>
            {
                return _iuow.ApplicationRepository.Get(app => app.Id == id);
            });
        }

        public async Task<Application> UpdateApplication(ApplicationUpdateDtoUp update)
        {
            return await Task.Run(() =>
            {
                IEnumerable<User> users = _iuow.UserRepository.GetAll(u => (update.UserIds ?? new List<int>()).Any(id => id == u.Id));
                Application baseApplication = _iuow.ApplicationRepository.Get(app => app.Id == update.Id);
                Application app = new()
                {
                    Id = (int)update.Id,
                    Description = update.Description ?? baseApplication.Description,
                    Name = update.Description ?? baseApplication.Description,
                    Url = update.Url ?? baseApplication.Url,
                    Users = users.ToList()
                };

                _iuow.ApplicationRepository.Update(app);
                _iuow.Commit();
                return app;
            });
        }
    }
}
