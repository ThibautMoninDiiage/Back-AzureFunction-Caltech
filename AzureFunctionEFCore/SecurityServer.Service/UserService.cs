using SecurityServer.Contract.UnitOfWork;
using SecurityServer.Models.Models;
using SecurityServer.Service.DTO.Down;
using SecurityServer.Service.DTO.Up;
using SecurityServer.Service.Interfaces;

namespace SecurityServer.Service
{
    public class UserService : IUserService
    {
        private readonly IUnitOfWork _uow;
        private readonly IJwtService _jwtService;
        public UserService(IUnitOfWork uow,IJwtService jwtService)
        {
            _uow = uow;
            _jwtService = jwtService;
        }

        public async Task<UserGetByIdDtoDown?> GetById(int? id)
        {
            User user = await _uow.UserRepository.GetAsync(x => x.Id == id);

            IEnumerable<ApplicationUserRole> applicationUserRoles = await _uow.ApplicationUserRoleRepository.GetAllAsync(a => a.UserId == user.Id);

            List<ApplicationByUserDtoDown> applications = new List<ApplicationByUserDtoDown>();

            foreach (var item in applicationUserRoles)
            {
                Application application = await _uow.ApplicationRepository.GetAsync(a => a.Id == item.ApplicationId);
                ApplicationByUserDtoDown applicationByUserDtoDown = new ApplicationByUserDtoDown() { Id = application.Id,Description = application.Description, Name = application.Name,RedirectUri = application.RedirectUri, Url = application.Url };
                Role role = await _uow.RoleRepository.GetAsync(r => r.Id == item.RoleId);
                RoleByApplicationIdDtoDown roleByApplicationIdDtoDown = new RoleByApplicationIdDtoDown() { Id = role.Id,Name = role.Name};
                applicationByUserDtoDown.Role = roleByApplicationIdDtoDown;
                applications.Add(applicationByUserDtoDown);
            }

            UserGetByIdDtoDown userGetByIdDtoDown = new UserGetByIdDtoDown()
            {
                Id = user.Id,
                Firstname = user.FirstName,
                Lastname = user.LastName,
                Mail = user.Mail,
                Username = user.Username,
                Avatar = user.Avatar,
                Applications = applications
            };

            if (user != null)
                return userGetByIdDtoDown;
            else
                return null;

        }

        public async Task<string> Authenticate(UserDtoUp model)
        {
            User user = await _uow.UserRepository.GetAsync(x => x.Mail == model.Mail);

            if (user == null) return null;

            ApplicationUserRole applicationUserRole = await _uow.ApplicationUserRoleRepository.GetAsync(x => x.ApplicationId == 1 && x.UserId == user.Id);

            if (applicationUserRole == null) return null;

            string hashedPassword = _jwtService.HashPasswordWithSalt(model.Password, Convert.FromBase64String(user.Salt));

            if (user.Password != hashedPassword)
                return null;

            Guid grantCode = _jwtService.GenerateGrantCode();

            Application application = await _uow.ApplicationRepository.GetAsync(a => a.Id == 1);

            Grant grantVerify = await _uow.GrantRepository.GetAsync(g => g.UserId == user.Id && g.ApplicationId == 1);

            if (grantVerify != null) 
                return application.Url + "&code=" + grantVerify.ToString();
            else
            {
                Grant grant = new Grant() { ApplicationId = 1, UserId = user.Id, Code = grantCode.ToString(), CreatedAt = DateTime.Now };

                _uow.GrantRepository.Add(grant);
                await _uow.CommitAsync();

                return application.Url + "&code=" + grantCode.ToString();
            }


        }

