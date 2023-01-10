using SecurityServer.Contract.UnitOfWork;
using SecurityServer.Models.Models;
using SecurityServer.Service.DTO.Down;
using SecurityServer.Service.DTO.Up;
using SecurityServer.Service.Interfaces;
using System.Linq.Expressions;

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

                Application committed = new()
                {
                    Description = application.Description,
                    Name = application.Name,
                    Url = application.Url,
                    RedirectUri = application.RedirectUri
                    //Users = new(), A modifier
                };

                _iuow.ApplicationRepository.Add(committed);
                _iuow.Commit();

                return _iuow.ApplicationRepository.Get(app => 
                    app.Name == committed.Name &&
                    app.Url == committed.Url &&
                    app.Description == (committed.Description ?? "") &&
                    app.RedirectUri == committed.RedirectUri
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

        public async Task<Application?> GetById(int id)
        {
            return await Task.Run(async () =>
            {
                Type type = typeof(Application);
                ParameterExpression member = Expression.Parameter(type, "param");
                MemberExpression fieldId = Expression.PropertyOrField(member, "id");
                Expression<Func<Application, bool>> requete = Expression.Lambda<Func<Application, bool>>(Expression.Equal(fieldId, Expression.Constant(id)), member);

                Application user = await _iuow.ApplicationRepository.GetAsync(requete);

                if (user != null)
                    return user;
                else
                    return null;
            });
        }

        public async Task<Application> UpdateApplication(ApplicationUpdateDtoUp update)
        {
            return await Task.Run(() =>
            {
                Application app = new()
                {
                    Id = (int)update.Id,
                    Description = update.Description,
                    Name = update.Name,
                    Url = update.Url
                };

                _iuow.ApplicationRepository.Update(app);
                _iuow.Commit();
                return app;
            });
        }
    }
}
