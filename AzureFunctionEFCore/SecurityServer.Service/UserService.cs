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

        public async Task<UserDtoDown?> Authenticate(UserDtoUp model)
        {
            User user = await _uow.UserRepository.GetAsync(x => x.Mail == model.Mail);

            if (user == null)
                return null;

            string hashedPassword = _jwtService.HashPasswordWithSalt(model.Password, Convert.FromBase64String(user.Salt));

            if (user.Password != hashedPassword)
                return null;

            string? token = _jwtService.generateJwtToken(user);

            return new UserDtoDown(user, token);
        }

        public async Task<UserDtoDown> CreateUser(UserCreationDtoUp model)
        {

            User userVerify = await _uow.UserRepository.GetAsync(x => x.Mail == model.Mail);

            if (userVerify != null)
                return null;

            var salt = _jwtService.GenerateSalt();
            var hashedPassword = _jwtService.HashPasswordWithSalt(model.Password, salt);

            // A modifier

            var user = new User
            {
                Mail = model.Mail,
                Avatar = model.Avatar,
                Username = model.Username,
                Password = hashedPassword,
                FirstName = model.FirstName,
                LastName = model.LastName,
                //IdRole = 2,
                Salt = Convert.ToBase64String(salt)
            };

            _uow.UserRepository.Add(user);
            await _uow.CommitAsync();

            User userCreated = await _uow.UserRepository.GetAsync(x => x.Mail == model.Mail);

            var token = _jwtService.generateJwtToken(userCreated);
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
    }
}