        public async Task<UserDtoDown> CreateUser(UserCreationDtoUp model)
        {

            User userVerify = await _uow.UserRepository.GetAsync(x => x.Mail == model.Mail);
            Role role = await _uow.RoleRepository.GetAsync(x => x.Name == model.Role.Name);

            if (userVerify != null) return null;

            var salt = _jwtService.GenerateSalt();
            var hashedPassword = _jwtService.HashPasswordWithSalt(model.Password, salt);

            User user = new User
            {
                Mail = model.Mail,
                Avatar = model.Avatar,
                Username = model.Username,
                Password = hashedPassword,
                FirstName = model.FirstName,
                LastName = model.LastName,
                Salt = Convert.ToBase64String(salt)
            };

            _uow.UserRepository.Add(user);
            await _uow.CommitAsync();

            User userCreated = await _uow.UserRepository.GetAsync(x => x.Mail == model.Mail);

            ApplicationUserRole applicationUserRole = new ApplicationUserRole()
            {
                UserId = userCreated.Id,
                RoleId = role.Id,
                ApplicationId = model.idApplication,
            };

            _uow.ApplicationUserRoleRepository.Add(applicationUserRole);
            await _uow.CommitAsync();

            var token = _jwtService.GenerateJwtToken(userCreated.Id,role.Id);
            return new UserDtoDown(user, token);
        }

        public async Task<User> UpdateUser(UserModifyDtoUp model)
        {
            User user = new User()
            {
                Id = model.Id,
                FirstName = model.FirstName,
                LastName = model.LastName,
                Username = model.Username,
                Mail = model.Mail,
                Avatar = model.Avatar
            };
            _uow.UserRepository.Update(user);
            await _uow.CommitAsync();
            return user;
        }

        public async Task<string> AuthenticateWithUrl(UserDtoUp model)
        {
            User user = await _uow.UserRepository.GetAsync(x => x.Mail == model.Mail);

            if (user == null) return null;

            Application application = await _uow.ApplicationRepository.GetAsync(x => x.Url == model.Url);

            if (application == null) return null;

            ApplicationUserRole applicationUserRole = await _uow.ApplicationUserRoleRepository.GetAsync(x => x.ApplicationId == application.Id && x.UserId == user.Id);

            if (applicationUserRole == null) return null;

            string hashedPassword = _jwtService.HashPasswordWithSalt(model.Password, Convert.FromBase64String(user.Salt));

            if (user.Password != hashedPassword)
                return null;

            Guid grantCode = _jwtService.GenerateGrantCode();

            Grant grantVerify = await _uow.GrantRepository.GetAsync(g => g.UserId == user.Id && g.ApplicationId == application.Id);

            if (grantVerify != null)
            {
                if (application.Url.LastIndexOf('/') == application.Url.Length - 1)
                    return application.Url + "?code=" + grantVerify.ToString();
                else
                    return application.Url + "/?code=" + grantVerify.ToString();
            }     
            else
            {
                Grant grant = new Grant() { ApplicationId = application.Id, UserId = user.Id, Code = grantCode.ToString(), CreatedAt = DateTime.Now };

                _uow.GrantRepository.Add(grant);
                await _uow.CommitAsync();

                if(application.Url.LastIndexOf('/') == application.Url.Length -1)
                    return application.Url + "?code=" + grantCode.ToString();
                else
                    return application.Url + "/?code=" + grantCode.ToString();
            }
        }

        public async Task<UserDtoDown> GetToken(string codeGrant)
        {
            Grant grant = await _uow.GrantRepository.GetAsync(g => g.Code == codeGrant);

            ApplicationUserRole applicationUserRole = await _uow.ApplicationUserRoleRepository.GetAsync(a => a.ApplicationId == grant.ApplicationId && a.UserId == grant.UserId);

            User user = await _uow.UserRepository.GetAsync(u => u.Id == applicationUserRole.UserId);

            string token = _jwtService.GenerateJwtToken(applicationUserRole.UserId, applicationUserRole.RoleId);

            _uow.GrantRepository.Remove(grant);
            await _uow.CommitAsync();

            return new UserDtoDown(user, token);
        }

        public async Task<bool> AddExistantUser(AddUserInApplicationDtoDown model)
        {
            Role role = await _uow.RoleRepository.GetAsync(r => r.Name == model.Role.Name);

            ApplicationUserRole applicationUserRole = new ApplicationUserRole() { ApplicationId = model.ApplicationId, RoleId = role.Id, UserId = model.UserId };
            _uow.ApplicationUserRoleRepository.Add(applicationUserRole);
            await _uow.CommitAsync();
            return true;
        }
    }
}