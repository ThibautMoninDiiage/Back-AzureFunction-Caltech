using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using SecurityServer.Contract.UnitOfWork;
using SecurityServer.Models.Models;
using SecurityServer.Service.DTO.Down;
using SecurityServer.Service.DTO.Up;
using SecurityServer.Service.Interfaces;
using System.Security.Cryptography;

namespace SecurityServer.Service
{
    public class AdminService : IAdminService
    {
        private readonly IUnitOfWork _uow;

        public AdminService(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<UserAdminDtoDown> CreateUser(UserCreationDtoUp model)
        {

            User userVerify = await _uow.UserRepository.GetAsync(x => x.Mail == model.Mail);

            if (userVerify != null) return null;

            var salt = GenerateSalt();
            var hashedPassword = HashPasswordWithSalt(model.Password, salt);

            var user = new User
            {
                Mail = model.Mail,
                Username = model.Username,
                Password = hashedPassword,
                FirstName = model.FirstName,
                LastName = model.LastName,
                Avatar = model.Avatar,
                Salt = Convert.ToBase64String(salt)
            };

            _uow.UserRepository.Add(user);
            await _uow.CommitAsync();

            User userCreated = await _uow.UserRepository.GetAsync(x => x.Mail == model.Mail);

            Role role = await _uow.RoleRepository.GetAsync(r => r.Name == model.Role.Name);

            ApplicationUserRole applicationUserRole = new ApplicationUserRole() { ApplicationId = model.idApplication,UserId = userCreated.Id, RoleId = role.Id};

            _uow.ApplicationUserRoleRepository.Add(applicationUserRole);
            await _uow.CommitAsync();

            UserAdminDtoDown userAdminDtoDown = new UserAdminDtoDown()
            {
                Id = userCreated.Id,
                Firstname = userCreated.FirstName,
                Lastname = userCreated.LastName,
                Mail = model.Mail,
                Username = model.Username
            };

            return userAdminDtoDown;
        }

        public async Task<List<UserAllDtoDown>> GetAllUsers()
        {

            List<User> users = _uow.UserRepository.GetAllAsync().Result.ToList();
            List<UserAllDtoDown> userAllDtoDowns = new List<UserAllDtoDown>();

            users.ForEach(u => userAllDtoDowns.Add(new UserAllDtoDown() { Id = u.Id, Avatar = u.Avatar, Mail = u.Mail, Username = u.Username}));

            return userAllDtoDowns;
        }

        public Role GetRoleById(int id)
        {
            return _uow.RoleRepository.Get(x => x.Id == id);
        }

        private byte[] GenerateSalt()
        {
            byte[] salt = new byte[128 / 8];
            using (var rngCsp = new RNGCryptoServiceProvider())
            {
                rngCsp.GetNonZeroBytes(salt);
            }

            return salt;
        }

        private string HashPasswordWithSalt(string password, byte[] salt)
        {
            // obtenir une clé de 256-bit (en utilisant HMACSHA256 sur 100,000 itérations)
            return Convert.ToBase64String(KeyDerivation.Pbkdf2(
                password: password,
                salt: salt,
                prf: KeyDerivationPrf.HMACSHA256,
                iterationCount: 100000,
                numBytesRequested: 256 / 8));
        }
    }
}
