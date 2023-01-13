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
            List<ApplicationDtoDown> applicationsDtoDown = new List<ApplicationDtoDown>();
            IEnumerable<Application> applications = await _iuow.ApplicationRepository.GetAllAsync();

            foreach (var application in applications)
            {
                IEnumerable<ApplicationUserRole> applicationUserRoles = await _iuow.ApplicationUserRoleRepository.GetAllAsync(a => a.ApplicationId == application.Id);
                List<UserByApplicationDtoDown> users = new List<UserByApplicationDtoDown>();

                foreach (var applicationUserRole in applicationUserRoles)
                {
                    User user = await _iuow.UserRepository.GetAsync(u => u.Id == applicationUserRole.UserId);
                    Role role = await _iuow.RoleRepository.GetAsync(r => r.Id == applicationUserRole.RoleId);

                    RoleByApplicationIdDtoDown roleByApplicationIdDtoDown = new RoleByApplicationIdDtoDown() { Id = role.Id, Name = role.Name };

                    UserByApplicationDtoDown userByApplicationDtoDown = new UserByApplicationDtoDown()
                    {
                        Id = user.Id,
                        Firstname = user.FirstName,
                        Lastname = user.LastName,
                        Avatar = user.Avatar,
                        Mail = user.Mail,
                        Username = user.Username,
                        Role = roleByApplicationIdDtoDown
                    };

                    users.Add(userByApplicationDtoDown);
                }

                ApplicationDtoDown applicationDtoDown = new ApplicationDtoDown() { Id = application.Id,Description = application.Description,Name = application.Name,Url = application.Url,Users = users};
                applicationsDtoDown.Add(applicationDtoDown);

            }

            return applicationsDtoDown;
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

        public async Task<bool> DeleteApplication(int appId)
        {
            Application target = await _iuow.ApplicationRepository.GetAsync(a => a.Id == appId);
            IEnumerable<ApplicationUserRole> applicationUserRole = await _iuow.ApplicationUserRoleRepository.GetAllAsync(a => a.ApplicationId == target.Id);

            if(applicationUserRole != null)
            {
                foreach (var item in applicationUserRole)
                {
                    _iuow.ApplicationUserRoleRepository.Remove(item);
                }
            }

            _iuow.ApplicationRepository.Remove(target);
            await _iuow.CommitAsync();

            return true;
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

        public async Task<List<ApplicationUserDtoDown>> GetUserWhereIsNotInAppli(int idApp)
        {
            List<ApplicationUserDtoDown> applicationUserDtoDowns = new List<ApplicationUserDtoDown>();

            //IEnumerable<Application> applications = await _iuow.ApplicationRepository.GetAllAsync(a => a.Id == idApp);
            IEnumerable<ApplicationUserRole> applicationUserRoles = await _iuow.ApplicationUserRoleRepository.GetAllAsync(a => a.ApplicationId == idApp);
            IEnumerable<User> users = await _iuow.UserRepository.GetAllAsync();

            IEnumerable<User> result = users.ExceptBy(applicationUserRoles.Select(a => a.UserId),u => u.Id);

            foreach (var item in result)
            {
                ApplicationUserDtoDown applicationUserDtoDown = new ApplicationUserDtoDown() { Id = item.Id, Mail = item.Mail };
                applicationUserDtoDowns.Add(applicationUserDtoDown);
            }

            return applicationUserDtoDowns;
        }
    }
}
