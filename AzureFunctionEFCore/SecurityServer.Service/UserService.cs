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

        public async Task<User?> GetById(int? id)
        {
            User user = await _uow.UserRepository.GetAsync(x => x.Id == id);

            if (user != null)
                return user;
            else
                return null;

        }

        public async Task<Guid?> Authenticate(UserDtoUp model)
        {
            User user = await _uow.UserRepository.GetAsync(x => x.Mail == model.Mail);

            if (user == null) return null;

            ApplicationUserRole applicationUserRole = await _uow.ApplicationUserRoleRepository.GetAsync(x => x.Application.Id == 1 && x.User.Id == user.Id);

            if (applicationUserRole == null) return null;

            string hashedPassword = _jwtService.HashPasswordWithSalt(model.Password, Convert.FromBase64String(user.Salt));

            if (user.Password != hashedPassword)
                return null;

            Guid grantCode = _jwtService.GenerateGrantCode();

            Grant grant = new Grant() { ApplicationId = 1,UserId = user.Id, Code = grantCode };


            //string? token = _jwtService.generateJwtToken(user.Id,applicationUserRole.RoleId);

            return grantCode;
        }

        public async Task<UserDtoDown> CreateUser(UserCreationDtoUp model)
        {

            User userVerify = await _uow.UserRepository.GetAsync(x => x.Mail == model.Mail);
            Role role = await _uow.RoleRepository.GetAsync(x => x.Name == model.Role.Name);

            if (userVerify != null) return null;

            var salt = _jwtService.GenerateSalt();
            var hashedPassword = _jwtService.HashPasswordWithSalt(model.Password, salt);

            // A modifier

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

            var token = _jwtService.generateJwtToken(userCreated.Id,role.Id);
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

        public async Task<UserDtoDown> AuthenticateWithUrl(UserDtoUp model)
        {
            User user = await _uow.UserRepository.GetAsync(x => x.Mail == model.Mail);

            if (user == null) return null;

            Application application = await _uow.ApplicationRepository.GetAsync(x => x.Url == model.Url);

            if (application == null) return null;

            ApplicationUserRole applicationUserRole = await _uow.ApplicationUserRoleRepository.GetAsync(x => x.Application.Id == application.Id && x.User.Id == user.Id);

            if (applicationUserRole == null) return null;

            string hashedPassword = _jwtService.HashPasswordWithSalt(model.Password, Convert.FromBase64String(user.Salt));

            if (user.Password != hashedPassword)
                return null;

            string? token = _jwtService.generateJwtToken(user.Id, applicationUserRole.RoleId);

            return new UserDtoDown(user, token);
        }
    }
}